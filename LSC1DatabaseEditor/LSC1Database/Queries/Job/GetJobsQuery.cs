using System.Collections.Generic;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class GetJobsQuery : MySqlQuery<IEnumerable<DbJobNameRow>>
    {
        protected override IEnumerable<DbJobNameRow> ProtectedExecution(MySqlConnection connection)
        {
            return new ReadRowsQuery<DbJobNameRow>("SELECT * FROM `tjobname`").Execute(connection);
        }
    }
}
