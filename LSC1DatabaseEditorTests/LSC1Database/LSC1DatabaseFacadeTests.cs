using Microsoft.VisualStudio.TestTools.UnitTesting;
using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.Tests
{
    [TestClass()]
    public class LSC1DatabaseFacadeTests
    {
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
        public void FindProcCorpsesTest()
        {
            var result = LSC1DatabaseFacade.FindProcCorpses();
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public void FindPosCorpsesTest()
        {
            var result = LSC1DatabaseFacade.FindPosCorpses();
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public void FindJobCorpsesTest()
        {
            var result = LSC1DatabaseFacade.FindJobCorpses();
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