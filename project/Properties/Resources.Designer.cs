﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ORMgenerator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ORMgenerator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.Generic;
        ///using System.Data;
        ///using System.Data.Common;
        ///
        ///public class DBASEWrapper : IDisposable
        ///{
        ///    private IDbConnection objConn;
        ///
        ///    public DBASEWrapper(IDbConnection connection)
        ///    {
        ///        try
        ///        {
        ///            this.objConn = connection;
        ///            this.objConn.Open();
        ///        }
        ///        catch (Exception ex)
        ///        {
        ///            General.Mes(ex.Message + &quot;\r\n\r\n&quot; + (ex.InnerException == null ? &quot;&quot; : ex.InnerException.Message), System.Wind [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string constDBASEWrapper {
            get {
                return ResourceManager.GetString("constDBASEWrapper", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System.Windows.Forms;
        ///
        ///internal static class General
        ///{
        ///    internal static DBASEWrapper db;
        ///
        ///    internal static DialogResult Mes(string descr, MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons butt = MessageBoxButtons.OK)
        ///    {
        ///        if (descr.Length &gt; 0)
        ///            return MessageBox.Show(descr, Application.ProductName, butt, icon);
        ///        else
        ///            return DialogResult.OK;
        ///    }
        ///
        ///}
        ///
        ///
        ///.
        /// </summary>
        internal static string constGeneral {
            get {
                return ResourceManager.GetString("constGeneral", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System.Collections.Generic;
        ///
        ///namespace Services
        ///{
        ///    public interface ICRUDService&lt;T&gt; where T : class
        ///    {
        ///        bool Insert(T obj);
        ///        long InsertReturnId(T obj);
        ///        T Get(long id);
        ///        List&lt;T&gt; GetList();
        ///        bool Update(T obj);
        ///        bool Delete(long id);
        ///    }
        ///}.
        /// </summary>
        internal static string constICRUDService {
            get {
                return ResourceManager.GetString("constICRUDService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using Dapper;
        ///using System.Collections.Generic;
        ///using System.Linq;
        ///using Models;
        ///
        ///namespace Services
        ///{
        ///    public class {className}Service : ICRUDService&lt;{className}&gt;
        ///    {
        ///        public bool Insert({className} obj)
        ///        {
        ///            var command = @&quot;{outputINSERT}&quot;;
        ///            var parms = new { {insertOBJvalues} };
        ///            var result =  General.db.GetConnection().Execute(command, parms);
        ///            return result &gt; 0;
        ///        }
        ///
        ///        public long InsertReturnId({className} obj)        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DapperServiceTemplate {
            get {
                return ResourceManager.GetString("DapperServiceTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System.Collections.Generic;
        ///using System.Linq;
        ///using Models;
        ///
        ///namespace Services
        ///{
        ///    public class {className}Service : ICRUDService&lt;{className}&gt;
        ///    {
        ///        public bool Insert({className} obj)
        ///        {
        ///            var command = @&quot;{outputINSERT}&quot;;
        ///            var parms = new { {insertOBJvalues} };
        ///            var result =  General.db.ExecuteModel(command, parms);
        ///            return result &gt; 0;
        ///        }
        ///
        ///        public long InsertReturnId({className} obj)
        ///        {
        ///            va [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DBASEWrapperReflectionServiceTemplate {
            get {
                return ResourceManager.GetString("DBASEWrapperReflectionServiceTemplate", resourceCulture);
            }
        }
    }
}
