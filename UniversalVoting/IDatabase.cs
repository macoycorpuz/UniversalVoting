using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EXAM_02_Corpuz
{
    interface IDatabase
    {
        bool HasConnectionError { get; }
        string ConnectionError { get; }

        DataTable Data { get; }

        void ExecuteCommand(string command);

        void ExecuteStoredProcedure(string command);

        void ExecuteStoredProcedure(string command, List<SqlParameter> sqlParam);

        void Dispose();
    }
}
