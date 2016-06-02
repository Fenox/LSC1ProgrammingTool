using GalaSoft.MvvmLight;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.TypedDataTables;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.UpdatedRows;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.UpdatingRows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

namespace LSC1DatabaseEditor.ViewModel
{
    public abstract class TableViewModelBase : ViewModelBase
    {
        public abstract TablesEnum Table { get; }
        public abstract bool HasStep { get; }
        public abstract bool HasNameColumn { get; }
        public abstract bool UsesNameFilter { get; }
        public abstract string DataGridName { get; }
        public abstract DataTable DataTable { get; set; }
        public ObservableCollection<string> NameFilterItems { get; set; } = new ObservableCollection<string>();
        public virtual void UpdateNameFilter(string jobId) { }
    }

    public abstract class TableViewModel<T> : TableViewModelBase where T : MyUpdatedDbRow, new()
    {
        private UpdatedDataTable<T> tableData;
        public UpdatedDataTable<T> TableData
        {
            get { return tableData; }
            set
            {
                tableData = value;
                RaisePropertyChanged("TableData");
            }
        }

        public override DataTable DataTable
        {
            get
            {
                return TableData.Database;
            }
            set
            {
                TableData.Database = value;
            }
        }
        
        public override TablesEnum Table
        {
            get
            {
                return TableData.TableName;
            }
        }

        public TableViewModel(LSC1DatabaseConnectionSettings conSettings, TablesEnum tableName)
        {
            TableData = new UpdatedDataTable<T>(conSettings, tableName);
        }
    }
    
    public class FrameTableViewModel : TableViewModel<UpdatingFrameRow>
    {
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "frameDataGrid"; } }

        public FrameTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tframe)
        {

        }
    }

    public class JobDataTableViewModel : TableViewModel<UpdatingJobDataRow>
    {
        public override bool HasStep { get { return true; } }
        public override bool HasNameColumn { get { return false; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "jobDataDataGrid"; } }

        public JobDataTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tjobdata)
        {

        }
    }

    public class JobNameTableViewModel : TableViewModel<UpdatingJobNameRow>
    {
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return false; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "jobNameDataGrid"; } }

        public JobNameTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tjobname)
        {

        }
    }

    public class MoveParamTableViewModel : TableViewModel<UpdatingMoveParamRow>
    {
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "moveParamDataGrid"; } }

        public MoveParamTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tmoveparam)
        {

        }
    }

    public class PosTableViewModel : TableViewModel<UpdatingPosRow>
    {
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "posDataGrid"; } }

        public PosTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tpos)
        {

        }
    }

    public class ProcLaserTableViewModel : TableViewModel<UpdatingProcLaserRow>
    {
        public override bool HasStep { get { return true; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return true; } }
        public override string DataGridName { get { return "laserDataGrid"; } }

        public ProcLaserTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tproclaserdata)
        {

        }

        public override void UpdateNameFilter(string jobId)
        {
            string getProcOfJob = "SELECT DISTINCT (Name) FROM `" + Table + "`" +
                          " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";
            
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);

            var procLaserOfJob = db.ReadSingleColumnQuery(getProcOfJob);

            NameFilterItems.Clear();
            foreach (var item in procLaserOfJob)
                NameFilterItems.Add(item);

        }
    }

    public class ProcPulseTableViewModel : TableViewModel<UpdatingProcPulseRow>
    {
        public override bool HasStep { get { return true; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return true; } }
        public override string DataGridName { get { return "procpulseDataGrid"; } }

        public ProcPulseTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tprocpulse)
        {

        }

        public override void UpdateNameFilter(string jobId)
        {
            string getProcOfJob = "SELECT DISTINCT (Name) FROM `" + Table + "`" +
                          " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);

            var procsOfJob = db.ReadSingleColumnQuery(getProcOfJob);

            NameFilterItems.Clear();
            foreach (var item in procsOfJob)
                NameFilterItems.Add(item);
        }
    }

    public class ProcRobotTableViewModel : TableViewModel<UpdatingProcRobotRow>
    {
        public override bool HasStep { get { return true; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return true; } }
        public override string DataGridName { get { return "procRobotDataGrid"; } }

        public ProcRobotTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tprocrobot)
        {

        }

        public override void UpdateNameFilter(string jobId)
        {
            string getProcOfJob = "SELECT DISTINCT (Name) FROM `" + Table + "`" +
                          " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";
            
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);

            var procLaserOfJob = db.ReadSingleColumnQuery(getProcOfJob);

            NameFilterItems.Clear();
            foreach (var item in procLaserOfJob)
                NameFilterItems.Add(item);            
        }
    }

    public class ProcPlcTableViewModel : TableViewModel<UpdatingProcPlcRow>
    {
        public override bool HasStep { get { return true; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return true; } }
        public override string DataGridName { get { return "procplcDataGrid"; } }

        public ProcPlcTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tprocplc)
        {

        }

        public override void UpdateNameFilter(string jobId)
        {
            string getProcOfJob = "SELECT DISTINCT (Name) FROM `" + Table + "`" +
                          " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";
                       
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);

            var procLaserOfJob = db.ReadSingleColumnQuery(getProcOfJob);

            NameFilterItems.Clear();
            foreach (var item in procLaserOfJob)
                NameFilterItems.Add(item);

        }
    }

    public class ProcTurnTableViewModel : TableViewModel<UpdatingProcTurnRow>
    {
        public ProcTurnTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tprocturn)
        {

        }
        public override bool HasStep { get { return true; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return true; } }
        public override string DataGridName { get { return "procTurnDataGrid"; } }

        public override void UpdateNameFilter(string jobId)
        {
            string getProcOfJob = "SELECT DISTINCT (Name) FROM `" + Table + "`" +
                          " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + jobId + ")";

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);

            var procsOfJob = db.ReadSingleColumnQuery(getProcOfJob);

            NameFilterItems.Clear();
            foreach (var item in procsOfJob)
                NameFilterItems.Add(item);
        }
    }

    public class TableTableViewModel : TableViewModel<UpdatingTableRow>
    {
        public TableTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.ttable)
        {

        }

        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return false; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "tableDataGrid"; } }
    }

    public class ToolTableViewModel : TableViewModel<UpdatingToolRow>
    {
        public ToolTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.ttool)
        {

        }
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "toolDataGrid"; } }
    }

    public class TWTTableViewModel : TableViewModel<UpdatingWTRow>
    {
        public TWTTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.twt)
        {

        }
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return false; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "twtDataGrid"; } }
    }
}
