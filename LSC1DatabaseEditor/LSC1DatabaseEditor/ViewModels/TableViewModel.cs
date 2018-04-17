using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.TypedDataTables;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.UpdatedRows;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.UpdatingRows;
using LSC1Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

namespace LSC1DatabaseEditor.ViewModel
{
    /// <summary>
    /// Viewmodel representing a table (e.g. tpos), that defines common methods
    /// but does not contain the type of the data stored, to be more of an abstraction.
    /// </summary>
    public abstract class LSC1TablePropertiesViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Specifies the kind of data that is stored in this table e.g. tpos
        /// </summary>
        public abstract TablesEnum Table { get; }
        /// <summary>
        /// Specifies whether this table needs a step column indicating that the data
        /// in this table is has an order.
        /// </summary>
        public abstract bool HasStep { get; }
        /// <summary>
        /// Specifies whether this table has a name column, this column is a reference
        /// to another item that has a name e.g. a laser proc.
        /// </summary>
        public abstract bool HasNameColumn { get; }
        /// <summary>
        /// specifies whether the data in this table can be filtered by name.
        /// </summary>
        public abstract bool UsesNameFilter { get; }
        public abstract string DataGridName { get; }
        public abstract DataTable DataTable { get; set; }
        public ObservableCollection<string> NameFilterItems { get; set; } = new ObservableCollection<string>();
        public virtual void UpdateNameFilter(string jobId) { }
    }

    public abstract class LSC1TablePropertiesViewModel<T> : LSC1TablePropertiesViewModelBase where T : LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.TypedDataTables.UpdatedDbRowViewModel, new()
    {
        private UpdatedDataTableViewModel<T> tableData;
        public UpdatedDataTableViewModel<T> TableData
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
                OnDatabaseChanged();
            }
        }
        
        public override TablesEnum Table
        {
            get
            {
                return TableData.TableName;
            }
        }

        public LSC1TablePropertiesViewModel(LSC1DatabaseConnectionSettings conSettings, TablesEnum tableName)
        {
            TableData = new UpdatedDataTableViewModel<T>(conSettings, tableName);
        }

        public virtual void OnDatabaseChanged() { }
    }
    
    public class FrameTableViewModel : LSC1TablePropertiesViewModel<UpdatingFrameRow>
    {
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "frameDataGrid"; } }

        public FrameTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tframe)
        {

        }
    }

    public class JobDataTableViewModel : LSC1TablePropertiesViewModel<UpdatingJobDataRow>
    {
        public override bool HasStep { get { return true; } }
        public override bool HasNameColumn { get { return false; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "jobDataDataGrid"; } }

        public JobDataTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tjobdata)
        {

        }
    }

    public class JobNameTableViewModel : LSC1TablePropertiesViewModel<UpdatingJobNameRow>
    {
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return false; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "jobNameDataGrid"; } }

        public JobNameTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tjobname)
        {

        }

        public override void OnDatabaseChanged()
        {
            if (TableData != null && TableData.Database != null)
                TableData.Database.ColumnChanged += Database_ColumnChanged;
        }

        private void Database_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName.Equals("Name"))
            {
                Messenger.Default.Send(new JobsChangedMessage() { });
            }
        }
    }

    public class MoveParamTableViewModel : LSC1TablePropertiesViewModel<UpdatingMoveParamRow>
    {
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "moveParamDataGrid"; } }

        public MoveParamTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tmoveparam)
        {

        }
    }

    public class PosTableViewModel : LSC1TablePropertiesViewModel<UpdatingPosRow>
    {
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "posDataGrid"; } }

        public PosTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.tpos)
        {

        }
    }

    public class ProcLaserTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcLaserRow>
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
            var procLaserOfJob = OfflineDatabase.AllJobProcNameMappings
                                    .Find(mapping => mapping.JobNr.Equals(jobId))
                                    .ProcLaserNames;

            if (procLaserOfJob == null)
                return;

            NameFilterItems.Clear();
            foreach (var item in procLaserOfJob)
                NameFilterItems.Add(item);

        }
    }

    public class ProcPulseTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcPulseRow>
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
            var procPulseOfJob = OfflineDatabase.AllJobProcNameMappings
                                           .Find(mapping => mapping.JobNr.Equals(jobId))
                                           .ProcPulseNames;

            if (procPulseOfJob == null)
                return;

            NameFilterItems.Clear();
            foreach (var item in procPulseOfJob)
                NameFilterItems.Add(item);
        }
    }

    public class ProcRobotTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcRobotRow>
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
            var procRobotOfJob = OfflineDatabase.AllJobProcNameMappings
                                       .Find(mapping => mapping.JobNr.Equals(jobId))
                                       .ProcRobotNames;

            if (procRobotOfJob == null)
                return;

            NameFilterItems.Clear();
            foreach (var item in procRobotOfJob)
                NameFilterItems.Add(item);            
        }
    }

    public class ProcPlcTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcPlcRow>
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
            var procPLCOfJob = OfflineDatabase.AllJobProcNameMappings
                                    .Find(mapping => mapping.JobNr.Equals(jobId))
                                    .ProcPLCNames;

            if (procPLCOfJob == null)
                return;

            NameFilterItems.Clear();
            foreach (var item in procPLCOfJob)
                NameFilterItems.Add(item);

        }
    }

    public class ProcTurnTableViewModel : LSC1TablePropertiesViewModel<UpdatingProcTurnRow>
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
            var procTurnOfJob = OfflineDatabase.AllJobProcNameMappings
                                        .Find(mapping => mapping.JobNr.Equals(jobId))
                                        .ProcTurnNames;

            if (procTurnOfJob == null)
                return;

            NameFilterItems.Clear();
            foreach (var item in procTurnOfJob)
                NameFilterItems.Add(item);
        }
    }

    public class TableTableViewModel : LSC1TablePropertiesViewModel<UpdatingTableRow>
    {
        public TableTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.ttable)
        {

        }

        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return false; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "tableDataGrid"; } }
    }

    public class ToolTableViewModel : LSC1TablePropertiesViewModel<UpdatingToolRow>
    {
        public ToolTableViewModel(LSC1DatabaseConnectionSettings conSettings) : base(conSettings, TablesEnum.ttool)
        {

        }
        public override bool HasStep { get { return false; } }
        public override bool HasNameColumn { get { return true; } }
        public override bool UsesNameFilter { get { return false; } }
        public override string DataGridName { get { return "toolDataGrid"; } }
    }

    public class TWTTableViewModel : LSC1TablePropertiesViewModel<UpdatingWTRow>
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
