using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class GetFrameNamesOfJobQuery : MySqlQuery<IEnumerable<string>>
    {
        private readonly string jobNr;

        public GetFrameNamesOfJobQuery(string jobNr)
        {
            this.jobNr = jobNr;
        }

        protected override IEnumerable<string> ProtectedExecution(MySqlConnection connection)
        {
            return new ReadRowsQuery<DbRow>("SELECT Frame FROM `tjobdata` WHERE JobNr = @JobNr AND Frame <> '?' GROUP BY Frame",
                    new MySqlParameter("JobNr", jobNr))
                .Execute(connection)
                .Select(row => row.Values[0]);
        }
    }
}
