using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSC1DatabaseEditor.LSC1Database
{
    //TODO: Refactor: Corpses to Orphans
    /// <summary>
    /// Handles the following kinds of inconsistencies:
    /// 1. Orphans
    /// Orphans are procRobot, tpos or other elements with name that are not 
    /// referenced in any job, there are the following kinds of orphans
    /// 1. position-orphans: elements in tpos that are not referenced in any job
    /// 2. laser-orphans and robot-orphans: elements in tproclaser and tprocrobot that are not referenced in any job
    ///    They always appear together because in every robot step there also is a laser step
    /// 4. frame-orphans: elements in tfram that are not referenced in any job, twt TODO: implement in view
    /// 5. plc-orphans: elements in tprocplc that are not referenced in any job TODO: implement
    /// 5. pulse-orphans: elements in tprocpulse that are not referenced in any job TODO: implement
    /// 6. turn-orphans: elements in tprocturn that are not referenced in any job TODO: implement
    /// </summary>
    public class LSC1InconsistencyHandler
    {
        private string connectionString;

        public LSC1InconsistencyHandler(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //TODO: Test        
        public async Task<IEnumerable<string>> FindPosOrphansAsync()
        {
            //Corpses in pos
            string posCorpsesQuery = "SELECT * FROM `tpos` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";

            return (await new LSC1MySqlQueryExecuter(connectionString)
                .ExecuteAsync(new ReadRowsQuery<DbPosRow>(posCorpsesQuery)))
                .Select(item => item.Name);
        }

        //TODO: test
        public async Task<IEnumerable<string>> FindProcLaserOrphansAsync()
        {
            string procLaserCorpsesQuery = "SELECT * FROM `tproclaserdata` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";

            return (await new LSC1MySqlQueryExecuter(connectionString)
                .ExecuteAsync(new ReadRowsQuery<DbProcLaserDataRow>(procLaserCorpsesQuery)))
                .Select(item => item.Name);
        }

        //TODO: test
        public async Task<IEnumerable<string>> FindProcRobotOrphansAsync()
        {
            string procRobotCorpsesQuery = "SELECT * FROM `tprocrobot` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";

            return (await new LSC1MySqlQueryExecuter(connectionString)
                .ExecuteAsync(new ReadRowsQuery<DbProcRobotRow>(procRobotCorpsesQuery)))
                .Select(item => item.Name);
        }

        //TODO: test
        public async Task<IEnumerable<string>> FindFrameOrphansAsync()
        {
            string procRobotCorpsesQuery = "SELECT* FROM `tframe` WHERE NOT EXISTS (SELECT NULL FROM (SELECT FrameT1 FROM `twt` UNION SELECT FrameT2 FROM `twt` UNION SELECT Frame FROM `tjobdata`) As t WHERE tframe.Name = t.FrameT1)";

            return (await new LSC1MySqlQueryExecuter(connectionString)
                .ExecuteAsync(new ReadRowsQuery<DbProcRobotRow>(procRobotCorpsesQuery)))
                .Select(item => item.Name);
        }


        //TODO: test
        public async Task<IEnumerable<string>> FindJobOrphansAsync()
        {
            string jobCorpsesQuery = "SELECT DISTINCT (JobNr) FROM `tjobdata` WHERE JobNr NOT IN (SELECT JobNr FROM `tjobname`)";

            return (await new LSC1MySqlQueryExecuter(connectionString)
                .ExecuteAsync(new ReadRowsQuery<DbJobDataRow>(jobCorpsesQuery)))
                .Select(item => item.JobNr);
        }

    }
}
