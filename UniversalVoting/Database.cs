using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;

namespace UniversalVoting
{
    class Database : IDatabase, IDisposable
    {

        #region Database Configurations

        private string dataSource = ConfigurationManager.AppSettings["DataSource"];
        private string initialCatalog = ConfigurationManager.AppSettings["InitialCatalog"];
        private bool integratedSecurity = bool.Parse(ConfigurationManager.AppSettings["IntegratedSecurity"]);
        private string userID = ConfigurationManager.AppSettings["UserID"];
        private string password = ConfigurationManager.AppSettings["Password"];

        #endregion

        #region Attributes

        private SqlConnection sqlCon = null;
        private SqlConnectionStringBuilder sqlConString = null;
        private SqlCommand sqlCmd;
        private SqlDataAdapter sqlAdpt;
        private DataTable data;
        private bool hasError;

        /// <summary>
        /// Gets Datatable
        /// </summary>
        public DataTable Data
        {
            get { return data; }
            private set { data = value; }
        }

        public bool HasError
        {
            get { return hasError; }
            private set { hasError = value; }
        }

        #endregion

        #region Constructors

        public Database()
        {
            this.Process();
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose all sql connection
        /// </summary>
        public void Dispose()
        {
            this.ConnectionClose();
        }

        #endregion

        #region Process

        public void Process()
        {
            this.ConnectionOpen();
            this.ConnectionClose();
        }

        /// <summary>
        /// Executes a Query
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommand(string query)
        {
            try
            {
                ConnectionOpen();
                using (sqlCmd = new SqlCommand(query, sqlCon))
                using (sqlAdpt = new SqlDataAdapter(sqlCmd))
                {
                    data = new DataTable();
                    sqlAdpt.Fill(data);
                }
                ConnectionClose();
                this.HasError = false;
            }
            catch (Exception ex)
            {
                this.HasError = true;
                MessageBox.Show("Unable to Execute Command!!\n\nError: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// Executes Stored Procedures
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteStoredProc(string query, params object[] args)
        {
            try
            {
                ConnectionOpen();
                using (sqlCmd = new SqlCommand(query, sqlCon))
                using (sqlAdpt = new SqlDataAdapter(sqlCmd))
                {
                    //Set Command Type to Stored Proc
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    //Construct Parameters
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] is string)
                        {
                            var sqlparam = new SqlParameter();
                            sqlparam.ParameterName = (string)args[i];
                            sqlparam.Value = args[++i];
                            sqlCmd.Parameters.Add(sqlparam);
                        }
                        else if (args[i] is SqlParameter)
                        {
                            sqlCmd.Parameters.Add((SqlParameter)args[i]);
                        }
                        else
                        {
                            throw new ArgumentException("Unknown sql parameter type");
                        }
                    }

                    //Insert data
                    data = new DataTable();
                    sqlAdpt.Fill(data);
                }
                ConnectionClose();
                this.HasError = false;
            }
            catch (Exception ex)
            {
                this.HasError = true;
                MessageBox.Show("Unable to Execute Stored Procedure!!\n\nError: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        #endregion

        #region Connection

        private void ConnectionOpen()
        {
            this.sqlCon = new SqlConnection();

            try
            {
                this.sqlConString = new SqlConnectionStringBuilder();
                this.sqlConString.DataSource = this.dataSource;
                this.sqlConString.InitialCatalog = this.initialCatalog;
                this.sqlConString.IntegratedSecurity = this.integratedSecurity;

                if (this.integratedSecurity == false)
                {
                    this.sqlConString.UserID = this.userID;
                    this.sqlConString.Password = this.password;
                }

                this.sqlCon.ConnectionString = this.sqlConString.ConnectionString;
                this.sqlCon.Open();
                this.HasError = false;
            }
            catch (Exception ex)
            {
                this.HasError = true;
                MessageBox.Show("Unable to connect database!!\n\nError: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void ConnectionClose()
        {
            if (this.sqlCon != null)
            {
                if (this.sqlCon.State == ConnectionState.Open)
                {
                    this.sqlCon.Close();
                }

                this.sqlCon.Dispose();
                this.sqlCon = null;
            }
        }

        #endregion
    }
}
