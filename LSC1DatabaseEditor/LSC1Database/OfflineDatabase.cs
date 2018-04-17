using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1Library
{
    public static class OfflineDatabase
    {
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

        public static void UpdateAllPosNames(LSC1DatabaseConnectionSettings con)
        {
            AllPosNames = GetDistinctColumnFromTable(con, "tpos", "Name");
        }

        public static void UpdateAllProcNames(LSC1DatabaseConnectionSettings con)
        {
            AllProcNames = GetDistinctColumnFromTable(con, "tprocRobot", "Name");
        }

        public static void UpdateAllProcPLCNames(LSC1DatabaseConnectionSettings con)
        {
            AllProcPLCNames = GetDistinctColumnFromTable(con, "tprocplc", "Name");
        }

        public static void UpdateAllProcPulseNames(LSC1DatabaseConnectionSettings con)
        {
            AllProcPulseNames = GetDistinctColumnFromTable(con, "tprocpulse", "Name");
        }

        public static void UpdateAllProcTurnNames(LSC1DatabaseConnectionSettings con)
        {
            AllProcTurnNames = GetDistinctColumnFromTable(con, "tprocturn", "Name");
        }

        public static void UpdateAllMoveParamNames(LSC1DatabaseConnectionSettings con)
        {
            AllMoveParamNames = GetDistinctColumnFromTable(con, "tmoveparam", "Name");
        }

        public static void UpdateAllFrameNames(LSC1DatabaseConnectionSettings con)
        {
            AllFrameNames = GetDistinctColumnFromTable(con, "tframe", "Name");
        }

        public static void UpdateAllToolNames(LSC1DatabaseConnectionSettings con)
        {
            AllToolNames = GetDistinctColumnFromTable(con, "ttool", "Name");
        }

        public static void UpdateAllJobNames(LSC1DatabaseConnectionSettings con)
        {
            AllJobNames = GetDistinctColumnFromTable(con, "tjobname", "Name");
        }

        public static List<string> GetDistinctColumnFromTable(LSC1DatabaseConnectionSettings con, string tableName, string columnName)
        {
            //TODO put in facade
            string possibleNamesQuery = "SELECT DISTINCT " + columnName + " FROM `" + tableName + "`";
            List<string> possibleNames = new List<string>();

            return LSC1DatabaseFacade.Read<DbRow>(con, possibleNamesQuery)
                 .Select(val => val.Values[0])
                 .ToList();
        }

        public static void UpdateJobsAndProcs(LSC1DatabaseConnectionSettings con)
        {
            //TODO put in Facade
            List<string> dbNames = new List<string> { "tprocrobot", "tprocplc", "tprocpulse", "tproclaserdata", "tprocturn" };

            var nameJobNrTableResults = new List<List<Tuple<string, string>>>();
            foreach (var item in dbNames)
            {
                string possibleNamesQuery = "SELECT DISTINCT Name, JobNr FROM `tjobdata` WHERE Name IN (SELECT DISTINCT (Name) FROM `" + item + "`)";

                var nameJobNrMapping = LSC1DatabaseFacade.Read<DbRow>(con, possibleNamesQuery)
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

        public static void UpdateAll(LSC1DatabaseConnectionSettings con)
        {
            foreach (TablesEnum item in Enum.GetValues(typeof(TablesEnum)))
                UpdateTable(con, item);

            UpdateJobsAndProcs(con);
        }

        public static void UpdateTable(LSC1DatabaseConnectionSettings con, TablesEnum table)
        {
            //Updaten der OfflineDatenbank
            switch (table)
            {
                case TablesEnum.tframe:
                    OfflineDatabase.UpdateAllFrameNames(con);
                    break;
                case TablesEnum.tmoveparam:
                    OfflineDatabase.UpdateAllMoveParamNames(con);
                    break;
                case TablesEnum.tpos:
                    OfflineDatabase.UpdateAllPosNames(con);
                    break;
                case TablesEnum.tproclaserdata:
                    OfflineDatabase.UpdateAllProcNames(con);
                    break;
                case TablesEnum.tprocplc:
                    OfflineDatabase.UpdateAllProcPLCNames(con);
                    break;
                case TablesEnum.tprocpulse:
                    OfflineDatabase.UpdateAllProcPulseNames(con);
                    break;
                case TablesEnum.tprocrobot:
                    OfflineDatabase.UpdateAllProcNames(con);
                    break;
                case TablesEnum.tprocturn:
                    OfflineDatabase.UpdateAllProcTurnNames(con);
                    break;
                case TablesEnum.ttool:
                    OfflineDatabase.UpdateAllToolNames(con);
                    break;
                case TablesEnum.tjobname:
                    OfflineDatabase.UpdateAllJobNames(con);
                    break;
                default:
                    break;
            }
        }
    }
}
