using System.Collections.Generic;
using System.Linq;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database
{
    //TODO: Methoden auslagern und zu Querys machen.
    public class LSC1DbFunctionCollection
    {
        private static MySqlConnection connection;

        public LSC1DbFunctionCollection(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
        }

        public List<DbJobNameRow> FindJobsThatUseName(string name)
        {
            string jobNrsWithProcNameQuery = "SELECT (JobNr) FROM `tjobdata` WHERE Name = '" + name + "' GROUP BY `JobNr`";
            string selectJobNameQuery = "SELECT * FROM `tjobname` WHERE JobNr IN (" + jobNrsWithProcNameQuery + ")";

            return new ReadRowsQuery<DbJobNameRow>(selectJobNameQuery).Execute(connection).ToList();
        }
        
        
        public List<DbJobNameRow> FindJobsWithFrame(string frame)
        {
            string jobNrsWithProcNameQuery = "SELECT (JobNr) FROM `tjobdata` WHERE Frame = '" + frame + "' GROUP BY `JobNr`";
            string selectJobNameQuery = "SELECT * FROM `tjobname` WHERE JobNr IN (" + jobNrsWithProcNameQuery + ")";

            return new ReadRowsQuery<DbJobNameRow>(selectJobNameQuery).Execute(connection).ToList();
        }
    }
}
