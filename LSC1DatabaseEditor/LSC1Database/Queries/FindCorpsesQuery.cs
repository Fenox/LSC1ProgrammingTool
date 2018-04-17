using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseEditor.LSC1Database.Queries
{
    //TODO: test
    public class FindCorpsesQuery : IMySqlQuery<IEnumerable<string>>
    {
        public IEnumerable<string> Execute(MySqlConnection connection)
        {
            var executer = new LSC1MySqlQueryExecuter(connection);

            string procLaserCorpsesQuery = "SELECT * FROM `tproclaserdata` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";

            var procLaserCorpses = executer
                .Execute(new ReadRowsQuery<DbProcLaserDataRow>(procLaserCorpsesQuery));

            //Corpses in procRobot
            string procRobotCorpsesQuery = "SELECT * FROM `tprocrobot` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";
            
            var procRobotCorpses = executer
                .Execute(new ReadRowsQuery<DbProcRobotRow>(procRobotCorpsesQuery));


            var corpseNames = procLaserCorpses.Select(item => item.Name);
            corpseNames.Concat(procRobotCorpses.Select(item => item.Name));
            
            return corpseNames;
        }
    }
}
