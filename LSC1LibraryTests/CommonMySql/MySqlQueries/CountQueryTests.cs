using Microsoft.VisualStudio.TestTools.UnitTesting;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries.Tests
{
    [TestClass()]
    public class CountQueryTests
    {
        private static MySqlConnectionStringBuilder connStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = "127.0.0.1",
            Database = "lsc1test",
            UserID = "root",
            Password = ""
        };

        [TestMethod()]
        public void ExecuteTest()
        {
            int result = new CountQuery("SELECT COUNT(*) FROM tpos").Execute(new MySqlConnection(connStringBuilder.ConnectionString));

            Assert.AreEqual(66, result);
        }
    }
}