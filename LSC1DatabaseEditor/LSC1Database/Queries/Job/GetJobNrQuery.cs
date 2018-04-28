using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using System.Linq;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class GetJobNrQuery : MySqlQuery<string>
    {
        private readonly string jobName;

        public GetJobNrQuery(string jobName)
        {
            this.jobName = jobName;
        }

        protected override string ProtectedExecution(MySqlConnection connection)
        {
            return new ReadRowsQuery<DbRow>("SELECT JobNr FROM `tjobname` WHERE Name = @Name",
                    new MySqlParameter("Name", jobName))
                .Execute(connection).First().Values[0];
        }
    }
}
