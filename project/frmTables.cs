using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ORMgenerator
{
    public partial class frmTables : Form
    {
       

        private static class DBtypeProperties
        {
            public static dbType DBtype { get; set; }
            public static string encloser { get; set; }
            public static string selectSQLgetLastId { get; set; }
            public static string selectSQL { get; set; }
        }

        public frmTables(dbType DBtype)
        {
            DBtypeProperties.DBtype = DBtype;

            InitializeComponent();
        }

        private void frmTables_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            FillDatabaseTables();

            Cursor = Cursors.Default;
        }

        private void FillDatabaseTables()
        {
            DataTable dt = null;

            switch (DBtypeProperties.DBtype)
            {
                case dbType.mysql:
                    DBtypeProperties.encloser = "`{0}`";
                    DBtypeProperties.selectSQLgetLastId = "SELECT @@IDENTITY";
                    DBtypeProperties.selectSQL = "SELECT * FROM `{0}` LIMIT 1";
                    
                    //get tables
                    dt = General.db.GetDataTable("show tables");
                    break;
                case dbType.sqlite:
                    DBtypeProperties.encloser = "\"\"{0}\"\"";
                    DBtypeProperties.selectSQLgetLastId = "SELECT last_insert_rowid()";
                    DBtypeProperties.selectSQL = "SELECT * FROM \"{0}\" LIMIT 1";

                    //get tables
                    dt = General.db.GetDataTable("SELECT tbl_name FROM sqlite_master where type = 'table' ORDER BY name");
                    break;
                case dbType.oracle:
                    DBtypeProperties.encloser = "\"\"{0}\"\"";
                    DBtypeProperties.selectSQLgetLastId = "not_supported_in_oracle";
                    DBtypeProperties.selectSQL = "SELECT * FROM \"{0}\" WHERE ROWNUM <= 1";

                    //get tables - https://www.oracletutorial.com/oracle-administration/oracle-show-tables/
                    dt = General.db.GetDataTable("SELECT table_name FROM all_tables ORDER BY table_name");
                    //as this applicaiton will be used on DEV machine, by default the owner omitted, in case needed the below SQL includes also the owner
                    //dt = General.db.GetDataTable("SELECT CONCAT(CONCAT(owner , '\".\"' ), table_name) as tbl FROM all_tables ORDER BY table_name");
                    //if there is owner.table (jess.customer) and want to be enclosed as "juess"."customer"
                    //but do a favor to youself, quit your job if using ORACLE at 2023.
                    break;
                case dbType.sqlserver:
                case dbType.sqlserver_integrated:
                case dbType.adonet_for_accdb:
                case dbType.adonet_for_mdb:
                case dbType.adonet_for_xls:
                case dbType.adonet_for_xlsx:
                    DBtypeProperties.encloser = "[{0}]";
                    DBtypeProperties.selectSQLgetLastId = "SELECT @@IDENTITY"; //only for SQLSERVER
                    DBtypeProperties.selectSQL = "SELECT TOP 1 * FROM [{0}];";
                    break;
                default:
                    break;
            }

            var f = dt.AsEnumerable().Select(x => x.Field<object>(0));

            lst.SuspendLayout();
            lst.Items.AddRange(f.Cast<object>().ToArray());
            lst.ResumeLayout();

            SetLabelTotals();
        }

        #region " useless form methods "

        private void btnAll_Click(object sender, EventArgs e)
        {
            CheckUnCheck(true);
        }

        private void CheckUnCheck(bool val)
        {
            for (int i = 0; i < lst.Items.Count; i++)
                lst.SetItemChecked(i, val);

        }

        private void btnUnAll_Click(object sender, EventArgs e)
        {
            CheckUnCheck(false);
        }

        private void SetLabelTotals()
        {
            this.Text = "Total : " + lst.Items.Count;
        }

        #endregion

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (lst.CheckedItems.Count == 0)
            {
                General.Mes("Please select at least one table");
                return;
            }

            string baseDir = Application.StartupPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\\";
            string folderModel = baseDir + "Models\\";
            string folderService = baseDir + "Services\\";

            Directory.CreateDirectory(folderModel);
            Directory.CreateDirectory(folderService);

            string tableNameNormalized;
            string tableName;
            DataTable dT;

            ExportAs exportType = GetExportUserSelection();

            string errors = string.Empty;
            foreach (var item in lst.CheckedItems)
            {
                tableName = item.ToString();
                //normalize table name for /CLASS name declaration/ + FILENAME
                tableNameNormalized = ConvertToSingular(ReplaceInvalidChars(UppercaseFirst(tableName)));

                //Console.WriteLine(string.Format("select * from [{0}]", item.ToString()));
                dT = General.db.GetDataTableWithSchema(string.Format(DBtypeProperties.selectSQL, item.ToString()));

                if (dT == null)
                {
                    errors += tableName + "\r\n";
                    //General.Mes("Cannot generate " + tableName, MessageBoxIcon.Exclamation);
                    continue;
                }
                
                //Model file
                File.WriteAllText(folderModel + tableNameNormalized + ".cs",
                    GenerateModelFromDataTable(dT, tableNameNormalized));

                if (exportType == ExportAs.dapper)
                {
                    //Export SERVICE folder for # DAPPER #
                    File.WriteAllText(folderService + tableNameNormalized + "Service.cs",
                        DapperGenerateServiceFromDataTable(dT, tableNameNormalized, tableName, Properties.Resources.DapperServiceTemplate));
                }
                else if (exportType == ExportAs.dbasewrapper_reflection)
                {
                    //Export SERVICE folder for # DBASE Wrapper REFLECTION #
                    File.WriteAllText(folderService + tableNameNormalized + "Service.cs",
                        DapperGenerateServiceFromDataTable(dT, tableNameNormalized, tableName, Properties.Resources.DBASEWrapperReflectionServiceTemplate));
                }
            }

            //constant files
            File.WriteAllText(folderService + "ICRUDService.cs", Properties.Resources.constICRUDService);
            File.WriteAllText(baseDir + "DBASEWrapper.cs", Properties.Resources.constDBASEWrapper);
            File.WriteAllText(baseDir + "General.cs", Properties.Resources.constGeneral);

            if (!string.IsNullOrEmpty(errors))
                General.Mes("Cannot generate\r\n\r\n" + errors, MessageBoxIcon.Exclamation);


            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", baseDir));
        }

        public static string DapperGenerateServiceFromDataTable(DataTable dataTable, string className, string tableName, string serviceTemplate)
        {
            //constants
            string primaryKey = GetPrimaryKey(dataTable);
            string[] columnNamesArray = dataTable.Columns.Cast<DataColumn>().Where(column => !column.ColumnName.ToLower().Equals(primaryKey)).Select(column => column.ColumnName.ToLower()).ToArray();

            string insertColumnNames = string.Join(", ", columnNamesArray);
            string insertColumnValues = "@" + string.Join(", @", columnNamesArray);
            string insertOBJvalues = "obj." + string.Join(", obj.", columnNamesArray);
            string tableNameEnclosed = string.Format(DBtypeProperties.encloser, tableName);
            string updateFields = string.Join(", ", columnNamesArray.Select(columnName => string.Format("{0} = @{0}", columnName)));
            

            //to be used
            string outputINSERT = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableNameEnclosed, insertColumnNames, insertColumnValues);
            string outputINSERTgetNewId = DBtypeProperties.selectSQLgetLastId;
            string outputGetId = string.Format("SELECT * FROM {0} WHERE {1} = @{1}", tableNameEnclosed, primaryKey);
            string outputGetList = string.Format("SELECT * FROM {0}", tableNameEnclosed);
            string outputUPDATE = string.Format("UPDATE {0} SET {1} WHERE {2} = @{2}", tableNameEnclosed, updateFields, primaryKey);
            string outputeDELETE = string.Format("DELETE FROM {0} WHERE {1} = @{1}", tableNameEnclosed, primaryKey);

            string serviceOutput = serviceTemplate.Replace("{className}", className)
                .Replace("{outputINSERT}", outputINSERT).Replace("{insertOBJvalues}", insertOBJvalues)
                .Replace("{outputINSERTgetNewId}", outputINSERTgetNewId).Replace("{outputGetId}", outputGetId).Replace("{primaryKey}", primaryKey)
                .Replace("{outputGetList}", outputGetList).Replace("{outputUPDATE}", outputUPDATE).Replace("{outputeDELETE}", outputeDELETE);


            return serviceOutput;
        }

        private static string GetPrimaryKey(DataTable dataTable)
        {
             var primaryKeyColumnName = dataTable.Constraints
                                             .OfType<UniqueConstraint>()
                                             .FirstOrDefault(uc => uc.IsPrimaryKey);
             if (primaryKeyColumnName != null)
             {
                 return primaryKeyColumnName.Columns.FirstOrDefault().ToString().ToLower();
             }
             else
                 return "primarykey_not_found";
        }

        private static string GenerateModelFromDataTable(DataTable dataTable, string className)
        {
            StringBuilder sb = new StringBuilder();

            //dataTable.WriteXml(Application.StartupPath + "\\wr" + className + ".xml");
            //dataTable.WriteXmlSchema(Application.StartupPath + "\\wrSC" + className + ".xml");

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace Models");
            sb.AppendLine("{");

            sb.AppendLine(string.Format("    public class {0}", className));
            sb.AppendLine("    {");

            foreach (DataColumn column in dataTable.Columns)
            {
                string columnName = column.ColumnName.ToLower();
                string columnType = FixDataTypeUppercase(column.DataType.Name);
                bool allowDBNull = column.AllowDBNull;

                string propertyDeclaration = allowDBNull ? columnType + "?" : columnType;
                sb.AppendLine(string.Format("        public {0} {1} {{ get; set; }}", propertyDeclaration, columnName));
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string FixDataTypeUppercase(string datatype)
        {
            switch (datatype)
            {
                case "String":
                    return "string";
                case "Int":
                case "Int32":
                    return "int";
                case "Boolean":
                    return "bool";
                case "Decimal":
                    return "decimal";
                case "Float":
                    return "float";
                //case "MySqlDateTime":
                //    return "DateTime";
                default:
                    return datatype;
            }
        }

        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

        private static string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        private static string ConvertToSingular(string testWord)
        {
            if (testWord.EndsWith("us") && !testWord.EndsWith("ous"))
            {
                return testWord;
            }
            else if (testWord.EndsWith("ies"))
            {
                return testWord.Substring(0, testWord.Length - 3) + "y";
            }
            else if (testWord.EndsWith("es"))
            {
                return testWord.Substring(0, testWord.Length - 1);
            }
            else if (testWord.EndsWith("s"))
            {
                return testWord.Substring(0, testWord.Length - 1);
            }

            return testWord;
        }

        private enum ExportAs
        {
            dbasewrapper_reflection = 1,
            dbasewrapper_parameters = 2,
            dapper=3,
            
        }

        private ExportAs GetExportUserSelection()
        {
            return (ExportAs) int.Parse(groupExport.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Tag.ToString());
        }

    }
}
