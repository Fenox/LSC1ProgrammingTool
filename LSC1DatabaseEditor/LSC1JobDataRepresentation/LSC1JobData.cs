using LSC1DatabaseEditor;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.LSC1JobRepresentation
{
    public class LSC1JobData
    {
        public DbJobNameRow JobName { get; set; }

        public List<DbJobDataRow> JobData { get; set; }

        public List<DbProcLaserDataRow> LaserData { get; set; }
        public List<DbProcPlcRow> PLCData { get; set; }
        public List<DbProcPulseRow> PulseData { get; set; }
        public List<DbProcRobotRow> RobotData { get; set; }
        public List<DbProcTurnRow> TurnData { get; set; }

        public List<DbFrameRow> Frames { get; set; }
        public List<DbMoveParamRow> MoveParams { get; set; }
        public List<DbPosRow> Positions { get; set; }
        public List<DbToolRow> Tools { get; set; }
        
        public LSC1JobData(DbJobNameRow job)
        {
            JobName = job;
        }
        public void LoadJob()
        {
            string jobDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tjobdata, null);
            JobData = LSC1DatabaseFacade.Read<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, jobDataQuery);

            string laserDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tproclaserdata, null);
            LaserData = LSC1DatabaseFacade.Read<DbProcLaserDataRow>(LSC1UserSettings.Instance.DBSettings, laserDataQuery);
            string plcDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocplc, null);
            PLCData = LSC1DatabaseFacade.Read<DbProcPlcRow>(LSC1UserSettings.Instance.DBSettings, plcDataQuery);
            string pulseDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocpulse, null);
            PulseData = LSC1DatabaseFacade.Read<DbProcPulseRow>(LSC1UserSettings.Instance.DBSettings, pulseDataQuery);
            string robotDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocrobot, null);
            RobotData = LSC1DatabaseFacade.Read<DbProcRobotRow>(LSC1UserSettings.Instance.DBSettings, robotDataQuery);
            string turnDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocturn, null);
            TurnData = LSC1DatabaseFacade.Read<DbProcTurnRow>(LSC1UserSettings.Instance.DBSettings, turnDataQuery);

            string framesQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tframe, null);
            Frames = LSC1DatabaseFacade.Read<DbFrameRow>(LSC1UserSettings.Instance.DBSettings, framesQuery);
            string moveParamsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tmoveparam, null);
            MoveParams = LSC1DatabaseFacade.Read<DbMoveParamRow>(LSC1UserSettings.Instance.DBSettings, moveParamsQuery);
            string positionsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tpos, null);
            Positions = LSC1DatabaseFacade.Read<DbPosRow>(LSC1UserSettings.Instance.DBSettings, positionsQuery);
            string toolsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.ttool, null);
            Tools = LSC1DatabaseFacade.Read<DbToolRow>(LSC1UserSettings.Instance.DBSettings, toolsQuery);
        }

        public DbProcLaserDataRow FilterLaserDataBy(string name, int step)
        {
            return LaserData.FirstOrDefault(data => data.Name == name && data.Step == step.ToString());
        }

        public DbProcPlcRow FilterPlcDataBy(string name, int step)
        {
            return PLCData.FirstOrDefault(data => data.Name == name && data.Step == step.ToString());
        }

        public DbProcPulseRow FilterPulseDataBy(string name, int step)
        {
            return PulseData.FirstOrDefault(data => data.Name == name && data.Step == step.ToString());
        }

        public DbProcRobotRow FilterRobotDataBy(string name, int step)
        {
            return RobotData.FirstOrDefault(data => data.Name == name && data.Step == step.ToString());
        }

        public DbProcTurnRow FilterTurnDataBy(string name, int step)
        {
            return TurnData.FirstOrDefault(data => data.Name == name && data.Step == step.ToString());
        }
    }  
}
