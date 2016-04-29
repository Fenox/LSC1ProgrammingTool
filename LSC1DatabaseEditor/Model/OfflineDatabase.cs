using LSC1DatabaseLibrary;
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
        public static List<string> AllPossibleInterpolValues { get; set; } = new List<string> { "ptp" };
        public static List<string> AllPossibleWEMValues { get; set; } = new List<string> { "yes", "no" };
        public static List<string> AllPossibleLaserProgNrValues { get; set; } = new List<string> { "15" };
        public static List<string> AllPossibleWhatValues { get; set; } = new List<string> { "laser", "pos", "proc", "sequenc", "pulse" };
        public static List<string> AllPossibleWhoValues { get; set; } = new List<string> { "plc", "robot" };
        public static List<string> AllPossibleBeamOnValues { get; set; } = new List<string> { "no", "yes" };
        public static List<string> AllPossibleKindValues { get; set; } = new List<string> { "point" };
        public static List<string> AllPossibleLockedValues { get; set; } = new List<string> { "False", "True" };


        public static void UpdateAllPosNames()
        {
            string possiblePosNamesQuery = "SELECT DISTINCT Name FROM `tpos`";
            List<string> possibleNames = new List<string>();

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            AllPosNames = db.ReadSingleColumnQuery(possiblePosNamesQuery);
        }

        public static void UpdateAllProcNames()
        {
            string possibleProcsNamesQuery = "SELECT DISTINCT Name FROM `tprocRobot`";
            List<string> possibleNames = new List<string>();

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            AllProcNames = db.ReadSingleColumnQuery(possibleProcsNamesQuery);
        }

        public static void UpdateAllProcPLCNames()
        {
            string possibleNamesQuery = "SELECT DISTINCT Name FROM `tprocplc`";
            List<string> possibleNames = new List<string>();

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            AllProcPLCNames = db.ReadSingleColumnQuery(possibleNamesQuery);
        }

        public static void UpdateAllProcPulseNames()
        {
            string possibleNamesQuery = "SELECT DISTINCT Name FROM `tprocpulse`";
            List<string> possibleNames = new List<string>();

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            AllProcPulseNames = db.ReadSingleColumnQuery(possibleNamesQuery);
        }

        public static void UpdateAllMoveParamNames()
        {
            string possibleNamesQuery = "SELECT DISTINCT Name FROM `tmoveparam`";
            List<string> possibleNames = new List<string>();

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            AllMoveParamNames = db.ReadSingleColumnQuery(possibleNamesQuery);
        }

        public static void UpdateAllFrameNames()
        {
            string possibleNamesQuery = "SELECT DISTINCT Name FROM `tframe`";
            List<string> possibleNames = new List<string>();

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            AllFrameNames = db.ReadSingleColumnQuery(possibleNamesQuery);
        }

        public static void UpdateAllToolNames()
        {
            string possibleNamesQuery = "SELECT DISTINCT Name FROM `ttool`";
            List<string> possibleNames = new List<string>();

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            AllToolNames = db.ReadSingleColumnQuery(possibleNamesQuery);
        }

        public static void UpdateAll()
        {
            UpdateAllPosNames();
            UpdateAllProcNames();
            UpdateAllProcPLCNames();
            UpdateAllMoveParamNames();
            UpdateAllFrameNames();
            UpdateAllToolNames();
            UpdateAllProcPulseNames();
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
                default:
                    break;
            }
        }
    }
}
