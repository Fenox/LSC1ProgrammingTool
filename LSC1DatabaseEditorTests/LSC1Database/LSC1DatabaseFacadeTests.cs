using System.Linq;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies;
using LSC1DatabaseEditor.LSC1Database.Queries.Job;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditorTests.LSC1Database
{
    [TestClass()]
    public class LSC1DatabaseFacadeTests
    {
        private static readonly MySqlConnectionStringBuilder ConnStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = "127.0.0.1",
            Database = "lsc1test",
            UserID = "root",
            Password = ""
        };

        private static readonly MySqlConnection Connection = new MySqlConnection(ConnStringBuilder.ConnectionString);


        [TestMethod]
        public void GetJobsTest()
        {
            var result = new GetJobsQuery().Execute(Connection);
            Assert.AreEqual(58, result.Count());
        }

        [TestMethod]
        public void FindProcCorpsesTest()
        {
            var result = new FindProcLaserOrphansQuery().Execute(Connection);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void FindPosCorpsesTest()
        {
            var result = new FindPosOrphansQuery().Execute(Connection);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void FindJobCorpsesTest()
        {
            var result = new FindJobOrphansQuery().Execute(Connection);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetFreeJobNumberTest()
        {
            string result = new GetFreeJobNumberQuery().Execute(Connection);
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void DeleteJobTest()
        {
            //Insert new job to be deleted
            new InsertQuery(new DbJobNameRow("1000", "deleteJobTest1")).Execute(Connection);

            //Insert job positions
            new InsertQuery(new DbPosRow("deleteJobTestPos1", "point", "1", "2", "2", "2", "2", "2", "False")).Execute(Connection);

            //Insert proc robot + laser data
            new InsertQuery(new DbProcRobotRow("deleteJobTestProc1", "1", "point", "lin", "2", "2", "2", "2", "2", "2", "2", "2", "2", "14", "0.3", "0.3")).Execute(Connection);
            new InsertQuery(new DbProcLaserDataRow("deleteJobTestProc1", "1", "no", "0", "2")).Execute(Connection);

            //Insert job frame
            new InsertQuery(new DbFrameRow("deleteJobTestFrame", "Job", "0", "0", "0", "0", "0", "0")).Execute(Connection);

            //insert jobdata
            new InsertQuery(new DbJobDataRow("1000", "1", "plc", "?", "?", "?", "?", "?", "?", "?", "?")).Execute(Connection);
            new InsertQuery(new DbJobDataRow("1000", "2", "plc", "laser", "?", "?", "?", "?", "?", "?", "15")).Execute(Connection);
            new InsertQuery(new DbJobDataRow("1000", "3", "robot", "pos", "deleteJobTestPos1", "mDefault", "deleteJobTestFrame", "ttoolJobDeleteTest", "ptp", "yes", "?")).Execute(Connection);
            new InsertQuery(new DbJobDataRow("1000", "4", "robot", "proc", "deleteJobTestProc1", "?", "deleteJobTestFrame", "ttoolJobDeleteTest", "?", "yes", "?")).Execute(Connection);
            new InsertQuery(new DbJobDataRow("1000", "5", "plc", "turn", "LagerPosL", "mDefault", "?", "noTool", "ptp",
                "yes", "?")).Execute(Connection);

            new DeleteJobQuery("deleteJobTest1")
                .Execute(Connection);


            Assert.AreEqual(false, new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tjobname WHERE Name = 'deleteJobTest1'").Execute(Connection).Any());
            Assert.AreEqual(false, new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tpos WHERE Name = 'deleteJobTestPos1'").Execute(Connection).Any());
            Assert.AreEqual(false, new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tprocrobot WHERE Name = 'deleteJobTestProc1'").Execute(Connection).Any());
            Assert.AreEqual(false, new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tproclaserdata WHERE Name = 'deleteJobTestProc1'").Execute(Connection).Any());

            Assert.AreEqual(false,  new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tframe WHERE Name = 'deleteJobTestFrame'").Execute(Connection).Any());
            Assert.AreEqual(false,  new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tjobdata WHERE JobNr = '1000'").Execute(Connection).Any());
            Assert.IsTrue(new CountQuery("SELECT COUNT(*) FROM tprocturn WHERE Name = 'LagerPosL'").Execute(Connection) > 0);
            

            new InsertQuery(new DbJobNameRow("1000", "deleteJobTest1")).Execute(Connection);
            var numPosEntriesBefore =  new CountQuery("SELECT COUNT(*) FROM tpos").Execute(Connection);
            var numProcRobotEntriesBefore =  new CountQuery("SELECT COUNT(*) FROM tprocrobot").Execute(Connection);
            var numProcLaserEntriesBefore =  new CountQuery("SELECT COUNT(*) FROM tproclaserdata").Execute(Connection);
            var numProcPlcEntriesBefore =  new CountQuery("SELECT COUNT(*) FROM tprocplc").Execute(Connection);
            var numProctFrameEntriesBefore =  new CountQuery("SELECT COUNT(*) FROM tframe").Execute(Connection);

            new InsertQuery(new DbJobDataRow("1000", "1", "plc", "?", "?", "?", "?", "?", "?", "?", "?"))
                .Execute(Connection);
            new InsertQuery(new DbJobDataRow("1000", "2", "plc", "laser", "?", "?", "?", "?", "?", "?", "15"))
                .Execute(Connection);
            new InsertQuery(new DbJobDataRow("1000", "3", "robot", "pos", "pKlappeHeft", "mDefault", "fLaHu", "tTool+4", "ptp", "yes", "?"))
                .Execute(Connection);
            new InsertQuery(new DbJobDataRow("1000", "4", "robot", "proc", "LagerWellePruef", "?", "fLaHu", "tTool+4", "?", "yes", "?"))
                .Execute(Connection);
            new InsertQuery(new DbJobDataRow("1000", "5", "plc", "turn", "LagerPosL", "mDefault", "?", "noTool", "ptp", "yes", "?"))
                .Execute(Connection);

            new DeleteJobQuery("deleteJobTest1")
                .Execute(Connection);

            Assert.AreEqual(false, new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tjobname WHERE Name = 'deleteJobTest1'").Execute(Connection).Any());

            Assert.AreEqual(numPosEntriesBefore, new CountQuery("SELECT COUNT(*) FROM tpos").Execute(Connection));
            Assert.AreEqual(numProcRobotEntriesBefore, new CountQuery("SELECT COUNT(*) FROM tprocrobot").Execute(Connection));
            Assert.AreEqual(numProcLaserEntriesBefore, new CountQuery("SELECT COUNT(*) FROM tproclaserdata").Execute(Connection));
            Assert.AreEqual(numProcPlcEntriesBefore, new CountQuery("SELECT COUNT(*) FROM tprocplc").Execute(Connection));
            Assert.AreEqual(numProctFrameEntriesBefore, new CountQuery("SELECT COUNT(*) FROM tframe").Execute(Connection));
        }
    }
}