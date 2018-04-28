using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace LSC1LibraryTests.CommonMySql.MySqlQueries
{
    [TestClass()]
    public class CountQueryTests
    {
        private static readonly MySqlConnectionStringBuilder ConnStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = "127.0.0.1",
            Database = "lsc1test",
            UserID = "root",
            Password = ""
        };

        [TestMethod()]
        public void ExecuteTest()
        {
            int result = new CountQuery("SELECT COUNT(*) FROM tpos").Execute(new MySqlConnection(ConnStringBuilder.ConnectionString));

            Assert.AreEqual(66, result);
        }
    }
}