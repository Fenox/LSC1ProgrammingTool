﻿using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseEditor.Model
{
    public static class OfflineDatabase
    {
        public static List<string> AllPosNames { get; set; } = new List<string>();
        public static List<string> AllProcNames { get; set; } = new List<string>();
        public static List<string> AllProcPLCNames { get; set; } = new List<string>();
        public static List<string> AllMoveParamNames { get; set; } = new List<string>();
        public static List<string> AllFrameNames { get; set; } = new List<string>();
        public static List<string> AllToolNames { get; set; } = new List<string>();
        public static List<string> AllProcPulseNames { get; set; } = new List<string>();
        public static List<string> AllJobNames { get; set; } = new List<string>();
        public static List<string> AllPossibleInterpolValues { get; set; } = new List<string> { "ptp", "lin" };
        public static List<string> AllPossibleWEMValues { get; set; } = new List<string> { "yes", "no" };
        public static List<string> AllPossibleEnableValues { get; set; } = new List<string> { "yes", "no" };
        public static List<string> AllPossibleLaserProgNrValues { get; set; } = new List<string> { "15" };
        public static List<string> AllPossibleWhatValues { get; set; } = new List<string> { "laser", "pos", "proc", "sequenc", "pulse" };
        public static List<string> AllPossibleWhoValues { get; set; } = new List<string> { "plc", "robot" };
        public static List<string> AllPossibleBeamOnValues { get; set; } = new List<string> { "no", "yes" };
        public static List<string> AllPossibleKindValues { get; set; } = new List<string> { "point" };
        public static List<string> AllPossibleLockedValues { get; set; } = new List<string> { "False", "True" };
        public static List<string> AllPossibleTypValues { get; set; } = new List<string> { "Job", "WT", "0" };


        public static void UpdateAllPosNames()
        {
            AllPosNames = GetDistinctColumnFromTable("tpos", "Name");
        }

        public static void UpdateAllProcNames()
        {
            AllProcNames = GetDistinctColumnFromTable("tprocRobot", "Name");
        }

        public static void UpdateAllProcPLCNames()
        {
            AllProcPLCNames = GetDistinctColumnFromTable("tprocplc", "Name");
        }

        public static void UpdateAllProcPulseNames()
        {
            AllProcPulseNames = GetDistinctColumnFromTable("tprocpulse", "Name");
        }

        public static void UpdateAllMoveParamNames()
        {
            AllMoveParamNames = GetDistinctColumnFromTable("tmoveparam", "Name");
        }

        public static void UpdateAllFrameNames()
        {
            AllFrameNames = GetDistinctColumnFromTable("tframe", "Name");
        }

        public static void UpdateAllToolNames()
        {
            AllToolNames = GetDistinctColumnFromTable("ttool", "Name");
        }

        public static void UpdateAllJobNames()
        {
            AllJobNames = GetDistinctColumnFromTable("tjobname", "Name");
        }

        public static List<string> GetDistinctColumnFromTable(string tableName, string columnName)
        {
            string possibleNamesQuery = "SELECT DISTINCT " + columnName + " FROM `"+ tableName +"`";
            List<string> possibleNames = new List<string>();

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            return db.ReadSingleColumnQuery(possibleNamesQuery);
        }

        public static void UpdateAll()
        {
            foreach (TablesEnum item in Enum.GetValues(typeof(TablesEnum)))
                UpdateTable(item);
        }

        public static void UpdateTable(TablesEnum table)
        {
            //Updaten der OfflineDatenbank
            switch (table)
            {
                case TablesEnum.tframe:
                    OfflineDatabase.UpdateAllFrameNames();
                    break;
                case TablesEnum.tmoveparam:
                    OfflineDatabase.UpdateAllMoveParamNames();
                    break;
                case TablesEnum.tpos:
                    OfflineDatabase.UpdateAllPosNames();
                    break;
                case TablesEnum.tproclaserdata:
                    OfflineDatabase.UpdateAllProcNames();
                    break;
                case TablesEnum.tprocplc:
                    OfflineDatabase.UpdateAllProcPLCNames();
                    break;
                case TablesEnum.tprocpulse:
                    OfflineDatabase.UpdateAllProcPulseNames();
                    break;
                case TablesEnum.tprocrobot:
                    OfflineDatabase.UpdateAllProcNames();
                    break;
                case TablesEnum.ttool:
                    OfflineDatabase.UpdateAllToolNames();
                    break;
                case TablesEnum.tjobname:
                    OfflineDatabase.UpdateAllJobNames();
                    break;
                default:
                    break;
            }
        }
    }
}
