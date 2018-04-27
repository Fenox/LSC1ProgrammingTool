using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary
{
    public static class SQLStringGenerator
    {
        public static string GetStepItemQuery(string jobId, TablesEnum tableName, string procFilter, int step)
        {
            switch (tableName)
            {
                case TablesEnum.NOTABLE:
                    break;
                case TablesEnum.tframe:
                    break;
                case TablesEnum.tjobdata:
                    return "SELECT * FROM `" + tableName + "` WHERE JobNr = '" + jobId + "' AND Step = '" + step + "'";
                case TablesEnum.tjobname:
                    break;
                case TablesEnum.tmoveparam:
                    break;
                case TablesEnum.tpos:
                    return "SELECT * FROM `" + tableName + "` WHERE Name = '" + procFilter + "' AND Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";
                case TablesEnum.tproclaserdata:
                case TablesEnum.tprocplc:
                case TablesEnum.tprocpulse:
                case TablesEnum.tprocrobot:
                case TablesEnum.tprocturn:
                    return "SELECT * FROM `" + tableName + "` WHERE Name = '" + procFilter + "' AND Step = '" + step + "' AND Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";
                case TablesEnum.ttool:
                    break;
                case TablesEnum.ttable:
                    break;
                case TablesEnum.twt:
                    break;
                default:
                    break;
            }

            return "";
        }

        public static string GetData(string jobId, TablesEnum tableName, string procFilter)
        {
            switch (tableName)
            {
                case TablesEnum.NOTABLE:
                    break;
                case TablesEnum.tframe:
                    if (jobId != null)
                        return "SELECT * FROM tframe" +
                            " WHERE Name IN (SELECT DISTINCT Frame FROM tjobdata WHERE JobNr = '" + jobId + "')" +
                            " OR Name IN (SELECT DISTINCT FrameT1 FROM twt WHERE JobT1 = '" + jobId + "')" +
                            " OR Name IN (SELECT DISTINCT FrameT2 FROM twt WHERE JobT2 = '" + jobId + "')";
                    else
                        return "SELECT * FROM tframe";
                case TablesEnum.tjobdata:
                    if (jobId != null)
                        return "SELECT * FROM tjobdata WHERE JobNr = " + jobId;
                    else
                        return "SELECT * FROM tjobdata";
                case TablesEnum.tjobname:
                    return "SELECT * FROM tjobname";
                case TablesEnum.tmoveparam:
                    if (jobId != null)
                        return "SELECT * FROM tmoveparam " +
                            " WHERE Name IN (SELECT DISTINCT MoveParam FROM tjobdata WHERE JobNr = " + jobId + ")";
                    else
                        return "SELECT * FROM tmoveparam";
                case TablesEnum.tpos:
                    if (jobId != null)
                        return "SELECT * FROM tpos" +
                            " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";
                    else
                        return "SELECT * FROM tpos";
                case TablesEnum.tproclaserdata:
                    if (jobId == null && procFilter == null)
                        return "SELECT * FROM tproclaserdata";
                    else if (jobId == null)
                        return "SELECT * FROM tproclaserdata WHERE Name = '" + procFilter + "'";
                    else if (procFilter == null)
                        return "SELECT * FROM tproclaserdata" +
                            " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ") ORDER BY Step";
                    else
                        return "SELECT * FROM tproclaserdata" +
                            " WHERE Name = '" + procFilter + "' AND Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ") ORDER BY `Step`";
                case TablesEnum.tprocplc:
                    if (jobId == null)
                        return "SELECT * FROM tprocplc";
                    else
                        return "SELECT * FROM tprocplc" +
                            " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ") ORDER BY `Step`";
                case TablesEnum.tprocpulse:
                    if (jobId == null)
                        return "SELECT * FROM tprocpulse";
                    else
                        return "SELECT * FROM tprocpulse" +
                            " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ") ORDER BY `Step`";
                case TablesEnum.tprocrobot:
                    if (procFilter == null && jobId == null)
                        return "SELECT * FROM tprocrobot";
                    else if (jobId == null)
                        return "SELECT * FROM tprocrobot WHERE Name = '" + procFilter + "'";
                    else if (procFilter == null)
                        return "SELECT * FROM tprocrobot" +
                            " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";
                    else
                        return "SELECT * FROM tprocrobot" +
                            " WHERE Name = '" + procFilter + "' AND Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ") ORDER BY `Step`";
                case TablesEnum.tprocturn:
                    if (jobId == null && procFilter == null)
                        return "SELECT * FROM `tprocturn`";
                    else if (jobId == null)
                        return "SELECT * FROM tprocturn WHERE Name = '" + procFilter + "'";
                    else if (procFilter == null)
                        return "SELECT * FROM tprocturn" +
                            " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";
                    else
                        return "SELECT * FROM tprocturn" +
                            " WHERE Name = '" + procFilter + "' AND Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ") ORDER BY `Step`";
                case TablesEnum.ttool:
                    if (jobId != null)
                        return "SELECT * FROM ttool " +
                            " WHERE Name IN (SELECT DISTINCT Tool FROM tjobdata WHERE JobNr = " + jobId + ")";
                    else
                        return "SELECT * FROM ttool";
                case TablesEnum.ttable:
                    return "SELECT * FROM ttable";
                case TablesEnum.twt:
                    if (jobId == null)
                        return "SELECT * FROM twt";
                    else
                        return "SELECT * FROM twt WHERE JobT1 = '" + jobId + "' OR JobT2 = '" + jobId + "'";
                default:
                    break;
            }

            return "";
        }

        public static string HighestStep(TablesEnum table, string nameFilter)
        {
            return "SELECT MAX(CAST(Step AS SIGNED)) FROM `" + table + "` WHERE Name = '" + nameFilter + "'";
        }
    }
}
