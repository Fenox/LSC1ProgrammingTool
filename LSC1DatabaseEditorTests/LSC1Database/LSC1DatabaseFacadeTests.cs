using Microsoft.VisualStudio.TestTools.UnitTesting;
using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using LSC1DatabaseEditor.LSC1Database;

namespace LSC1DatabaseLibrary.Tests
{
    [TestClass()]
    public class LSC1DatabaseFacadeTests
    {
        private static LSC1InconsistencyHandler inconsistencyFinder = new LSC1InconsistencyHandler(new MySqlConnectionStringBuilder
            {
                Server = "127.0.0.1",
                Database = "lsc1test",
                UserID = "root",
                Password = ""
            }.ConnectionString); 

        [TestInitialize]
        public void Initialize()
        {
            MySqlConnectionStringBuilder connStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = "127.0.0.1",
                Database = "lsc1test",
                UserID = "root",
                Password = ""
            };

            LSC1DatabaseFacade.ConnectionString = connStringBuilder.ConnectionString;
        }

        [TestMethod()]
        public void FindJobsWithFrameTest()
        {
            var result = LSC1DatabaseFacade.FindJobsWithFrame("fWeKl11_1");

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("11", result[0].JobNr);
        }

        [TestMethod()]
        public void GetJobsTest()
        {
            var result = LSC1DatabaseFacade.GetJobs();
            Assert.AreEqual(59, result.Count);
        }

        [TestMethod()]
        public async void FindProcCorpsesTest()
        {
            var result = await inconsistencyFinder.FindProcLaserOrphansAsync();
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public async void FindPosCorpsesTest()
        {
            var result = await inconsistencyFinder.FindPosOrphansAsync();
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public async void FindJobCorpsesTest()
        {
            var result = await inconsistencyFinder.FindJobOrphansAsync();
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public void GetFreeJobNumberTest()
        {
            var result = LSC1DatabaseFacade.GetFreeJobNumber();
            Assert.AreEqual("0", result);
        }

        [TestMethod()]
        public void DeleteJobTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindJobsWithNameTest()
        {
            var result = LSC1DatabaseFacade.FindJobsThatUseName("WelleHeft");
            Assert.AreEqual("Job111", result[0].Name);
            Assert.AreEqual("Job112E", result[1].Name);
        }

        [TestMethod()]
        public void InsertTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CopyJobTest()
        {
            Assert.Fail();
        }
    }
}