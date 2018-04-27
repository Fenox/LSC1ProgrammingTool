using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class IncreaseProcRobotDataRow : MySqlQuery<object>
    {
        private readonly string tableName;
        private readonly int startingStep;
        private readonly string increment;
        private readonly string procName;

        public IncreaseProcRobotDataRow(string increment, int startingStep, string tableName, string procName)
        {
            this.increment = increment;
            this.startingStep = startingStep;
            this.tableName = tableName;
            this.procName = procName;
        }

        protected override object ProtectedExecution(MySqlConnection connection)
        {
            //Update all items with higher step  
            //TODO: async + save
            string getHigherStepsQuery = "SELECT Step FROM `" + tableName + "` WHERE Step > @StartingStep AND Name = @ProcName";
            var higherSteps = new ReadRowsQuery<DbProcRobotRow>(getHigherStepsQuery,
                    new MySqlParameter("StartingStep", startingStep + 1),
                    new MySqlParameter("ProcName", procName))
                .Execute(connection)
                .Select(item => int.Parse(item.Step)).ToList();

            higherSteps.Sort();
            higherSteps.Reverse();

            foreach (int step in higherSteps)
            {
                string setStepHigherQuery = "UPDATE `" + tableName + "` SET Step = Step + '@Increment' WHERE Step = '@Step' AND Name = '@ProcName'";
                new NonReturnSimpleQuery(setStepHigherQuery,
                        new MySqlParameter("Increment", increment),
                        new MySqlParameter("Step", step),
                        new MySqlParameter("ProcName", procName))
                    .Execute(connection);
            }

            return null;
        }
    }
}
