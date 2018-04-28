using System.Collections.Generic;
using System.Linq;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies
{
    public class FindProcTurnOrphans : MySqlQuery<IEnumerable<string>>
    {
        protected override IEnumerable<string> ProtectedExecution(MySqlConnection connection)
        {
            const string procRobotCorpsesQuery = "SELECT * FROM `tprocturn` WHERE Name NOT IN (SELECT Name FROM `tjobdata` WHERE What = 'turn') GROUP BY `Name`";

            return new ReadRowsQuery<DbProcTurnRow>(procRobotCorpsesQuery)
                .Execute(connection)
                .Select(item => item.Name);
        }
    }
}
