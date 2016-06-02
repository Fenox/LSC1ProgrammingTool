using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class LSC1DataTable<T> where T : DbRow, new()
    {
        public List<T> LoadRows(LSC1DatabaseConnectionSettings con, string tableName, string sqlWhereClause)
        {
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            return db.ReadRows<T>("SELECT * FROM " + tableName + "WHERE " + sqlWhereClause);
        }
    }
}
