using LSC1DatabaseEditor.LSC1Database.Queries.Job;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditorTests.LSC1Database.Queries.Job
{
    [TestClass]
    public class CountJobDataReferencesTests
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
        public void CountJobDataReferencesNotInJobTest()
        {
            Assert.AreEqual(8, new CountJobDataReferences("pStartWeKlR2", "pos").Execute(Connection));
        }
    }
}