using LSC1DatabaseEditor;
using System.Collections.Generic;
using System.Linq;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.LSC1JobRepresentation
{
    public class LSC1JobData
    {
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
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
            JobData = new ReadRowsQuery<DbJobDataRow>(jobDataQuery).Execute(Connection).ToList();

            string laserDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tproclaserdata, null);
            LaserData = new ReadRowsQuery<DbProcLaserDataRow>(laserDataQuery).Execute(Connection).ToList();
            string plcDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocplc, null);
            PLCData = new ReadRowsQuery<DbProcPlcRow>(plcDataQuery).Execute(Connection).ToList();
            string pulseDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocpulse, null);
            PulseData = new ReadRowsQuery<DbProcPulseRow>(pulseDataQuery).Execute(Connection).ToList();
            string robotDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocrobot, null);
            RobotData = new ReadRowsQuery<DbProcRobotRow>(robotDataQuery).Execute(Connection).ToList();
            string turnDataQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tprocturn, null);
            TurnData = new ReadRowsQuery<DbProcTurnRow>(turnDataQuery).Execute(Connection).ToList();

            string framesQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tframe, null);
            Frames = new ReadRowsQuery<DbFrameRow>(framesQuery).Execute(Connection).ToList();
            string moveParamsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tmoveparam, null);
            MoveParams = new ReadRowsQuery<DbMoveParamRow>(moveParamsQuery).Execute(Connection).ToList();
            string positionsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.tpos, null);
            Positions = new ReadRowsQuery<DbPosRow>(positionsQuery).Execute(Connection).ToList();
            string toolsQuery = SQLStringGenerator.GetData(JobName.JobNr, TablesEnum.ttool, null);
            Tools = new ReadRowsQuery<DbToolRow>(toolsQuery).Execute(Connection).ToList();
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
