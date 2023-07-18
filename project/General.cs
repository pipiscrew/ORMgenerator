using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;


public static class General
{
    internal static DBASEWrapper db;

    internal static List<Connection> dbJSON;
    private static string filenameDB = Application.StartupPath + "\\dbase.db";

    internal static DialogResult Mes(string descr, MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons butt = MessageBoxButtons.OK)
    {
        if (descr.Length > 0)
            return MessageBox.Show(descr, Application.ProductName, butt, icon);
        else
            return DialogResult.OK;
    }

    internal static void LoadDatabase()
    {
        General.dbJSON = new List<Connection>();

        if (!File.Exists(filenameDB))
            return;

        try
        {
            General.dbJSON.AddRange(JsonSerializer.Deserialize<List<Connection>>(File.ReadAllText(filenameDB, Encoding.UTF8)));
        }
        catch (Exception x)
        {
            General.Mes(x.Message, MessageBoxIcon.Error);
            General.dbJSON = new List<Connection>();
        }
    }

    internal static void SaveDatabase()
    {
        try
        {
            File.WriteAllText(filenameDB, General.dbJSON.Serialize(), Encoding.UTF8);
        }
        catch (Exception x)
        {
            General.Mes(x.Message, MessageBoxIcon.Error);
            General.dbJSON = new List<Connection>();
        }
    }

}

public enum dbType
{
    nosql = 0,
    sqlite = 1,
    mysql = 2,
    sqlserver = 3,
    sqlserver_integrated = 4,
    oracle = 5,
    adonet_for_xls = 6,
    adonet_for_mdb = 7,
    adonet_for_xlsx = 8,
    adonet_for_accdb = 9,
}

public class Connection
{
    public dbType dbType { get; set; }
    public string connectionString { get; set; }
    public string connectionAlias { get; set; }

    [IgnoreDataMember]
    public string completeName
    {
        get
        {
            return string.Format("[{0}] {1}", dbType.ToString(), connectionAlias);
        }
    }
}
