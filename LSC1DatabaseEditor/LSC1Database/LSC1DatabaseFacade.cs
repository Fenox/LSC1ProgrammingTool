using LSC1DatabaseEditor;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSC1DatabaseLibrary
{
    public static class LSC1DatabaseFacade
    {
        public static LSC1MySqlQueryExecuter executer = new LSC1MySqlQueryExecuter(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public static List<DbJobNameRow> GetJobs()
        {
            string query = "SELECT * FROM `tjobname`";
            return executer.Execute(new ReadRowsQuery<DbJobNameRow>(query));            
        }

        internal static object FindProcCorpses()
        {
            throw new NotImplementedException();
        }

        public static void SimpleQuery(string query)
        {
            executer.Execute(new NonReturnSimpleQuery(query));
        }
        
        internal static void AssignNameToJob(string jobNr, string name)
        {
            string insertQuery = "INSERT INTO `tjobname` VALUES('" + jobNr + "', '" + name + "')";
            executer.Execute(new NonReturnSimpleQuery(insertQuery));
        }

        public static string GetFreeJobNumber()
        {
            var jobs = GetJobs();
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

        public static void DeleteJob(DbJobNameRow job, List<string> procs, List<string> positions, List<string> frames, bool deleteBaseFrame)
        {
            string deleteFromJobnameQuery = "DELETE FROM `tjobname` WHERE JobNr = '" + job.JobNr + "'";
            string deleteFromJobDataQuery = "DELETE FROM `tjobdata` WHERE JobNr = '" + job.JobNr + "'";

            executer.MultiExecute(new NonReturnSimpleQuery(deleteFromJobnameQuery))
                .Execute(new NonReturnSimpleQuery(deleteFromJobDataQuery));
            
            if(deleteBaseFrame)
            {
                //Get Entry
                string getWtEntryQuery = "SELECT * FROM `twt` WHERE WtId = '" + job.JobNr + "'";
                var wtEntry = executer.Execute(new ReadRowsQuery<DbTwtRow>(getWtEntryQuery));

                if(wtEntry.Count > 0)
                {
                    string deleteBaseFrameQuery = "DELETE FROM `tframe` WHERE Name = '" + wtEntry[0].FrameT1 + "'";
                    executer.Execute(new NonReturnSimpleQuery(deleteBaseFrameQuery));
                }
            }

            foreach (var proc in procs)
            {
                string deleteProcRobotQuery = "DELETE FROM `tprocrobot` WHERE Name = '" + proc + "'";
                string deleteProcLaserQuery = "DELETE FROM `tproclaserdata` WHERE Name = '" + proc + "'";

                executer.MultiExecute(new NonReturnSimpleQuery(deleteProcRobotQuery))
                    .Execute(new NonReturnSimpleQuery(deleteProcLaserQuery));
            }

            foreach (var pos in positions)
            {
                string deletePosQuery = "DELETE FROM `tpos` WHERE Name = '" + pos + "'";
                executer.Execute(new NonReturnSimpleQuery(deletePosQuery));
            }

            foreach (var frame in frames)
            {
                string deleteFrameQuery = "DELETE FROM `tframe` WHERE Name = '" + frame + "'";
                executer.Execute(new NonReturnSimpleQuery(deleteFrameQuery));
            }
        }

        public static List<DbJobNameRow> FindJobsThatUseName(string name)
        {
            string jobNrsWithProcNameQuery = "SELECT (JobNr) FROM `tjobdata` WHERE Name = '" + name + "' GROUP BY `JobNr`";
            string selectJobNameQuery = "SELECT * FROM `tjobname` WHERE JobNr IN (" + jobNrsWithProcNameQuery + ")";

            return executer.Execute(new ReadRowsQuery<DbJobNameRow>(selectJobNameQuery));
        }

        public static void Insert(LSC1DatabaseConnectionSettings dBSettings, DbRow dbRow)
        {
            executer.Execute(new InsertQuery(dbRow));
        }

        public static void Insert(LSC1DatabaseConnectionSettings dBSettings, string tableName, List<string> columnNames, List<string> values)
        {
            executer.Execute(new InsertQuery(tableName, columnNames, values));
        }

        internal static List<T> Read<T>(LSC1DatabaseConnectionSettings dbSettings, string query) where T : DbRow, new()
        {
            return executer.Execute(new ReadRowsQuery<T>(query));
        }

        internal async static Task<List<T>> ReadAsync<T>(LSC1DatabaseConnectionSettings dbSettings, string query) where T : DbRow, new()
        {
            return await Task.Run(() => executer.Execute(new ReadRowsQuery<T>(query)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="tableName"></param>
        /// <exception cref="MySqlException"></exception>
        public static void Insert(LSC1DatabaseConnectionSettings dbSetting, DataRow row, string tableName)
        {
            var columnsList = new List<string>();
            foreach (DataColumn item in row.Table.Columns)
            {
                columnsList.Add(item.ColumnName);
            }

            var values = new List<string>();
            foreach (var item in row.ItemArray)
            {
                values.Add(item.ToString());
            }

            try
            {
                executer.Execute(new InsertQuery(tableName, columnsList, values));                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<DbJobNameRow> FindJobsWithFrame(string frame)
        {
            string jobNrsWithProcNameQuery = "SELECT (JobNr) FROM `tjobdata` WHERE Frame = '" + frame + "' GROUP BY `JobNr`";
            string selectJobNameQuery = "SELECT * FROM `tjobname` WHERE JobNr IN (" + jobNrsWithProcNameQuery + ")";

            return executer.Execute(new ReadRowsQuery<DbJobNameRow>(selectJobNameQuery));
        }

        //TODO: test
        public static void CopyJob(string newJobName, string oldJobName, Dictionary<string, bool> keepOldPos, Dictionary<string, bool> keepOldProc, Dictionary<string, bool> keepOldFrame, bool keepBaseFrame)
        {
            //TODO: add limit to only read one row
            //zu kopierenden Job holen
            var oldJob = executer
                .Execute(new ReadRowsQuery<DbJobNameRow>("SELECT * FROM tjobname WHERE Name = '" + oldJobName + "'"))[0];

            //Neuen Job in jobname erstellen
            var freeJobNr = GetFreeJobNumber();
            string newJobQuery = "INSERT INTO `tjobname` (JobNr, Name) VALUES ('" + freeJobNr + "', '" + newJobName + "')";

            //TODO Errorhandling falls der JobName schon existiert.
            executer.Execute(new NonReturnSimpleQuery(newJobQuery));

            //Neuen eintrag in twt erstellen
            //Holen des alten twt Eintrags
            //Annahme, das Twt id = job id
            string oldtwtQuery = "SELECT * FROM `twt` WHERE WtId = '" + oldJob.JobNr + "'";
            var oldTwtEntrys = executer.Execute(new ReadRowsQuery<DbTwtRow>(oldtwtQuery));

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
                    //TODO add limit 1 to query
                    var oldFrameEntry = executer.Execute(new ReadRowsQuery<DbFrameRow>(oldFrameQuery))[0];
                    oldFrameEntry.Name = newFrameName;
                    executer.Execute(new InsertQuery(oldFrameEntry));
                }
                catch (Exception)
                {
                    MessageBox.Show("Bereinigen sie nicht genutzte Frames mit (Frame Leichen finden)! und kopieren sie den Job nocheinmal.");
                }

                oldTwtEntry.FrameT1 = newFrameName;
                oldTwtEntry.FrameT2 = newFrameName;
            }

            executer.Execute(new InsertQuery(oldTwtEntry));

            //Neue Einträge in tframe
            string frameNamesInJob = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr + "' AND `Frame` != '?' GROUP By `Frame`";
            var oldFrameNames = executer.Execute(new ReadRowsQuery<DbJobDataRow>(frameNamesInJob));
            Dictionary<string, string> oldToNewFrame = new Dictionary<string, string>();

            for (int i = 0; i < oldFrameNames.Count; i++)
            {
                string newFrameName = "";
                if (keepOldFrame[oldFrameNames[i].Frame])
                    newFrameName = oldFrameNames[i].Frame;
                else
                {
                    newFrameName = "f" + newJobName + freeJobNr + "Nr" + i;
                    executer.Execute(new InsertQuery(new DbFrameRow(newFrameName, "Job", "0", "0", "0", "0", "0", "0")));
                }

                oldToNewFrame.Add(oldFrameNames[i].Frame, newFrameName);
            }

            //Neue Einträge in tpos
            string tposQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr + "' AND `What` = 'pos' GROUP By `Name`";
            var posDataList = executer.Execute(new ReadRowsQuery<DbJobDataRow>(tposQuery));
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
                    executer.Execute(new InsertQuery(new DbPosRow(newPosName, "point", "0", "0", "0", "0", "0", "0", "False")));
                }

                oldPosToNewPosName.Add(posDataList[i].Name, newPosName);
            }

            //Neue Einträge in tprocLaserdata
            string tprocLaserQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr + "' AND `What` = 'proc' GROUP By `Name`";
            var procLaserDataList = executer.Execute(new ReadRowsQuery<DbJobDataRow>(tprocLaserQuery));
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
                    executer.Execute(new InsertQuery(new DbProcLaserDataRow(newprocLaserName, "0", "no", "0", "0")));
                }

                oldprocLaserToNewProcLaser.Add(procLaserDataList[i].Name, newprocLaserName);
            }

            //Neue Einträge in tprocRobot
            string tprocRobotQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + oldJob.JobNr + "' AND `What` = 'proc' GROUP By `Name`";
            var procRobotList = executer.Execute(new ReadRowsQuery<DbJobDataRow>(tprocRobotQuery));
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
                    executer.Execute(new InsertQuery(new DbProcRobotRow(newprocRobotName, "0", "point", "lin", "0", "0", "0", "0", "0", "0", "100", "15", "100", "16.6", "0.1", "0.1")));
                }

                oldProcRobToNewProcRobName.Add(procRobotList[i].Name, newprocRobotName);
            }

            //Neue Einträge in tjobdata
            string selectJobData = "SELECT * FROM tjobdata WHERE JobNr = " + oldJob.JobNr;

            foreach (var jobData in executer.Execute(new ReadRowsQuery<DbJobDataRow>(selectJobData)))
            {
                string name = jobData.Name;
                if (oldPosToNewPosName.ContainsKey(jobData.Name) && jobData.What.Equals("pos"))
                    name = oldPosToNewPosName[jobData.Name];
                else if (oldProcRobToNewProcRobName.ContainsKey(jobData.Name) && jobData.What.Equals("proc"))
                    name = oldProcRobToNewProcRobName[jobData.Name];

                string frame = "?";
                if (oldToNewFrame.ContainsKey(jobData.Frame))
                    frame = oldToNewFrame[jobData.Frame];

                executer.Execute(new InsertQuery(new DbJobDataRow(freeJobNr, jobData.Step, jobData.Who, jobData.What, name, jobData.MoveParam, frame, jobData.Tool, jobData.Interpol, jobData.WEM, jobData.LaserProgNr)));
            }
        }

        public static void DeleteRow(string tableName, List<string> rowValues, List<string> columnNames)
        {
            string deleteQuery = "DELETE FROM `" + tableName + "` WHERE ";
            
            int i = 0;
            foreach (var item in rowValues)
            {
                deleteQuery += "`" + columnNames[i] + "` = '" + item + "' AND ";
                i++;
            }

            deleteQuery = deleteQuery.Remove(deleteQuery.Length - 5);

            executer.Execute(new NonReturnSimpleQuery(deleteQuery));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public static DataTable GetTable(string query)
        {
            DataTable dt = new DataTable();


            MySqlConnection connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            connection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

            try
            {

                adapter.Fill(dt);
            }
            catch (MySqlException)
            {
                throw;
            }
            finally
            {
                adapter.Dispose();
                builder.Dispose();
                connection.Dispose();
            }

            return dt;
        }
    }
}
