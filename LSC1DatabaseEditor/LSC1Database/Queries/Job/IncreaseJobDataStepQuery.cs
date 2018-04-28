using System.Linq;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class IncreaseJobDataStepQuery : MySqlQuery<object>
    {
        private readonly string jobNr;
        private readonly int startingStep;
        private readonly string increment;

        /// <summary>
        /// Creates a new query that increments the step number of all conecutive steps
        /// (starting from startingStep) and increases them by the given increment.
        /// </summary>
        /// <param name="jobNr"></param>
        /// <param name="startingStep"></param>
        /// <param name="increment"></param>
        public IncreaseJobDataStepQuery(string jobNr, int startingStep, string increment)
        {
            this.jobNr = jobNr;
            this.startingStep = startingStep;
            this.increment = increment;
        }

        protected override object ProtectedExecution(MySqlConnection connection)
        {
            const string getHigherStepsQuery = "SELECT Step FROM `tjobdata` WHERE Step > '@StartStep' AND JobNr = '@JobNr'";
            var higherSteps = new ReadRowsQuery<DbJobDataRow>(getHigherStepsQuery, 
                    new MySqlParameter("StartStep", (startingStep + 1).ToString()),
                    new MySqlParameter("JobNr", jobNr))
                .Execute(connection)
                .Select(item => int.Parse(item.Step)).ToList();

            higherSteps.Sort();
            higherSteps.Reverse();

            foreach (int item in higherSteps)
            {
                string setStepHigherQuery = "UPDATE `tjobdata` SET Step = Step + @Increment" +
                                            " WHERE Step = '@Step' AND JobNr = '@JobNr'";

                new NonReturnSimpleQuery(setStepHigherQuery,
                        new MySqlParameter("Increment", increment),
                        new MySqlParameter("Step", item),
                        new MySqlParameter("JobNr", jobNr))
                    .Execute(connection);
            }

            return null;
        }
    }
}
