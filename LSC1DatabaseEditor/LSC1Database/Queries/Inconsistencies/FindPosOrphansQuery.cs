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
    public class FindPosOrphansQuery : MySqlQuery<IEnumerable<string>>
    {
        protected override IEnumerable<string> ProtectedExecution(MySqlConnection connection)
        {      //Corpses in pos
            const string posCorpsesQuery = "SELECT * FROM `tpos` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";

            return new ReadRowsQuery<DbPosRow>(posCorpsesQuery)
                .Execute(connection)
                .Select(item => item.Name);
        }
    }
}
