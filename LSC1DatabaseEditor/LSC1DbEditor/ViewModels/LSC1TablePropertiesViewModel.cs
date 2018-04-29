using System.Data;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.TypedDataTables;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.UpdatingRows;
using LSC1DatabaseLibrary;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public abstract class LSC1TablePropertiesViewModel<T> : LSC1TablePropertiesViewModelBase where T : UpdatedDbRowViewModel, new()
    {
        private UpdatedDataTableViewModel<T> tableData;
        public UpdatedDataTableViewModel<T> TableData
        {
            get => tableData;
            set
            {
                tableData = value;
                RaisePropertyChanged();
            }
        }

        public override DataTable DataTable
        {
            get => TableData.Database;
            set
            {
                TableData.Database = value;
                OnDatabaseChanged();
            }
        }
        
        public override TablesEnum Table => TableData.TableName;

        protected LSC1TablePropertiesViewModel(string connectionString, TablesEnum tableName) => TableData = new UpdatedDataTableViewModel<T>(connectionString, tableName);

        public virtual void OnDatabaseChanged() { }
    }
    
    public class FrameTableViewModel : LSC1TablePropertiesViewModel<UpdatingFrameRow>
    {
        public override bool HasStep => false;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => false;
        public override string DataGridName => "frameDataGrid";

        public FrameTableViewModel(string connectionString) : base(connectionString, TablesEnum.tframe)
        {

        }
    }

    public class JobDataTableViewModel : LSC1TablePropertiesViewModel<UpdatingJobDataRow>
    {
        public override bool HasStep => true;
        public override bool HasNameColumn => false;
        public override bool UsesNameFilter => false;
        public override string DataGridName => "jobDataDataGrid";

        public JobDataTableViewModel(string connectionString) : base(connectionString, TablesEnum.tjobdata)
        {

        }
    }

    public class JobNameTableViewModel : LSC1TablePropertiesViewModel<UpdatingJobNameRow>
    {
        public override bool HasStep => false;
        public override bool HasNameColumn => false;
        public override bool UsesNameFilter => false;
        public override string DataGridName => "jobNameDataGrid";

        public JobNameTableViewModel(string connectionString) : base(connectionString, TablesEnum.tjobname)
        {

        }

        public override void OnDatabaseChanged()
        {
            if (TableData?.Database != null)
                TableData.Database.ColumnChanged += Database_ColumnChanged;
        }

        private static void Database_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName.Equals("Name"))
            {
                Messenger.Default.Send(new JobsChangedMessage());
            }
        }
    }

    public class MoveParamTableViewModel : LSC1TablePropertiesViewModel<UpdatingMoveParamRow>
    {
        public override bool HasStep => false;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => false;
        public override string DataGridName => "moveParamDataGrid";

        public MoveParamTableViewModel(string connectionString) : base(connectionString, TablesEnum.tmoveparam)
        {

        }
    }

    public class PosTableViewModel : LSC1TablePropertiesViewModel<UpdatingPosRow>
    {
        public override bool HasStep => false;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => false;
        public override string DataGridName => "posDataGrid";

        public PosTableViewModel(string connectionString) : base(connectionString, TablesEnum.tpos)
        {

        }
    }

    public class ProcLaserTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcLaserRow>
    {
        public override bool HasStep => true;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => true;
        public override string DataGridName => "laserDataGrid";

        public ProcLaserTableViewModel(string connectionString) : base(connectionString, TablesEnum.tproclaserdata)
        {

        }

        public override async void UpdateNameFilter(string jobId)
        {
            var procLaserOfJob = await new LSC1AsyncDBTaskExecuter()
                                    .DoTaskAsync("Aktualisiere Names Filter..." ,() => OfflineDatabase.AllJobProcNameMappings
                                        .Find(mapping => mapping.JobNr.Equals(jobId))
                                        .ProcLaserNames);

            NameFilterItems.Clear();
            RaisePropertyChanged($"NameFilterItems");
            if (procLaserOfJob == null)
                return;

            foreach (string item in procLaserOfJob)
                NameFilterItems.Add(item);
        }
    }

    public class ProcPulseTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcPulseRow>
    {
        public override bool HasStep => true;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => true;
        public override string DataGridName => "procpulseDataGrid";

        public ProcPulseTableViewModel(string connectionString) : base(connectionString, TablesEnum.tprocpulse)
        {

        }

        public override async void UpdateNameFilter(string jobId)
        {
            var procPulseOfJob = await new LSC1AsyncDBTaskExecuter()
                                    .DoTaskAsync("Aktualisiere Names Filter", () => OfflineDatabase.AllJobProcNameMappings
                                           .Find(mapping => mapping.JobNr.Equals(jobId))
                                           .ProcPulseNames);

            NameFilterItems.Clear();
            RaisePropertyChanged($"NameFilterItems");
            if (procPulseOfJob == null)
                return;

            foreach (var item in procPulseOfJob)
                NameFilterItems.Add(item);
        }
    }

    public class ProcRobotTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcRobotRow>
    {
        public override bool HasStep => true;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => true;
        public override string DataGridName => "procRobotDataGrid";

        public ProcRobotTableViewModel(string connectionString) : base(connectionString, TablesEnum.tprocrobot)
        {

        }

        public override async void UpdateNameFilter(string jobId)
        {
            var procRobotOfJob = await new LSC1AsyncDBTaskExecuter()
                                    .DoTaskAsync("Aktualisiere Names Filter", () => OfflineDatabase.AllJobProcNameMappings
                                       .Find(mapping => mapping.JobNr.Equals(jobId))
                                       .ProcRobotNames);

            NameFilterItems.Clear();
            RaisePropertyChanged($"NameFilterItems");
            if (procRobotOfJob == null)
                return;
            
            foreach (string item in procRobotOfJob)
                NameFilterItems.Add(item);            
        }
    }

    public class ProcPlcTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcPlcRow>
    {
        public override bool HasStep => true;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => true;
        public override string DataGridName => "procplcDataGrid";

        public ProcPlcTableViewModel(string connectionString) : base(connectionString, TablesEnum.tprocplc)
        {

        }

        public override async void UpdateNameFilter(string jobId)
        {
            var procPLCOfJob = await new LSC1AsyncDBTaskExecuter()
                                    .DoTaskAsync("Aktualisiere Names Filter", () => OfflineDatabase.AllJobProcNameMappings
                                    .Find(mapping => mapping.JobNr.Equals(jobId))
                                    .ProcPLCNames);

            NameFilterItems.Clear();
            RaisePropertyChanged($"NameFilterItems");
            if (procPLCOfJob == null)
                return;

            foreach (string item in procPLCOfJob)
                NameFilterItems.Add(item);

        }
    }

    public class ProcTurnTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcTurnRow>
    {
        public ProcTurnTableViewModel(string connectionString) : base(connectionString, TablesEnum.tprocturn)
        {

        }
        public override bool HasStep => true;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => true;
        public override string DataGridName => "procTurnDataGrid";

        public override async void UpdateNameFilter(string jobId)
        {
            var procTurnOfJob = await new LSC1AsyncDBTaskExecuter()
                                    .DoTaskAsync("Aktualisiere Names Filter", () => OfflineDatabase.AllJobProcNameMappings
                                        .Find(mapping => mapping.JobNr.Equals(jobId))
                                        .ProcTurnNames);

            NameFilterItems.Clear();
            if (procTurnOfJob == null)
                return;

            foreach (var item in procTurnOfJob)
                NameFilterItems.Add(item);
        }
    }

    public class TableTableViewModel : LSC1TablePropertiesViewModel<UpdatingTableRow>
    {
        public TableTableViewModel(string connectionString) : base(connectionString, TablesEnum.ttable)
        {

        }

        public override bool HasStep => false;
        public override bool HasNameColumn => false;
        public override bool UsesNameFilter => false;
        public override string DataGridName => "tableDataGrid";
    }

    public class ToolTableViewModel : LSC1TablePropertiesViewModel<UpdatingToolRow>
    {
        public ToolTableViewModel(string connectionString) : base(connectionString, TablesEnum.ttool)
        {

        }
        public override bool HasStep => false;
        public override bool HasNameColumn => true;
        public override bool UsesNameFilter => false;
        public override string DataGridName => "toolDataGrid";
    }

    public class TWTTableViewModel : LSC1TablePropertiesViewModel<UpdatingWTRow>
    {
        public TWTTableViewModel(string connectionString) : base(connectionString, TablesEnum.twt)
        {

        }
        public override bool HasStep => false;
        public override bool HasNameColumn => false;
        public override bool UsesNameFilter => false;
        public override string DataGridName => "twtDataGrid";
    }
}
