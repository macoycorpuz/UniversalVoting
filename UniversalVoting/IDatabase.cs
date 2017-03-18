using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace UniversalVoting
{
    interface IDatabase
    {
        DataTable Data { get; }
   
        bool HasError { get; }

        void ExecuteCommand(string query);

        void ExecuteStoredProc(string query, params object[] args);

        void Dispose();
    }
}
