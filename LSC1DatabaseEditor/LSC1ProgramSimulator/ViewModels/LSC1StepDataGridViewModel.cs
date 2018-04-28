using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.LSC1CommonTool.Messages;
using LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels.DataStructures;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseLibrary;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LSC1DatabaseEditor.Common.Messages;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels
{
    public class LSC1StepDataGridViewModel : ViewModelBase
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();
        public LSC1TablePropertiesViewModelBase items1Table = new ProcRobotTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString);
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
        public LSC1TablePropertiesViewModelBase Items1
        {
            get => items1Table;
            set
            {
                items1Table = value;
                RaisePropertyChanged("Items1");
            }
        }
        public LSC1TablePropertiesViewModelBase items2Table = new ProcLaserTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString);
        public LSC1TablePropertiesViewModelBase Items2
        {
            get => items2Table;
            set
            {
                items2Table = value;
                RaisePropertyChanged("Items2");
            }
        }

        public DbJobNameRow Job { get; set; }


        private ObservableCollection<LSC1TablePropertiesViewModelBase> tables;
        public ObservableCollection<LSC1TablePropertiesViewModelBase> Tables
        {
            get => tables;
            set
            {
                tables = value;
                RaisePropertyChanged("Tables");
            }
        }

        public LSC1StepDataGridViewModel()
        {
            Tables = new ObservableCollection<LSC1TablePropertiesViewModelBase>
            {
                new FrameTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new JobDataTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new JobNameTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new MoveParamTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new PosTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcLaserTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcPlcTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcPulseTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcRobotTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcTurnTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new TableTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ToolTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new TWTTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString)
            };

            Messenger.Default.Register<LSC1JobChangedMessage>(this, LSC1SimulatorViewModel.MessageToken, (msg) => Job = msg.NewJob);
            Messenger.Default.Register<SelectedTreeViewItemChanged>(this, LSC1SimulatorViewModel.MessageToken, PopulateGridItems);
            Messenger.Default.Register<DataGridCellValueChangedMessage>(this, LSC1SimulatorViewModel.MessageToken, OnCellValueChanged);
        }

        private void OnCellValueChanged(DataGridCellValueChangedMessage msg)
        {
            //Create String that Updates a row 
            string queryString = "UPDATE " + msg.TableVM.Table +
                       " SET `" + msg.ColumnName + "` = '" + msg.NewValue +
                       "' WHERE ";

            for (int i = 0; i < msg.TableVM.DataTable.Columns.Count; i++)
            {
                string columnNamei = msg.TableVM.DataTable.Columns[i].ColumnName;

                if (msg.ColumnName == columnNamei)
                {
                    //Falls die letzte Spalte geändert wurde muss das AND wieder weg
                    if (i == msg.TableVM.DataTable.Columns.Count - 1)
                        queryString = queryString.Remove(queryString.Length - 4);

                    continue;
                }

                queryString += "`" + columnNamei + "` = '" + msg.Row.Row[i] + "'";

                if (i != msg.TableVM.DataTable.Columns.Count - 1)
                    queryString += " AND ";
            }

            try
            {
                new NonReturnSimpleQuery(queryString).Execute(Connection);            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            //Updaten der OfflineDatenbank, falls wichtiges geändert wurde
            if (msg.ColumnName == "Name")
            {
                OfflineDatabase.UpdateTable(LSC1UserSettings.Instance.DBSettings.ConnectionString, msg.TableVM.Table);
            }
            
            Messenger.Default.Send(new LSC1JobChangedMessage(Job), LSC1SimulatorViewModel.MessageToken);
        }

        private async void PopulateGridItems(SelectedTreeViewItemChanged msg)
        {
            if (msg.SelectedItem.GetType() == typeof(LSC1TreeViewPointLeaveItem))
            {
                var item = (LSC1TreeViewPointLeaveItem)msg.SelectedItem;

                foreach (DbRow instruction in item.InstructionStepData.Instructions)
                {
                    if (instruction.GetType() == typeof(DbPosRow))
                    {
                        var posRow = (DbPosRow)instruction;
                        //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
                        Messenger.Default.Send(new TableSelectionChangedMessage(new PosTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString)), LSC1SimulatorViewModel.MessageToken);
                        string itemQuery = SQLStringGenerator.GetStepItemQuery(Job.JobNr, TablesEnum.tpos, posRow.Name, 0);

                        Items1 = Tables.First(t => t.Table == TablesEnum.tpos);  
                        Items1.DataTable = await AsyncDbExecuter.DoTaskAsync("Lade pos steps.", () => new GetTableQuery(itemQuery).Execute(Connection));
                        RaisePropertyChanged("Items1");
                    }
                    else if(instruction.GetType() == typeof(DbProcRobotRow))
                    {
                        var procRobotRow = (DbProcRobotRow)instruction;
                        //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
                        Messenger.Default.Send(new TableSelectionChangedMessage(new ProcRobotTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString)), LSC1SimulatorViewModel.MessageToken);
                        var itemQuery = SQLStringGenerator.GetStepItemQuery(Job.JobNr, TablesEnum.tprocrobot, procRobotRow.Name, int.Parse(procRobotRow.Step));

                        Items1 = Tables.First(t => t.Table == TablesEnum.tprocrobot);
                        Items1.DataTable = await AsyncDbExecuter.DoTaskAsync("Lade proc robot steps.", () => new GetTableQuery(itemQuery).Execute(Connection));
                        RaisePropertyChanged("Items1");
                    }
                    else if(instruction.GetType() == typeof(DbProcLaserDataRow))
                    {
                        var procLaserRow = (DbProcLaserDataRow)instruction;
                        //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
                        Messenger.Default.Send(new TableSelectionChangedMessage(new ProcLaserTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString)), LSC1SimulatorViewModel.MessageToken);
                        var itemQuery = SQLStringGenerator.GetStepItemQuery(Job.JobNr, TablesEnum.tproclaserdata, procLaserRow.Name, int.Parse(procLaserRow.Step));

                        Items2 = Tables.First(t => t.Table == TablesEnum.tproclaserdata);
                        Items2.DataTable = await AsyncDbExecuter.DoTaskAsync("Lade proc laser steps.", () => new GetTableQuery(itemQuery).Execute(Connection));
                        RaisePropertyChanged("Items2");
                    }
                }
            }
            else if(msg.SelectedItem.GetType() == typeof(LSC1TreeViewJobStepNode))
            {
                var item = (LSC1TreeViewJobStepNode)msg.SelectedItem;
                
                Messenger.Default.Send(new TableSelectionChangedMessage(new JobDataTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString)), LSC1SimulatorViewModel.MessageToken);

                var itemQuery = SQLStringGenerator.GetStepItemQuery(Job.JobNr, TablesEnum.tjobdata, null, int.Parse(item.JobStepData.JobDataStepRow.Step));

                Items1 = Tables.First(t => t.Table == TablesEnum.tjobdata);
                Items1.DataTable = await AsyncDbExecuter.DoTaskAsync("Lade jobdata steps.", () => new GetTableQuery(itemQuery).Execute(Connection));                
            }
        }
    }
}
