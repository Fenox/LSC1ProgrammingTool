﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.LSC1CommonTool.Messages;
using LSC1DatabaseEditor.LSC1ProgramSimulator.Messages;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.Messages.Common;
using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseEditor.ViewModel.DataStructures;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;

namespace LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels
{
    public class LSC1StepDataGridVM : ViewModelBase
    {
        public TableViewModelBase items1Table = new ProcRobotTableViewModel(LSC1UserSettings.Instance.DBSettings);
        public TableViewModelBase Items1
        {
            get { return items1Table; }
            set
            {
                items1Table = value;
                RaisePropertyChanged("Items1");
            }
        }
        public TableViewModelBase items2Table = new ProcLaserTableViewModel(LSC1UserSettings.Instance.DBSettings);
        public TableViewModelBase Items2
        {
            get { return items2Table; }
            set
            {
                items2Table = value;
                RaisePropertyChanged("Items2");
            }
        }

        public DbJobNameRow Job { get; set; }


        ObservableCollection<TableViewModelBase> tables;
        public ObservableCollection<TableViewModelBase> Tables
        {
            get
            {
                return tables;
            }
            set
            {
                tables = value;
                RaisePropertyChanged("Tables");
            }
        }

        public LSC1StepDataGridVM()
        {
            Tables = new ObservableCollection<TableViewModelBase>();
            Tables.Add(new FrameTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new JobDataTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new JobNameTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new MoveParamTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new PosTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new ProcLaserTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new ProcPlcTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new ProcPulseTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new ProcRobotTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new ProcTurnTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new TableTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new ToolTableViewModel(LSC1UserSettings.Instance.DBSettings));
            Tables.Add(new TWTTableViewModel(LSC1UserSettings.Instance.DBSettings));

            Messenger.Default.Register<LSC1JobChangedMessage>(this, SimulatorViewModel.MessageToken, (msg) => Job = msg.NewJob);
            Messenger.Default.Register<SelectedTreeViewItemChanged>(this, SimulatorViewModel.MessageToken, PopulateGridItems);
            Messenger.Default.Register<DataGridCellValueChangedMessage>(this, SimulatorViewModel.MessageToken, OnCellValueChanged);
        }

        void OnCellValueChanged(DataGridCellValueChangedMessage msg)
        {
            //Create String that Updates a row 
            string updateString = "UPDATE " + msg.TableVM.Table +
                       " SET `" + msg.ColumnName + "` = '" + msg.NewValue +
                       "' WHERE ";

            for (int i = 0; i < msg.TableVM.DataTable.Columns.Count; i++)
            {
                string columnNamei = msg.TableVM.DataTable.Columns[i].ColumnName;

                if (msg.ColumnName == columnNamei)
                {
                    //Falls die letzte Spalte geändert wurde muss das AND wieder weg
                    if (i == msg.TableVM.DataTable.Columns.Count - 1)
                        updateString = updateString.Remove(updateString.Length - 4);

                    continue;
                }

                updateString += "`" + columnNamei + "` = '" + msg.Row.Row[i] + "'";

                if (i != msg.TableVM.DataTable.Columns.Count - 1)
                    updateString += " AND ";
            }

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);

            try
            {
                db.ExecuteQuery(updateString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            //Updaten der OfflineDatenbank, falls wichtiges geändert wurde
            if (msg.ColumnName.ToString() == "Name")
            {
                OfflineDatabase.UpdateTable(LSC1UserSettings.Instance.DBSettings, msg.TableVM.Table);
            }
            
            Messenger.Default.Send(new LSC1JobChangedMessage(Job), SimulatorViewModel.MessageToken);
        }

        void PopulateGridItems(SelectedTreeViewItemChanged msg)
        {
            if (msg.SelectedItem.GetType() == typeof(LSC1TreeViewPointLeaveItem))
            {
                var item = (LSC1TreeViewPointLeaveItem)msg.SelectedItem;

                foreach (var instruction in item.InstructionStepData.Instructions)
                {
                    if (instruction.GetType() == typeof(DbPosRow))
                    {
                        var posRow = (DbPosRow)instruction;
                        //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
                        Messenger.Default.Send(new TableSelectionChangedMessage(new PosTableViewModel(LSC1UserSettings.Instance.DBSettings)), SimulatorViewModel.MessageToken);
                        var itemQuery = SQLStringGenerator.GetStepItemQuery(Job.JobNr, TablesEnum.tpos, posRow.Name, 0);

                        LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);
                        Items1 = Tables.First(t => t.Table == TablesEnum.tpos);  
                        Items1.DataTable = db.GetTable(itemQuery);
                        RaisePropertyChanged("Items1");
                    }
                    else if(instruction.GetType() == typeof(DbProcRobotRow))
                    {
                        var procRobotRow = (DbProcRobotRow)instruction;
                        //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
                        Messenger.Default.Send(new TableSelectionChangedMessage(new ProcRobotTableViewModel(LSC1UserSettings.Instance.DBSettings)), SimulatorViewModel.MessageToken);
                        var itemQuery = SQLStringGenerator.GetStepItemQuery(Job.JobNr, TablesEnum.tprocrobot, procRobotRow.Name, int.Parse(procRobotRow.Step));

                        LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);
                        Items1 = Tables.First(t => t.Table == TablesEnum.tprocrobot);
                        Items1.DataTable = db.GetTable(itemQuery);
                        RaisePropertyChanged("Items1");
                    }
                    else if(instruction.GetType() == typeof(DbProcLaserDataRow))
                    {
                        var procLaserRow = (DbProcLaserDataRow)instruction;
                        //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
                        Messenger.Default.Send(new TableSelectionChangedMessage(new ProcLaserTableViewModel(LSC1UserSettings.Instance.DBSettings)), SimulatorViewModel.MessageToken);
                        var itemQuery = SQLStringGenerator.GetStepItemQuery(Job.JobNr, TablesEnum.tproclaserdata, procLaserRow.Name, int.Parse(procLaserRow.Step));

                        LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);
                        Items2 = Tables.First(t => t.Table == TablesEnum.tproclaserdata);
                        Items2.DataTable = db.GetTable(itemQuery);
                        RaisePropertyChanged("Items2");
                    }
                }
            }
            else if(msg.SelectedItem.GetType() == typeof(LSC1TreeViewJobStepNode))
            {
                var item = (LSC1TreeViewJobStepNode)msg.SelectedItem;
                
                Messenger.Default.Send(new TableSelectionChangedMessage(new JobDataTableViewModel(LSC1UserSettings.Instance.DBSettings)), SimulatorViewModel.MessageToken);

                var itemQuery = SQLStringGenerator.GetStepItemQuery(Job.JobNr, TablesEnum.tjobdata, null, int.Parse(item.JobStepData.JobDataStepRow.Step));

                LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);
                Items1 = Tables.First(t => t.Table == TablesEnum.tjobdata);
                Items1.DataTable = db.GetTable(itemQuery);                
            }
        }
    }
}
