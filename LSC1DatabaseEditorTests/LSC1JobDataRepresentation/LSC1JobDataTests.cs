using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.LSC1JobRepresentation.Tests
{
    [TestClass()]
    public class LSC1JobDataTests
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
        public void LoadJobTest()
        {
            var jobRow = LSC1DatabaseFacade.GetJobs().Find(job => job.JobNr.Equals("223"));
            LSC1JobData jobData = new LSC1JobData(jobRow);
            jobData.LoadJob();

            Assert.AreEqual("Job223P", jobData.JobName.Name);
            Assert.IsTrue(jobData.Positions.Exists(pos => pos.Name.Equals("pLager3")));
            Assert.AreEqual("240.389", jobData.Positions[2].X);
        }
    }
}