using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using LSC1DatabaseEditor.LSC1Database.Queries.Job;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.LSC1JobRepresentation.Tests
{
    [TestClass()]
    public class LSC1JobDataTests
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
        public void LoadJobTest()
        {
            var jobRow = new GetJobsQuery().Execute(Connection).ToList().Find(job => job.JobNr.Equals("223"));
            LSC1JobData jobData = new LSC1JobData(jobRow);
            jobData.LoadJob();

            Assert.AreEqual("Job223P", jobData.JobName.Name);
            Assert.IsTrue(jobData.Positions.Exists(pos => pos.Name.Equals("pLager3")));
            Assert.AreEqual("240.389", jobData.Positions[2].X);
        }
    }
}