using System;
using System.Linq;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class DeleteJobQuery : MySqlQuery<object>
    {
        private readonly string jobName;

        public DeleteJobQuery(string jobName)
        {
            this.jobName = jobName;
        }

        protected override object ProtectedExecution(MySqlConnection connection)
        {
            // Check whether job exists
            if (!new ExistsJobQuery(jobName).Execute(connection))
                throw new ArgumentException("Given job does not exist");

            string jobNr = new GetJobNrQuery(jobName).Execute(connection);

            //Check whether the given job is the active job, if so, stop and do not delete.
            if (new IsJobActiveQuery(jobNr).Execute(connection))
                throw new ArgumentException("Given job is active in ttable");

            DeleteJobFrames(connection, jobNr);
            //Delete positions of job
            SafeDelete(connection, jobNr, "tpos", "pos");
            //Delete ProcLaserData of job
            SafeDelete(connection, jobNr, "tproclaserdata", "proc");
            //Delete ProcRobot of job
            SafeDelete(connection, jobNr, "tprocrobot", "proc");
            //Delete proc plc of job
            SafeDelete(connection, jobNr, "tprocplc", "sequence");
            //Delete proc pulse of job
            SafeDelete(connection, jobNr, "tprocpulse", "pulse");
            //Delete proc turn of job
            SafeDelete(connection, jobNr, "tprocturn", "turn");

            //Delete tjobdata of job
            DeleteJobData(connection, jobNr);

            //Delete tjobname
            new NonReturnSimpleQuery("DELETE FROM tjobname WHERE JobNr = @JobNr",
                new MySqlParameter("JobNr", jobNr)).Execute(connection);

            return null;
        }

        private static void DeleteJobData(MySqlConnection connection, string jobNr)
        {
            new NonReturnSimpleQuery("DELETE FROM tjobdata WHERE JobNr = @JobNr",
                new MySqlParameter("JobNr", jobNr)).Execute(connection);
        }

        /// <summary>
        /// Deletes all a proc, pos, pulse, sequence, turn... from the given table (e.g. tpos)
        /// that have no where other referenced than in the given job.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="jobNr"></param>
        /// <param name="tableToDeleteFrom"></param>
        /// <param name="what"></param>
        public static void SafeDelete(MySqlConnection connection, string jobNr, string tableToDeleteFrom, string what)
        {
            var namesOfWhatOfJob = new GetNamesOfJobQuery(jobNr, what).Execute(connection).ToList();

            var namesOfWhatWithOneJobRefernece =
                namesOfWhatOfJob.Where(pos => new CountJobDataReferences(pos, what).Execute(connection) == 1);

            foreach (string proc in namesOfWhatWithOneJobRefernece)
                new NonReturnSimpleQuery("DELETE FROM " + tableToDeleteFrom + " WHERE Name = @Name",
                        new MySqlParameter("Name", proc))
                    .Execute(connection);
        }

        private static void DeleteJobFrames(MySqlConnection connection, string jobNr)
        {
            //1. hole Namen aller Frames des Jobs
            var framesOfJob = new GetFrameNamesOfJobQuery(jobNr).Execute(connection).ToList();
            //2. prüfe Anzahl der referenzen in job data, die nicht zum aktuellen job gehören
            var framesWithOneReference = framesOfJob
                .Where(frame => new CountJobFrameReferencesQuery(jobNr, frame).Execute(connection) == 0).ToList();

            //3. lösche alle frames die eine Referenz haben.
            foreach (string frame in framesWithOneReference)
                new NonReturnSimpleQuery("DELETE FROM tframe WHERE Name = @Name",
                    new MySqlParameter("Name", frame))
                        .Execute(connection);
        }
    }
}
