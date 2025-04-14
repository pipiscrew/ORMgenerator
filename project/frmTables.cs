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
            else if (optDapper.Checked && chkWinForms.Checked)
            {
                General.Mes("Dapper & WinForms is not implemented. Operation aborted!");
                return;
            }

            string baseDir = Application.StartupPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\\";

            GenerateBase(baseDir);

            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", baseDir));
        }

        private void GenerateBase(string baseDir)
        {
            string folderModel = baseDir + "Models\\";
            string folderService = baseDir + "Services\\";
            string folderModelBase = baseDir + "Models\\Base\\";
            string folderServiceBase = baseDir + "Services\\Base\\";
            string folderServiceBaseHelper = baseDir + "Services\\Base\\Helper\\";

            Directory.CreateDirectory(folderModelBase);

            if (chkWinForms.Checked)
                Directory.CreateDirectory(folderServiceBaseHelper);
            else
                Directory.CreateDirectory(folderServiceBase);

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

                //ModelBase file
                File.WriteAllText(folderModelBase + tableNameNormalized + "Base.cs",
                    GenerateModelFromDataTable(dT, tableNameNormalized, chkWinForms.Checked));

                //Model file
                File.WriteAllText(folderModel + tableNameNormalized + ".cs", Properties.Resources.Model.Replace("{className}", tableNameNormalized));

                //Service file
                File.WriteAllText(folderService + tableNameNormalized + "Service.cs", Properties.Resources.Service.Replace("{className}", tableNameNormalized + "Service"));

                if (exportType == ExportAs.dapper)
                {
                    //Export SERVICE folder for # DAPPER #
                    File.WriteAllText(folderServiceBase + tableNameNormalized + "ServiceBase.cs",
                        DapperGenerateServiceFromDataTable(dT, tableNameNormalized, tableName, Properties.Resources.DapperServiceTemplate));
                }
                else if (exportType == ExportAs.dbasewrapper_reflection)
                {
                    //Export SERVICE folder for # DBASE Wrapper REFLECTION #
                    File.WriteAllText(folderServiceBase + tableNameNormalized + "ServiceBase.cs",
                        DapperGenerateServiceFromDataTable(dT, tableNameNormalized, tableName, 
                        Properties.Resources.DBASEWrapperReflectionServiceTemplate
                        .Replace("{winformGetListSortFunction}", chkWinForms.Checked ? Properties.Resources.DBASEWrapperReflectionServiceSortFunction : "")
                        .Replace("{winformGetExportEXCELFunction}", chkWinForms.Checked ? Properties.Resources.DBASEWrapperReflectionServiceExportEXCELFunction : "")));
                }

                if (chkWinForms.Checked)
                {
                    if (chkWinFormsDevExpress.Checked)
                        GenerateFormDevExpress(baseDir, tableNameNormalized, GetPrimaryKey(dT), dT);
                    else 
                        GenerateForm(baseDir, tableNameNormalized, GetPrimaryKey(dT), dT);
                }
            }

            //constant files
            File.WriteAllText(folderServiceBase + "ICRUDService.cs", Properties.Resources.constICRUDService);
            File.WriteAllText(baseDir + "DBASEWrapper.cs", Properties.Resources.constDBASEWrapper);
            File.WriteAllText(baseDir + "General.cs", Properties.Resources.constGeneral);

            if (chkWinForms.Checked)
            {
                File.WriteAllText(folderModelBase + "ModelBase.cs", Properties.Resources.constModelBase);
                File.WriteAllText(folderServiceBaseHelper + "SortableBindingList.cs", Properties.Resources.SortableBindingList);
                File.WriteAllText(folderServiceBaseHelper + "ExcelExport.cs", Properties.Resources.ExcelExport);

                if (chkWinFormsDevExpress.Checked)
                    File.WriteAllText(baseDir + "!DevExpress_Notes.txt", Properties.Resources.DevExpress_Notes);
            }

            if (!string.IsNullOrEmpty(errors))
                General.Mes("Cannot generate\r\n\r\n" + errors, MessageBoxIcon.Exclamation);
        }

        private void GenerateFormDevExpress(string dir, string entity, string PK, DataTable dT)
        {
            //FORM DESINGER
            string des = Properties.Resources.Form3devexpress_designer_cs;
            des = des.Replace("{entity}", entity);

            File.WriteAllText(dir + "frm" + entity + ".designer.cs", des);

            //FORM CODE + RES
            string code = Properties.Resources.Form3devexpress_cs;

            code = code.Replace("{entity}", entity)
                .Replace("{entityL}", entity.ToLower())
                .Replace("{PK}", UppercaseFirst(PK))
                .Replace("{PKL}", PK.ToLower());

            File.WriteAllText(dir + "frm" + entity + ".cs", code);
            File.WriteAllText(dir + "frm" + entity + ".resx", Properties.Resources.Form3devexpress_resx);
        }

        private void GenerateForm(string dir, string entity, string PK, DataTable dT)
        {
            //FORM DESINGER
            string lblDeclare = Properties.Resources.lblDeclare;
            string lblInit = Properties.Resources.lblInit;
            string lblParent = Properties.Resources.lblParent;
            string lblProp = Properties.Resources.lblProp;

            string txtDeclare = Properties.Resources.txtDeclare;
            string txtInit = Properties.Resources.txtInit;
            string txtParent = Properties.Resources.txtParent;
            string txtProp = Properties.Resources.txtProp;
            string txtBinding = Properties.Resources.txtBinding;

            StringBuilder sbInit = new StringBuilder();
            StringBuilder sbParent = new StringBuilder();
            StringBuilder sbProp = new StringBuilder();
            StringBuilder sbDeclare = new StringBuilder();
            StringBuilder sbtxtBinding = new StringBuilder();

            int lblTop = 26; int lblLeft =7;
            int txtTop = 23; int txtLeft = 104;
            int alltop = 26;
            int tabIndex = 0;

            foreach (DataColumn column in dT.Columns)
            {
                string columnNameCapitalize = UppercaseFirst( column.ColumnName);

                //label
                sbInit.AppendLine(lblInit.Replace("{field}", columnNameCapitalize));
                sbDeclare.AppendLine(lblDeclare.Replace("{field}", columnNameCapitalize));
                sbParent.AppendLine(lblParent.Replace("{field}", columnNameCapitalize));
                sbProp.AppendLine(lblProp.Replace("{field}", columnNameCapitalize).Replace("{left}", lblLeft.ToString()).Replace("{top}", lblTop.ToString()).Replace("{tabindex}", tabIndex.ToString()));

                //textbox
                sbInit.AppendLine(txtInit.Replace("{field}", columnNameCapitalize));
                sbDeclare.AppendLine(txtDeclare.Replace("{field}", columnNameCapitalize));
                sbParent.AppendLine(txtParent.Replace("{field}", columnNameCapitalize));
                sbProp.AppendLine(txtProp.Replace("{field}", columnNameCapitalize).Replace("{left}", txtLeft.ToString()).Replace("{top}", txtTop.ToString()).Replace("{tabindex}", tabIndex.ToString()));
                sbtxtBinding.AppendLine(txtBinding.Replace("{field}", columnNameCapitalize).Replace("{fieldL}", column.ColumnName.ToLower()));

                lblTop += 38;
                txtTop += 38;
                alltop += 38;
                tabIndex++;

                if (alltop >= 256) //226
                {
                    lblLeft += 279; lblTop = 26;
                    txtLeft += 253; txtTop = 23;
                    alltop = 0;
                }
            }


            string des = Properties.Resources.Form2_designer_cs;
            des = des.Replace("{entity}", entity);
            des = des.Replace("{groupCTLS}", sbParent.ToString());
            des = des.Replace("{lblINIT}", sbInit.ToString());
            des = des.Replace("{lblPROPS}", sbProp.ToString());
            des = des.Replace("{lblDECLARE}", sbDeclare.ToString());
            


            File.WriteAllText(dir + "frm" + entity + ".designer.cs", des);


            //FORM CODE + RES
            string code = Properties.Resources.Form2_cs;

            code = code.Replace("{entity}", entity)
                .Replace("{entityL}", entity.ToLower())
                .Replace("{PK}", UppercaseFirst(PK))
                .Replace("{PKL}", PK.ToLower())
                .Replace("{bindcontrols}", sbtxtBinding.ToString());

            File.WriteAllText(dir + "frm" + entity + ".cs", code);
            File.WriteAllText(dir + "frm" + entity + ".resx", Properties.Resources.Form2_resx);


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
                .Replace("{outputGetList}", outputGetList).Replace("{outputUPDATE}", outputUPDATE).Replace("{outputeDELETE}", outputeDELETE).Replace("{titleField}",columnNamesArray[0]);


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

        private static string GenerateModelFromDataTable(DataTable dataTable, string className, bool isWinForms)
        {
            StringBuilder sb = new StringBuilder();

            //dataTable.WriteXml(Application.StartupPath + "\\wr" + className + ".xml");
            //dataTable.WriteXmlSchema(Application.StartupPath + "\\wrSC" + className + ".xml");

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace Models.Base");
            sb.AppendLine("{");
            sb.AppendLine(string.Format("    /// <summary>\r\n    /// Base class for {0}.  Do not make changes to this class,\r\n    /// instead, put additional code in the {0} class\r\n    /// </summary>", className));
            
            className += "Base";

            sb.AppendLine(string.Format("    public class {0}{1}", className, isWinForms ? " : ModelBase" : ""));
            sb.AppendLine("    {");

            foreach (DataColumn column in dataTable.Columns)
            {
                string columnName = column.ColumnName.ToLower().Replace(" ", "_");
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

        private void chkWinForms_CheckedChanged(object sender, EventArgs e)
        {
            chkWinFormsDevExpress.Enabled = chkWinForms.Checked;

            if (!chkWinForms.Checked)
                chkWinFormsDevExpress.Checked = false;
        }



    }
}
