using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class CountJobDataReferences : MySqlQuery<int>
    {
        private readonly string name;
        private readonly string what;

        public CountJobDataReferences(string name, string what)
        {
            this.name = name;
            this.what = what;
        }


        protected override int ProtectedExecution(MySqlConnection connection)
        {
            const string query = "SELECT COUNT(*) as `count` FROM (SELECT 0 as `dummy` FROM tjobdata WHERE Name = @Name AND What = @What GROUP BY JobNr) sq";

            return new CountQuery(query,
                new MySqlParameter("Name", name),
                new MySqlParameter("What", what)).Execute(connection);
        }
    }
}
