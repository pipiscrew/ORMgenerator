using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace ORMgenerator
{
    public partial class Form1 : Form
    {
        //string connectionString = "Data Source=localhost;Initial Catalog=sagiko2;User ID=root;Password=password;";

        //DBase x;
        //MySQLClass x;

        private BindingSource bindSource;

        public Form1()
        {
            InitializeComponent();

            this.Text = Application.ProductName + " v" + Application.ProductVersion;

            cmbType.DataSource = Enum.GetValues(typeof(dbType));

            General.LoadDatabase();

            if (General.dbJSON.Count == 0)
            {
                groupBox1.Enabled = btnSave.Enabled = btnDelete.Enabled = false;
            }

            bindSource = new BindingSource();
            bindSource.DataSource = General.dbJSON;
            lst.DataSource = bindSource;
            lst.DisplayMember = "completeName";

            txtAlias.DataBindings.Add(new Binding("Text", this.bindSource, "connectionAlias", false));
            txtC.DataBindings.Add(new Binding("Text", this.bindSource, "connectionString", false));
            cmbType.DataBindings.Add(new Binding("SelectedItem", this.bindSource, "dbType", false));
            //x = new DBase(new MySql.Data.MySqlClient.MySqlConnection(connectionString));
            //x = new MySQLClass(connectionString);
        }


        #region " Form DragDrop "
        private void frm_DragDrop(object sender, DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);



            if (FileList[0].ToLower().EndsWith(".db") || FileList[0].ToLower().EndsWith(".db3") || FileList[0].ToLower().EndsWith(".mdb") || FileList[0].ToLower().EndsWith(".accdb") || FileList[0].ToLower().EndsWith(".xls") || FileList[0].ToLower().EndsWith(".xlsx"))
            {
                groupBox1.Enabled = false;
                BuildConnectionStringFromFile(FileList[0]);
                groupBox1.Enabled = true;
            }

        }

        private void frm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;

        }

        private void BuildConnectionStringFromFile(string filepath)
        {
            string ext = Path.GetExtension(filepath).ToLower();
            Connection x = new Connection();

            switch (ext)
            {
                case ".xls":
                    x.connectionAlias = "excel";
                    x.connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;", filepath);
                    x.dbType = dbType.adonet_for_xls;
                    break;
                case ".mdb":
                    x.connectionAlias = "mdb";
                    x.connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", filepath);
                    x.dbType = dbType.adonet_for_mdb;
                    break;
                case ".xlsx":
                    x.connectionAlias = "xlsx";
                    x.connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;", filepath);
                    x.dbType = dbType.adonet_for_xlsx;
                    break;
                case ".accdb":
                    x.connectionAlias = "accdb";
                    x.connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}", filepath);
                    x.dbType = dbType.adonet_for_accdb;
                    break;
                case ".db":
                case ".db3":
                    x.connectionAlias = "sqlite";
                    x.connectionString = string.Format("Data Source={0};Version=3;", filepath);
                    x.dbType = dbType.sqlite;
                    break;
                default:
                    break;

            }

            if (x.connectionString != null)
            {
                General.dbJSON.Add(x);
                bindSource.ResetBindings(false);
                groupBox1.Enabled = btnSave.Enabled = btnDelete.Enabled = true;
                //lst.SelectedIndex = lst.Items.Count - 1;
            }
        }

        #endregion

        private void btnNew_Click(object sender, EventArgs e)
        {
            General.dbJSON.Add(new Connection() { connectionAlias = "hi there!" });
            bindSource.ResetBindings(false);
            lst.SelectedIndex = lst.Items.Count - 1;

            groupBox1.Enabled = btnSave.Enabled = btnDelete.Enabled = true;

            txtAlias.Focus();
            //dg.DataSource = x.GetDATATABLE("select * from leagueseasons");

            //dg.DataSource = x.GetDATATABLE("select * from leagueseasons");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (General.dbJSON.Count == 0)
            {
                General.Mes("Please use first the 'new' button");
                return;
            }
            General.SaveDatabase();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bindSource.Current == null)
            {
                General.Mes("Please select an item");
                return;
            }

            General.dbJSON.Remove((Connection)bindSource.Current);
            bindSource.ResetBindings(false);
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!groupBox1.Enabled || bindSource == null || bindSource.Current == null) // || (bindSource.Current as Connection).connectionString != null)
            //    return;

            if (bindSource == null)
                return;

            if ((bindSource.Current as Connection).dbType != dbType.nosql)
                return;

            string x = string.Empty;
            switch ((dbType)cmbType.SelectedItem)
            {
                case dbType.adonet_for_xls:
                    x = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=1111111111111;Extended Properties=Excel 8.0;";
                    break;
                case dbType.adonet_for_mdb:
                    x = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=1111111111111;Jet OLEDB:Database Password=IF_IS_ANY";
                    break;
                case dbType.adonet_for_xlsx:
                    x = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=1111111111111;Extended Properties=Excel 12.0;";
                    break;
                case dbType.adonet_for_accdb:
                    x = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=1111111111111;Jet OLEDB:Database Password=IF_IS_ANY";
                    break;
                case dbType.mysql:
                    x = "Data Source=localhost;Initial Catalog=DB_NAME;User ID=USER;Password=PASSWORD;CharSet=utf8;Allow Zero Datetime=True";
                    break;
                case dbType.oracle:
                    x = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=111111)(PORT=1111)))(CONNECT_DATA=(SERVICE_NAME=11111)));User Id=11111;Password=1111;";
                    break;
                case dbType.sqlite:
                    x = "Data Source=1111111111111;Version=3;Password=IF_IS_ANY";
                    break;
                case dbType.sqlserver:
                    //Server=.\sqlexpress; Database=1111111111; User Id=sa; Password=12; Trusted_Connection=False;
                    x = "Data Source=127.0.0.1;Initial Catalog=DB_NAME;User ID=USER;Password=PASSWORD;";
                    break;
                case dbType.sqlserver_integrated:
                    x = "Data Source=127.0.0.1;Initial Catalog=DB_NAME;Integrated Security=True;";
                    break;
                default:
                    break;

            }

            (bindSource.Current as Connection).connectionString = x;
            SendKeys.Send("{TAB}{TAB}");

        }


        private void btnNext_Click(object sender, EventArgs e)
        {
            if (bindSource.Current == null)
            {
                General.Mes("Please select an item");
                return;
            }

            /////////////////////////////////////////////////////////////////////////////
            /// -- HERE ESTABLISH THE CONNECTION - WHEN SUCCESS PROCEED TO NEXT STEP
            /////////////////////////////////////////////////////////////////////////////

            string x = string.Empty;
            switch ((dbType)cmbType.SelectedItem)
            {
                case dbType.adonet_for_xls:
                case dbType.adonet_for_mdb:
                case dbType.adonet_for_xlsx:
                case dbType.adonet_for_accdb:
                    ConnectMS((bindSource.Current as Connection).connectionString);
                    //General.db = new DBASEWrapper(new OleDbConnection((bindSource.Current as Connection).connectionString));
                    break;
                case dbType.mysql:
                    ConnectMYSQL((bindSource.Current as Connection).connectionString);
                    //General.db = new DBASEWrapper(new MySqlConnection((bindSource.Current as Connection).connectionString));
                    break;
                case dbType.oracle:
                    ConnectORACLE((bindSource.Current as Connection).connectionString);
                    //x = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=111111)(PORT=1111)))(CONNECT_DATA=(SERVICE_NAME=11111)));User Id=11111;Password=1111;";
                    break;
                case dbType.sqlite:
                    ConnectSQLITE((bindSource.Current as Connection).connectionString);
                    //General.db = new DBASEWrapper(new SQLiteConnection((bindSource.Current as Connection).connectionString));
                    break;
                case dbType.sqlserver:
                case dbType.sqlserver_integrated:
                    ConnectSQLSERVER((bindSource.Current as Connection).connectionString);
                    //General.db = new DBASEWrapper(new SqlConnection((cmbType.SelectedItem as Connection).connectionString));
                    break;
                default:
                    break;

            }

            if (General.db == null || !General.db.IsConnected)
            {
                General.Mes("cannot connect to the database",MessageBoxIcon.Exclamation);
                return;
            }

            frmTables frm = new frmTables((dbType)cmbType.SelectedItem);
            frm.ShowDialog();
        }


        private static void ConnectMS(string connectionString)
        {
            General.db = new DBASEWrapper(new OleDbConnection(connectionString));
        }

        private static void ConnectMYSQL(string connectionString)
        {
            General.db = new DBASEWrapper(new MySqlConnection(connectionString));
        }

        private static void ConnectSQLITE(string connectionString)
        {
            General.db = new DBASEWrapper(new SQLiteConnection(connectionString));
        }

        private static void ConnectSQLSERVER(string connectionString)
        {
            General.db = new DBASEWrapper(new SqlConnection(connectionString));
        }

        private static void ConnectORACLE(string connectionString)
        {   // tested & working - add reference frm462 - https://www.nuget.org/packages/Oracle.ManagedDataAccess/21.10.0
            //General.db = new DBASEWrapper(new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString));
        }
    }
}