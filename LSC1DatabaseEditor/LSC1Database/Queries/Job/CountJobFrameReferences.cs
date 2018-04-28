using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using System.Linq;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class CountJobFrameReferencesQuery : MySqlQuery<int>
    {
        private readonly string jobNr;
        private readonly string frameName;

        public CountJobFrameReferencesQuery(string jobNr, string frameName)
        {
            this.jobNr = jobNr;
            this.frameName = frameName;
        }

        protected override int ProtectedExecution(MySqlConnection connection)
        {
            const string query = "SELECT COUNT(*) as `count` FROM (SELECT 0 AS `dummy` FROM `tjobdata` " +
                                 "WHERE JobNr <> @JobNr AND Frame = @Frame " +
                                 "GROUP BY JobNr) sq";

            return int.Parse(new ReadRowsQuery<DbRow>(query,
                                    new MySqlParameter("JobNr", jobNr),
                                    new MySqlParameter("Frame", frameName))
                                    .Execute(connection).First().Values[0]);
        }
    }
}
