using System;
using System.Collections.Generic;
using System.Linq;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1Database
{
    public static class OfflineDatabase
    {
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public static List<string> AllPosNames { get; set; } = new List<string>();
        public static List<string> AllProcNames { get; set; } = new List<string>();
        public static List<string> AllProcPLCNames { get; set; } = new List<string>();
        public static List<string> AllProcTurnNames { get; set; } = new List<string>();
        public static List<string> AllMoveParamNames { get; set; } = new List<string>();
        public static List<string> AllFrameNames { get; set; } = new List<string>();
        public static List<string> AllToolNames { get; set; } = new List<string>();
        public static List<string> AllProcPulseNames { get; set; } = new List<string>();
        public static List<string> AllJobNames { get; set; } = new List<string>();
        public static List<string> AllPossibleInterpolValues { get; set; } = new List<string> { "ptp", "lin" };
        public static List<string> AllPossibleWEMValues { get; set; } = new List<string> { "yes", "no" };
        public static List<string> AllPossibleEnableValues { get; set; } = new List<string> { "yes", "no" };
        public static List<string> AllPossibleLaserProgNrValues { get; set; } = new List<string> { "15" };
        public static List<string> AllPossibleWhatValues { get; set; } = new List<string> { "laser", "pos", "proc", "sequenc", "pulse", "turn" };
        public static List<string> AllPossibleWhoValues { get; set; } = new List<string> { "plc", "robot" };
        public static List<string> AllPossibleBeamOnValues { get; set; } = new List<string> { "no", "yes" };
        public static List<string> AllPossibleKindValues { get; set; } = new List<string> { "point" };
        public static List<string> AllPossibleLockedValues { get; set; } = new List<string> { "False", "True" };
        public static List<string> AllPossibleTypValues { get; set; } = new List<string> { "Job", "WT", "0" };
        public static List<JobAndProcNames> AllJobProcNameMappings { get; set; } = new List<JobAndProcNames>();

        public static void UpdateAllPosNames(string con)
        {
            AllPosNames = GetDistinctColumnFromTable(con, "tpos", "Name");
        }

        public static void UpdateAllProcNames(string con)
        {
            AllProcNames = GetDistinctColumnFromTable(con, "tprocRobot", "Name");
        }

        public static void UpdateAllProcPLCNames(string con)
        {
            AllProcPLCNames = GetDistinctColumnFromTable(con, "tprocplc", "Name");
        }

        public static void UpdateAllProcPulseNames(string con)
        {
            AllProcPulseNames = GetDistinctColumnFromTable(con, "tprocpulse", "Name");
        }

        public static void UpdateAllProcTurnNames(string con)
        {
            AllProcTurnNames = GetDistinctColumnFromTable(con, "tprocturn", "Name");
        }

        public static void UpdateAllMoveParamNames(string con)
        {
            AllMoveParamNames = GetDistinctColumnFromTable(con, "tmoveparam", "Name");
        }

        public static void UpdateAllFrameNames(string con)
        {
            AllFrameNames = GetDistinctColumnFromTable(con, "tframe", "Name");
        }

        public static void UpdateAllToolNames(string con)
        {
            AllToolNames = GetDistinctColumnFromTable(con, "ttool", "Name");
        }

        public static void UpdateAllJobNames(string con)
        {
            AllJobNames = GetDistinctColumnFromTable(con, "tjobname", "Name");
        }

        public static List<string> GetDistinctColumnFromTable(string con, string tableName, string columnName)
        {
            //TODO put in facade
            string possibleNamesQuery = "SELECT DISTINCT " + columnName + " FROM `" + tableName + "`";

            return new ReadRowsQuery<DbRow>(possibleNamesQuery)
                .Execute(Connection)
                .Select(val => val.Values[0])
                .ToList();
        }

        public static void UpdateJobsAndProcs(string con)
        {
            //TODO put in Facade
            var dbNames = new List<string> { "tprocrobot", "tprocplc", "tprocpulse", "tproclaserdata", "tprocturn" };

            var nameJobNrTableResults = new List<List<Tuple<string, string>>>();
            foreach (var item in dbNames)
            {
                string possibleNamesQuery = "SELECT DISTINCT Name, JobNr FROM `tjobdata` WHERE Name IN (SELECT DISTINCT (Name) FROM `" + item + "`)";

                var nameJobNrMapping = new ReadRowsQuery<DbRow>(possibleNamesQuery).Execute(Connection)
                     .Select(val => new Tuple<string, string>(val.Values[0], val.Values[1]))
                     .ToList();

                nameJobNrTableResults.Add(nameJobNrMapping);
            }

            var jobNumbers = nameJobNrTableResults[0].Select(val => val.Item2).ToList();

            AllJobProcNameMappings = jobNumbers.Select((jobNum) => new JobAndProcNames()
            {
                JobNr = jobNum,
                ProcRobotNames = nameJobNrTableResults[0].FindAll(res => res.Item2.Equals(jobNum)).Select(val => val.Item1).ToList(),
                ProcPLCNames = nameJobNrTableResults[1].FindAll(res => res.Item2.Equals(jobNum)).Select(val => val.Item1).ToList(),
                ProcPulseNames = nameJobNrTableResults[2].FindAll(res => res.Item2.Equals(jobNum)).Select(val => val.Item1).ToList(),
                ProcLaserNames = nameJobNrTableResults[3].FindAll(res => res.Item2.Equals(jobNum)).Select(val => val.Item1).ToList(),
                ProcTurnNames = nameJobNrTableResults[4].FindAll(res => res.Item2.Equals(jobNum)).Select(val => val.Item1).ToList(),
            }).ToList();
        }

        public struct JobAndProcNames
        {
            public string JobNr { get; set; }
            public List<string> ProcPulseNames { get; set; }
            public List<string> ProcRobotNames { get; set; }
            public List<string> ProcPLCNames { get; set; }
            public List<string> ProcLaserNames { get; set; }
            public List<string> ProcTurnNames { get; internal set; }
        }

        public static void UpdateAll(string con)
        {
            foreach (TablesEnum item in Enum.GetValues(typeof(TablesEnum)))
                UpdateTable(con, item);

            UpdateJobsAndProcs(con);
        }

        public static void UpdateTable(string connectionString, TablesEnum table)
        {
            //Updaten der OfflineDatenbank
            switch (table)
            {
                case TablesEnum.tframe:
                    OfflineDatabase.UpdateAllFrameNames(connectionString);
                    break;
                case TablesEnum.tmoveparam:
                    OfflineDatabase.UpdateAllMoveParamNames(connectionString);
                    break;
                case TablesEnum.tpos:
                    OfflineDatabase.UpdateAllPosNames(connectionString);
                    break;
                case TablesEnum.tproclaserdata:
                    OfflineDatabase.UpdateAllProcNames(connectionString);
                    break;
                case TablesEnum.tprocplc:
                    OfflineDatabase.UpdateAllProcPLCNames(connectionString);
                    break;
                case TablesEnum.tprocpulse:
                    OfflineDatabase.UpdateAllProcPulseNames(connectionString);
                    break;
                case TablesEnum.tprocrobot:
                    OfflineDatabase.UpdateAllProcNames(connectionString);
                    break;
                case TablesEnum.tprocturn:
                    OfflineDatabase.UpdateAllProcTurnNames(connectionString);
                    break;
                case TablesEnum.ttool:
                    OfflineDatabase.UpdateAllToolNames(connectionString);
                    break;
                case TablesEnum.tjobname:
                    OfflineDatabase.UpdateAllJobNames(connectionString);
                    break;
            }
        }
    }
}
