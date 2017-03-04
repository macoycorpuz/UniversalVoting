using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalVoting
{
    class Database : IDatabase, IDisposable
    {
        #region Attributes
        private string dataSource;
        private string initialCatalog;
        private bool integratedSecurity;
        private string userID;
        private string password;

        private SqlConnection sqlCon;
        private SqlConnectionStringBuilder sqlConString;
        private SqlCommand sqlCmd;
        private SqlDataAdapter sqlAdpt;

        private bool hasConnectionError;
        private string connectionError;
        private DataTable data;

        /// <summary>
        /// Returns Connection Error (bool)
        /// </summary>
        public bool HasConnectionError
        {
            get { return hasConnectionError; }
            private set { hasConnectionError = value; }
        }
        
        /// <summary>
        /// Return Connection Error (Message)
        /// </summary>
        public string ConnectionError
        {
            get { return connectionError; }
            private set { connectionError = value; }
        }

        public DataTable Data
        {
            get { return data; }
            private set { data = value; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initialize Database (Windows Authentication)
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="InitialCatalog"></param>
        /// <param name="IntegratedSecurity"></param>
        public Database(string DataSource, string InitialCatalog, bool IntegratedSecurity)
        {
            this.dataSource = DataSource;
            this.initialCatalog = InitialCatalog;
            this.integratedSecurity = IntegratedSecurity;
            
            this.HasConnectionError = true;
            this.ConnectionError = "";

            this.Process();
        }

        /// <summary>
        /// Initialize Database (SQL Server Authentication)
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="InitialCatalog"></param>
        /// <param name="IntegratedSecurity"></param>
        /// <param name="UserID"></param>
        /// <param name="Password"></param>
        public Database(string DataSource, string InitialCatalog, bool IntegratedSecurity, string UserID, string Password)
        {
            this.dataSource = DataSource;
            this.initialCatalog = InitialCatalog;
            this.integratedSecurity = IntegratedSecurity;
            this.userID = UserID;
            this.password = Password;
            
            this.HasConnectionError = true;
            this.ConnectionError = "";

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
            this.CommandClose();
        }

        #endregion

        /// <summary>
        /// All process and sql executions.
        /// </summary>
        #region Process

        public void Process()
        {
            this.ConnectionOpen();
            this.CommandOpen();
            this.CommandClose();
            this.ConnectionClose();
        }

        /// <summary>
        /// This executes a sql command.
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommand(string command)
        {
            ConnectionOpen();
            CommandOpen();
            sqlCmd.CommandText = command;
            using (sqlAdpt = new SqlDataAdapter(sqlCmd))
            {
                data = new DataTable();
                sqlAdpt.Fill(data);
            }
            CommandClose();
            ConnectionClose();
        }

        /// <summary>
        /// This executes a stored procedure without parameters.
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteStoredProcedure(string command)
        {
            ConnectionOpen();
            CommandOpen();
            sqlCmd.CommandText = command;
            using (sqlAdpt = new SqlDataAdapter(sqlCmd))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                data = new DataTable();
                sqlAdpt.Fill(data);
            }
            CommandClose();
            ConnectionClose();
        }

        /// <summary>
        /// This executes a stored procedure with parameters.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="sqlParam"></param>
        public void ExecuteStoredProcedure(string command, List<SqlParameter> sqlParam)
        {
            ConnectionOpen();
            CommandOpen();
            sqlCmd.CommandText = command;
            using (sqlAdpt = new SqlDataAdapter(sqlCmd))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddRange(sqlParam.ToArray());
                data = new DataTable();
                sqlAdpt.Fill(data);
            }
            CommandClose();
            ConnectionClose();
        }

        #endregion

        /// <summary>
        /// Checking all connection problems.
        /// </summary>
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

                this.HasConnectionError = false;
            }
            catch (Exception ex)
            {
                this.ConnectionError = ex.Message;
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

        private void CommandOpen()
        {
            this.sqlCmd = new SqlCommand();

            try
            {
                this.sqlCmd.Connection.Open();
                this.HasConnectionError = false;
            }
            catch (Exception ex)
            {
                this.ConnectionError = ex.Message;
            }
        }
        private void CommandClose()
        {
            if (this.sqlCmd != null)
            {
                if (this.sqlCmd.Connection.State == ConnectionState.Open)
                {
                    this.sqlCmd.Connection.Close();
                }

                this.sqlCmd.Connection.Dispose();
                this.sqlCmd = null;
            }
        }

        #endregion
    }
}
