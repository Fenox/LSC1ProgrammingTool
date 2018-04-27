using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace LSC1LibraryTests.CommonMySql.MySqlQueries
{
    [TestClass()]
    public class InsertQueryTests
    {
        private static readonly MySqlConnectionStringBuilder ConnStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = "127.0.0.1",
            Database = "lsc1test",
            UserID = "root",
            Password = ""
        };

        private static readonly MySqlConnection Connection = new MySqlConnection(ConnStringBuilder.ConnectionString);

        [TestMethod()]
        public void CreateTest()
        {
            new InsertQuery("tjobname", new[] {"JobNr", "Name"}, new[] {"testNr", "InsertTestJob"})
                .Execute(Connection);

            new NonReturnSimpleQuery("DELETE FROM tjobname WHERE JobNr = @Value",
                new MySqlParameter("Value", "testNr")).Execute(Connection);
        }
    }
}