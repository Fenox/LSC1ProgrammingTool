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
    public class FindPulseOrphans : MySqlQuery<IEnumerable<string>>
    {

        protected override IEnumerable<string> ProtectedExecution(MySqlConnection connection)
        {
            const string procRobotCorpsesQuery = "SELECT * FROM `tprocpulse` WHERE Name NOT IN (SELECT Name FROM `tjobdata` WHERE What = 'pulse') GROUP BY `Name`";

            return new ReadRowsQuery<DbProcPulseRow>(procRobotCorpsesQuery)
                .Execute(connection)
                .Select(item => item.Name);
        }
    }
}
