using System.Linq;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class GetFreeJobNumberQuery : MySqlQuery<string>
    {
        protected override string ProtectedExecution(MySqlConnection connection)
        {
            var jobs = new ReadRowsQuery<DbJobNameRow>("SELECT * FROM `tjobname`")
                .Execute(connection).ToList();
            int jobNr = -1;

            for (var i = 0; i < jobs.Count + 1; i++)
            {
                if (jobs.Exists(j => j.JobNr.Equals(i.ToString()))) continue;

                jobNr = i;
                break;
            }

            return jobNr.ToString();

        }
    }
}
