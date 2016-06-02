using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSC1DatabaseLibrary
{
    public static class LSC1DatabaseFunctions
    {
        public static List<DbJobNameRow> GetJobs(LSC1DatabaseConnectionSettings con)
        {
            string query = "SELECT * FROM `tjobname`";
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            return db.ReadRows<DbJobNameRow>(query).ToList();
        }

        public static List<string> FindProcCorpses(LSC1DatabaseConnectionSettings con)
        {
            //Corpses in procLaserData
            string procLaserCorpsesQuery = "SELECT * FROM `tproclaserdata` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            var procLaserCorpses = db.ReadRows<DbProcLaserDataRow>(procLaserCorpsesQuery);

            //Corpses in procRobot
            string procRobotCorpsesQuery = "SELECT * FROM `tprocrobot` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";
            var procRobotCorpses = db.ReadRows<DbProcLaserDataRow>(procRobotCorpsesQuery);


            List<string> result = new List<string>();
            foreach (var item in procLaserCorpses)
            {
                result.Add(item.Name);
            }

            foreach (var item in procRobotCorpses)
            {
                if (!result.Contains(item.Name))
                    result.Add(item.Name);
            }

            return result;
        }

        public static IEnumerable<string> FindPosCorpses(LSC1DatabaseConnectionSettings con)
        {
            //Corpses in pos
            string posCorpsesQuery = "SELECT * FROM `tpos` WHERE Name NOT IN (SELECT Name FROM `tjobdata`) GROUP BY `Name`";

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            var posCorpses = db.ReadRows<DbPosRow>(posCorpsesQuery);

            List<string> result = new List<string>();
            foreach (var item in posCorpses)
                yield return item.Name;
        }

        public static List<string> FindJobCorpses(LSC1DatabaseConnectionSettings con)
        {
            string jobCorpsesQuery = "SELECT DISTINCT (JobNr) FROM `tjobdata` WHERE JobNr NOT IN (SELECT JobNr FROM `tjobname`)";

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
           
            var jobCorpses = db.ReadSingleColumnQuery(jobCorpsesQuery);
                       
            return jobCorpses;
        }

        public static string GetFreeJobNumber(LSC1DatabaseConnectionSettings con)
        {
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);

            string query = "SELECT * FROM `tjobname`";
            var jobs = db.ReadRows<DbJobNameRow>(query).ToList();
            int jobNr = -1;

            for (int i = 0; i < jobs.Count + 1; i++)
            {
                if (!jobs.Exists((DbJobNameRow j) => j.JobNr.Equals(i.ToString())))
                {
                    jobNr = i;
                    break;
                }
            }

            return jobNr.ToString();
        }

        public static void DeleteJob(LSC1DatabaseConnectionSettings con, DbJobNameRow job, List<string> procs, List<string> positions, List<string> frames, bool deleteBaseFrame)
        {
            string deleteFromtJobnameQuery = "DELETE FROM `tjobname` WHERE JobNr = '" + job.JobNr + "'";
            string deleteFromJobDataQuery = "DELETE FROM `tjobdata` WHERE JobNr = '" + job.JobNr + "'";

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);

            db.ExecuteQuery(deleteFromtJobnameQuery);
            db.ExecuteQuery(deleteFromJobDataQuery);

            if(deleteBaseFrame)
            {
                //Get Entry
                string getWtEntryQuery = "SELECT * FROM `twt` WHERE WtId = '" + job.JobNr + "'";
                var wtEntry = db.ReadRows<DbTwtRow>(getWtEntryQuery);

                string deleteBaseFrameQuery = "DELETE FROM `tframe` WHERE Name = '" + wtEntry[0].FrameT1 + "'";
                db.ExecuteQuery(deleteBaseFrameQuery);
            }

            foreach (var proc in procs)
            {
                string deleteProcRobotQuery = "DELETE FROM `tprocrobot` WHERE Name = '" + proc + "'";
                string deleteProcLaserQuery = "DELETE FROM `tproclaserdata` WHERE Name = '" + proc + "'";

                db.ExecuteQuery(deleteProcRobotQuery);
                db.ExecuteQuery(deleteProcLaserQuery);
            }

            foreach (var pos in positions)
            {
                string deletePosQuery = "DELETE FROM `tpos` WHERE Name = '" + pos + "'";
                db.ExecuteQuery(deletePosQuery);
            }

            foreach (var frame in frames)
            {
                string deleteFrameQuery = "DELETE FROM `tframe` WHERE Name = '" + frame + "'";
                db.ExecuteQuery(deleteFrameQuery);
            }
        }

        public static List<DbJobNameRow> FindJobsWithName(LSC1DatabaseConnectionSettings con, string name)
        {
            string jobNrsWithProcNameQuery = "SELECT (JobNr) FROM `tjobdata` WHERE Name = '" + name + "' GROUP BY `JobNr`";
            string selectJobNameQuery = "SELECT * FROM `tjobname` WHERE JobNr IN (" + jobNrsWithProcNameQuery + ")";

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            return db.ReadRows<DbJobNameRow>(selectJobNameQuery);
        }

        public static List<DbJobNameRow> FindJobsWithFrame(LSC1DatabaseConnectionSettings con, string frame)
        {
            string jobNrsWithProcNameQuery = "SELECT (JobNr) FROM `tjobdata` WHERE Frame = '" + frame + "' GROUP BY `JobNr`";
            string selectJobNameQuery = "SELECT * FROM `tjobname` WHERE JobNr IN (" + jobNrsWithProcNameQuery + ")";

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            return db.ReadRows<DbJobNameRow>(selectJobNameQuery);
        }

        public static void CopyJob(LSC1DatabaseConnectionSettings con, string newJobName, string oldJobName, Dictionary<string, bool> keepOldPos, Dictionary<string, bool> keepOldProc, Dictionary<string, bool> keepOldFrame, bool keepBaseFrame)
        {
            //zu kopierenden Job holen
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            var oldJob = db.ReadRows<DbJobNameRow>("SELECT * FROM tjobname WHERE Name = '" + oldJobName + "'", 1)[0];

            //Neuen Job in jobname erstellen
            var freeJobNr = GetFreeJobNumber(con);
            string newJobQuery = "INSERT INTO `tjobname` (JobNr, Name) VALUES ('" + freeJobNr + "', '" + newJobName + "')";
            db.ExecuteQuery(newJobQuery); //TODO Errorhandling falls der JobName schon existiert.

            //Neuen eintrag in twt erstellen
            //Holen des alten twt Eintrags
            //Annahme, das Twt id = job id
            string oldtwtQuery = "SELECT * FROM `twt` WHERE WtId = '" + oldJob.JobNr + "'";
            var oldTwtEntrys = db.ReadRows<DbTwtRow>(oldtwtQuery);

            var oldTwtEntry = new DbTwtRow();
            if (oldTwtEntrys.Count > 0)
                oldTwtEntry = oldTwtEntrys[0];

            oldTwtEntry.WtId = freeJobNr;
            oldTwtEntry.JobT1 = freeJobNr;
            oldTwtEntry.JobT2 = freeJobNr;

            if(!keepBaseFrame && oldTwtEntrys.Count > 0)
            {
                //Neuen Frame erstellen
                string newFrameName = "fWT" + freeJobNr;

                //Holen des alten frames 
                string oldFrameQuery = "SELECT * FROM `tframe` WHERE `Name` = '" + oldTwtEntry.FrameT1 + "'";
                
                try
                {
                    var oldFrameEntry = db.ReadRows<DbFrameRow>(oldFrameQuery)[0];
                    oldFrameEntry.Name = newFrameName;
                    db.Insert(oldFrameEntry);
                }
                catch (Exception)
                {
                    MessageBox.Show("Bereinigen sie nicht genutzte Frames mit (Frame Leichen finden)! und kopieren sie den Job nocheinmal.");
                }

                oldTwtEntry.FrameT1 = newFrameName;
                oldTwtEntry.FrameT2 = newFrameName;
            }

            db.Insert(oldTwtEntry);

            //Neue Einträge in tframe
            string frameNamesInJob = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr + "' AND `Frame` != '?' GROUP By `Frame`";
            var oldFrameNames = db.ReadRows<DbJobDataRow>(frameNamesInJob);
            Dictionary<string, string> oldToNewFrame = new Dictionary<string, string>();

            for (int i = 0; i < oldFrameNames.Count; i++)
            {
                string newFrameName = "";
                if (keepOldFrame[oldFrameNames[i].Frame])
                    newFrameName = oldFrameNames[i].Frame;
                else
                {
                    newFrameName = "f" + newJobName + freeJobNr + "Nr" + i;
                    db.Insert(new DbFrameRow(newFrameName, "Job", "0", "0", "0", "0", "0", "0"));
                }

                oldToNewFrame.Add(oldFrameNames[i].Frame, newFrameName);
            }

            //Neue Einträge in tpos
            string tposQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr + "' AND `What` = 'pos' GROUP By `Name`";
            var posDataList = db.ReadRows<DbJobDataRow>(tposQuery);
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
                    db.Insert(new DbPosRow(newPosName, "point", "0", "0", "0", "0", "0", "0", "False"));
                }

                oldPosToNewPosName.Add(posDataList[i].Name, newPosName);
            }

            //Neue Einträge in tprocLaserdata
            string tprocLaserQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr + "' AND `What` = 'proc' GROUP By `Name`";
            var procLaserDataList = db.ReadRows<DbJobDataRow>(tprocLaserQuery);
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
                    db.Insert(new DbProcLaserDataRow(newprocLaserName, "0", "no", "0", "0"));
                }

                oldprocLaserToNewProcLaser.Add(procLaserDataList[i].Name, newprocLaserName);
            }

            //Neue Einträge in tprocRobot
            string tprocRobotQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr + "' AND `What` = 'proc' GROUP By `Name`";
            var procRobotList = db.ReadRows<DbJobDataRow>(tprocRobotQuery);
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
                    db.Insert(new DbProcRobotRow(newprocRobotName, "0", "point", "lin", "0", "0", "0", "0", "0", "0", "100", "15", "100", "16.6", "0.1", "0.1"));
                }

                oldProcRobToNewProcRobName.Add(procRobotList[i].Name, newprocRobotName);
            }

            //Neue Einträge in tjobdata
            string selectJobData = "SELECT * FROM tjobdata WHERE JobNr = " + oldJob.JobNr;

            foreach (var jobData in db.ReadRows<DbJobDataRow>(selectJobData))
            {
                string name = jobData.Name;
                if (oldPosToNewPosName.ContainsKey(jobData.Name) && jobData.What.Equals("pos"))
                    name = oldPosToNewPosName[jobData.Name];
                else if (oldProcRobToNewProcRobName.ContainsKey(jobData.Name) && jobData.What.Equals("proc"))
                    name = oldProcRobToNewProcRobName[jobData.Name];

                string frame = "?";
                if (oldToNewFrame.ContainsKey(jobData.Frame))
                    frame = oldToNewFrame[jobData.Frame];

                db.Insert(new DbJobDataRow(freeJobNr, jobData.Step, jobData.Who, jobData.What, name, jobData.MoveParam, frame, jobData.Tool, jobData.Interpol, jobData.WEM, jobData.LaserProgNr));
            }
        }

        public static void DeleteRow(LSC1DatabaseConnectionSettings con, string tableName, List<string> rowValues, List<string> columnNames)
        {
            string deleteQuery = "DELETE FROM `" + tableName + "` WHERE ";
            
            int i = 0;
            foreach (var item in rowValues)
            {
                deleteQuery += "`" + columnNames[i] + "` = '" + item + "' AND ";
                i++;
            }

            deleteQuery = deleteQuery.Remove(deleteQuery.Length - 5);

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            db.ExecuteQuery(deleteQuery);
        }
    }
}
