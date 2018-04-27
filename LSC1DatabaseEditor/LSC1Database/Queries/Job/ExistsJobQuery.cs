using System.Linq;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class ExistsJobQuery : MySqlQuery<bool>
    {
        private readonly string jobName;

        public ExistsJobQuery(string jobName)
        {
            this.jobName = jobName;
        }

        protected override bool ProtectedExecution(MySqlConnection connection)
        {
            return new ReadRowsQuery<DbRow>("SELECT * FROM tjobname WHERE Name = @Name",
                    new MySqlParameter("Name", jobName))
                            .Execute(connection).Any();
        }
    }
}
