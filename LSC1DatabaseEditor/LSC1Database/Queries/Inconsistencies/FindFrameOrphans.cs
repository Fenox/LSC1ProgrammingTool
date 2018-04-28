using System.Collections.Generic;
using System.Linq;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies
{
    public class FindFrameOrphans : MySqlQuery<IEnumerable<string>>
    {
        protected override IEnumerable<string> ProtectedExecution(MySqlConnection connection)
        {
            const string procRobotCorpsesQuery = "SELECT* FROM `tframe` WHERE NOT EXISTS (SELECT NULL FROM (SELECT FrameT1 FROM `twt` UNION SELECT FrameT2 FROM `twt` UNION SELECT Frame FROM `tjobdata`) As t WHERE tframe.Name = t.FrameT1)";

            return new ReadRowsQuery<DbProcRobotRow>(procRobotCorpsesQuery)
                .Execute(connection)
                .Select(item => item.Name);
        }
    }
}
