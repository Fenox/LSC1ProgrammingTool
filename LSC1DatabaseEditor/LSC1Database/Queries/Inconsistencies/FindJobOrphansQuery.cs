using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies
{
    public class FindJobOrphansQuery : MySqlQuery<IEnumerable<string>>
    {
        protected override IEnumerable<string> ProtectedExecution(MySqlConnection connection)
        {
            const string jobCorpsesQuery = "SELECT DISTINCT (JobNr) FROM `tjobdata` WHERE JobNr NOT IN (SELECT JobNr FROM `tjobname`)";

            return new ReadRowsQuery<DbJobDataRow>(jobCorpsesQuery)
                .Execute(connection)
                .Select(item => item.JobNr);
        }
    }
}
