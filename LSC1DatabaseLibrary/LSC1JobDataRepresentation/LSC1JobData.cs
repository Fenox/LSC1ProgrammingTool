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
        public void LoadJob(LSC1DatabaseConnectionSettings con)
        {
            string jobDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tjobdata, null);
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(con);
            JobData = db.ReadRows<DbJobDataRow>(jobDataQuery);
            
            string laserDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tproclaserdata, null);
            LaserData = db.ReadRows<DbProcLaserDataRow>(laserDataQuery);            
            string plcDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocplc, null);
            PLCData = db.ReadRows<DbProcPlcRow>(plcDataQuery);
            string pulseDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocpulse, null);
            PulseData = db.ReadRows<DbProcPulseRow>(pulseDataQuery);
            string robotDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocrobot, null);
            RobotData = db.ReadRows<DbProcRobotRow>(robotDataQuery);
            string turnDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocturn, null);
            TurnData = db.ReadRows<DbProcTurnRow>(turnDataQuery);

            string framesQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tframe, null);
            Frames = db.ReadRows<DbFrameRow>(framesQuery);
            string moveParamsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tmoveparam, null);
            MoveParams = db.ReadRows<DbMoveParamRow>(moveParamsQuery);
            string positionsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tpos, null);
            Positions = db.ReadRows<DbPosRow>(positionsQuery);
            string toolsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.ttool, null);
            Tools = db.ReadRows<DbToolRow>(toolsQuery);
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
