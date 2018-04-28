using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    /// <summary>
    /// Goal of this class is to search for all entries in the given jobdata 
    /// that are a pos or proc and return their names.
    /// </summary>
    public class GetNamesOfJobQuery : MySqlQuery<IEnumerable<string>>
    {
        private readonly string jobNr;
        private readonly string what;

        /// <summary>
        /// Goal of this class is to search for all entries in the given jobdata 
        /// that are a pos or proc and return their names.
        /// </summary>
        /// <param name="jobNr">JobNr of job in which to search</param>
        /// <param name="what">type of the entry (pos or proc)</param>
        public GetNamesOfJobQuery(string jobNr, string what)
        {
            this.jobNr = jobNr;
            this.what = what;
        }

        protected override IEnumerable<string> ProtectedExecution(MySqlConnection connection)
        {
            return new ReadRowsQuery<DbRow>("SELECT Name FROM tjobdata WHERE JobNr = @JobNr AND What = @What GROUP BY Name",
                    new MySqlParameter("JobNr", jobNr),
                    new MySqlParameter("What", what))
                        .Execute(connection)
                        .Select(item => item.Values[0]);
        }
    }
}