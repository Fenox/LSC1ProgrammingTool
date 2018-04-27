using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.TypedDataTables;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditorTests.LSC1DbEditor.ViewModels.DatabaseViewModel.TypedDataTables
{
    [TestClass()]
    public class UpdatedDbRowViewModelTests
    {
        private static readonly MySqlConnectionStringBuilder ConnStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = "127.0.0.1",
            Database = "lsc1test",
            UserID = "root",
            Password = ""
        };
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();

        [TestMethod]
        public async Task ValuesChangedTest()
        {
            string query = SQLStringGenerator.GetData("11", TablesEnum.tproclaserdata, "WelleHeft");

            DataTable dataTable = new GetTableQuery(query).Execute(new MySqlConnection(ConnStringBuilder.ConnectionString));
            var table = new ProcLaserTableViewModel(ConnStringBuilder.ConnectionString)
            {
                DataTable = dataTable
            };


            var testRow = table.TableData.Rows[3];
            Assert.AreEqual("1000", testRow.Row.ItemArray[3].ToString());

            await Task.Run(() => table.TableData.Rows[0].Row[3] = "1200");

            var res1 = new ReadRowsQuery<DbProcLaserDataRow>(
                    "SELECT * FROM tproclaserdata WHERE Name IN (SELECT Name FROM tjobdata WHERE JobNr = '11' AND What = 'proc') ORDER BY Step")
                .Execute(new MySqlConnection(ConnStringBuilder.ConnectionString)).ToList();

            Assert.AreEqual("1000", res1[3].Power);


            table.TableData.Rows[0].Row[3] = "1000";

            var res2 = new ReadRowsQuery<DbProcLaserDataRow>("SELECT * FROM tproclaserdata WHERE Name IN (SELECT Name FROM tjobdata WHERE JobNr = '11' AND What = 'proc') ORDER BY Step")
                .Execute(new MySqlConnection(ConnStringBuilder.ConnectionString)).ToList();

            Assert.AreEqual("1000", res2[3].Power);
        }
    }
}