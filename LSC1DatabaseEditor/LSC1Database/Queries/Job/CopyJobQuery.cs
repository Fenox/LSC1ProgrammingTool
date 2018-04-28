using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class CopyJobQuery : MySqlQuery<object>
    {
        private readonly string newJobName;
        private readonly string oldJobName;
        private readonly Dictionary<string, bool> keepOldPos;
        private readonly Dictionary<string, bool> keepOldProc;
        private readonly Dictionary<string, bool> keepOldFrame;
        private readonly bool keepBaseFrame;

        public CopyJobQuery(string newJobName, string oldJobName, Dictionary<string, bool> keepOldPos, Dictionary<string, bool> keepOldProc, Dictionary<string, bool> keepOldFrame, bool keepBaseFrame)
        {
            this.newJobName = newJobName;
            this.oldJobName = oldJobName;
            this.keepOldPos = keepOldPos;
            this.keepOldProc = keepOldProc;
            this.keepOldFrame = keepOldFrame;
            this.keepBaseFrame = keepBaseFrame;
        }


        protected override object ProtectedExecution(MySqlConnection connection)
        {
            //TODO: add limit to only read one row
            //zu kopierenden Job holen
            var oldJob = new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tjobname WHERE Name = '" + oldJobName + "'")
                .Execute(connection).First();

            //Neuen Job in jobname erstellen
            var freeJobNr = new GetFreeJobNumberQuery().Execute(connection);
            string newJobQuery = "INSERT INTO `tjobname` (JobNr, Name) VALUES ('" + freeJobNr + "', '" + newJobName +
                                 "')";

            //TODO Errorhandling falls der JobName schon existiert.
            new NonReturnSimpleQuery(newJobQuery).Execute(connection);

            //Neuen eintrag in twt erstellen
            //Holen des alten twt Eintrags
            //Annahme, das Twt id = job id
            string oldtwtQuery = "SELECT * FROM `twt` WHERE WtId = '" + oldJob.JobNr + "'";
            var oldTwtEntrys = new ReadRowsQuery<DbTwtRow>(oldtwtQuery).Execute(connection).ToList();

            var oldTwtEntry = new DbTwtRow();
            if (oldTwtEntrys.Any())
                oldTwtEntry = oldTwtEntrys.First();

            oldTwtEntry.WtId = freeJobNr;
            oldTwtEntry.JobT1 = freeJobNr;
            oldTwtEntry.JobT2 = freeJobNr;

            if (!keepBaseFrame && oldTwtEntrys.Any())
            {
                //Neuen Frame erstellen
                string newFrameName = "fWT" + freeJobNr;

                //Holen des alten frames 
                string oldFrameQuery = "SELECT * FROM `tframe` WHERE `Name` = '" + oldTwtEntry.FrameT1 + "'";

                //TODO add limit 1 to query
                var oldFrameEntry = new ReadRowsQuery<DbFrameRow>(oldFrameQuery).Execute(connection).First();
                oldFrameEntry.Name = newFrameName;
                new InsertQuery(oldFrameEntry).Execute(connection);

                oldTwtEntry.FrameT1 = newFrameName;
                oldTwtEntry.FrameT2 = newFrameName;
            }

            new InsertQuery(oldTwtEntry).Execute(connection);

            //Neue Einträge in tframe
            string frameNamesInJob = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr +
                                     "' AND `Frame` != '?' GROUP By `Frame`";
            var oldFrameNames = new ReadRowsQuery<DbJobDataRow>(frameNamesInJob).Execute(connection).ToList();
            Dictionary<string, string> oldToNewFrame = new Dictionary<string, string>();

            for (int i = 0; i < oldFrameNames.Count; i++)
            {
                string newFrameName = "";
                if (keepOldFrame[oldFrameNames[i].Frame])
                    newFrameName = oldFrameNames[i].Frame;
                else
                {
                    newFrameName = "f" + newJobName + freeJobNr + "Nr" + i;
                    new InsertQuery(new DbFrameRow(newFrameName, "Job", "0", "0", "0", "0", "0", "0")).Execute(
                        connection);
                }

                oldToNewFrame.Add(oldFrameNames[i].Frame, newFrameName);
            }

            //Neue Einträge in tpos
            string tposQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr +
                               "' AND `What` = 'pos' GROUP By `Name`";
            var posDataList = new ReadRowsQuery<DbJobDataRow>(tposQuery).Execute(connection).ToList();
            Dictionary<string, string> oldPosToNewPosName = new Dictionary<string, string>();

            for (int i = 0; i < posDataList.Count; i++)
            {
                string newPosName = "";
                if (keepOldPos[posDataList[i].Name])
                    newPosName = posDataList[i].Name;
                else
                {
                    newPosName = "p" + newJobName + freeJobNr + "Nr" + i;
                    //Nur wenn eine neue Position erstellt werden soll muss sie eingefügt werden.
                    new InsertQuery(new DbPosRow(newPosName, "point", "0", "0", "0", "0", "0", "0", "False")).Execute(
                        connection);
                }

                oldPosToNewPosName.Add(posDataList[i].Name, newPosName);
            }

            //Neue Einträge in tprocLaserdata
            string tprocLaserQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr +
                                     "' AND `What` = 'proc' GROUP By `Name`";
            var procLaserDataList = new ReadRowsQuery<DbJobDataRow>(tprocLaserQuery).Execute(connection).ToList();
            Dictionary<string, string> oldprocLaserToNewProcLaser = new Dictionary<string, string>();

            for (int i = 0; i < procLaserDataList.Count; i++)
            {
                string newprocLaserName = "";
                if (keepOldProc[procLaserDataList[i].Name])
                    newprocLaserName = procLaserDataList[i].Name;
                else
                {
                    newprocLaserName = "proc" + newJobName + freeJobNr + "Nr" + i;
                    //Nur wenn eine neue procLaserDatarow erstellt werden soll muss sie eingefügt werden.
                    new InsertQuery(new DbProcLaserDataRow(newprocLaserName, "0", "no", "0", "0")).Execute(connection);
                }

                oldprocLaserToNewProcLaser.Add(procLaserDataList[i].Name, newprocLaserName);
            }

            //Neue Einträge in tprocRobot
            string tprocRobotQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr +
                                     "' AND `What` = 'proc' GROUP By `Name`";
            var procRobotList = new ReadRowsQuery<DbJobDataRow>(tprocRobotQuery).Execute(connection).ToList();
            Dictionary<string, string> oldProcRobToNewProcRobName = new Dictionary<string, string>();

            for (int i = 0; i < procRobotList.Count; i++)
            {
                string newprocRobotName = "";
                if (keepOldProc[procRobotList[i].Name])
                    newprocRobotName = procRobotList[i].Name;
                else
                {
                    newprocRobotName = "proc" + newJobName + freeJobNr + "Nr" + i;
                    //Nur wenn eine neue procRobotrow erstellt werden soll muss sie eingefügt werden.
                    new InsertQuery(new DbProcRobotRow(newprocRobotName, "0", "point", "lin", "0", "0", "0", "0", "0",
                        "0", "100", "15", "100", "16.6", "0.1", "0.1")).Execute(connection);
                }

                oldProcRobToNewProcRobName.Add(procRobotList[i].Name, newprocRobotName);
            }

            //Neue Einträge in tjobdata
            string selectJobData = "SELECT * FROM tjobdata WHERE JobNr = @JobNr";

            foreach (var jobData in new ReadRowsQuery<DbJobDataRow>(selectJobData,
                new MySqlParameter("JobNr", oldJob.JobNr)).Execute(connection))
            {
                string name = jobData.Name;
                if (oldPosToNewPosName.ContainsKey(jobData.Name) && jobData.What.Equals("pos"))
                    name = oldPosToNewPosName[jobData.Name];
                else if (oldProcRobToNewProcRobName.ContainsKey(jobData.Name) && jobData.What.Equals("proc"))
                    name = oldProcRobToNewProcRobName[jobData.Name];

                string frame = "?";
                if (oldToNewFrame.ContainsKey(jobData.Frame))
                    frame = oldToNewFrame[jobData.Frame];

                new InsertQuery(new DbJobDataRow(freeJobNr, jobData.Step, jobData.Who, jobData.What, name,
                        jobData.MoveParam, frame, jobData.Tool, jobData.Interpol, jobData.WEM, jobData.LaserProgNr))
                    .Execute(connection);
            }

            return null;
        }
    }
}
