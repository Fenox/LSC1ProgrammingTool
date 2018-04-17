﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.ViewModel.DataStructures;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1Library;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LSC1DatabaseEditor.ViewModel
{
    public class DeleteJobViewModel : ViewModelBase
    {
        public ObservableCollection<TreeViewItem> TreeItems { get; set; } = new ObservableCollection<TreeViewItem>();

        public List<DbJobNameRow> Jobs { get; set; }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get { return selectedJob; }
            set
            {
                selectedJob = value;
                JobChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<Window> DeleteCommand { get; set; }

        public DeleteJobViewModel()
        {
            Jobs = LSC1DatabaseFacade.GetJobs();
            DeleteCommand = new RelayCommand<Window>(DeleteJob, (wnd) => SelectedJob != null);
        }

        void JobChanged()
        {
            TreeItems.Clear();
                        
            //Laden der Frames
            string baseFrameQuery = "SELECT FrameT1 FROM `twt` WHERE `WtId` = '" + SelectedJob.JobNr + "'";
            string baseFrame = LSC1DatabaseFacade.Read<DbRow>(LSC1UserSettings.Instance.DBSettings, baseFrameQuery)
                .Select(val => val.Values[0])
                .First();

            var item0 = new TreeViewItem
            {
                Text = "base frame"
            };

            //Berechnen der Vorkommen des Frames
            string numOfBaseFrameOccurencesQuery = "SELECT * FROM `twt` WHERE FrameT1 ='" + baseFrame + "'";
            var baseFrameOccurences = LSC1DatabaseFacade.Read<DbTwtRow>(LSC1UserSettings.Instance.DBSettings, numOfBaseFrameOccurencesQuery);

            List<TextItem> occurenceIds = new List<TextItem>();
            foreach (var item in baseFrameOccurences)
            {
                occurenceIds.Add(new TextItem()
                {
                    Text = item.WtId
                });
            }

            item0.SubItems.Add(new CheckableItemWithSub()
            {
                Text = baseFrame,
                Checked = baseFrameOccurences.Count() == 0,
                SubItems = new ObservableCollection<TextItem>(occurenceIds)               
            });
            TreeItems.Add(item0);

            //Laden von tprocdata
            //TODO: Refactor most of this into one method
            string tprocdataQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'proc' GROUP BY `Name`";
            var procDataList = LSC1DatabaseFacade.Read<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, tprocdataQuery);

            var item1 = new TreeViewItem
            {
                Text = "tproc"
            };
            foreach (var item in procDataList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithProc = LSC1DatabaseFacade.FindJobsThatUseName(item.Name);

                foreach (var job in jobsWithProc)
                {
                    subSubItems.Add(new TextItem()
                    {
                        Text = job.Name,
                    });
                }

                item1.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item.Name,
                    SubItems = subSubItems,
                    Checked = jobsWithProc.Count <= 1 ? true : false
                });
            }
            TreeItems.Add(item1);

            //Laden aller tpos
            string tposQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'pos' GROUP BY `Name`";
            var posDataList = LSC1DatabaseFacade.Read<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, tposQuery);
            var item2 = new TreeViewItem
            {
                Text = "tpos"
            };
            foreach (var item in posDataList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithPos = LSC1DatabaseFacade.FindJobsThatUseName(item.Name);

                foreach (var job in jobsWithPos)
                {
                    subSubItems.Add(new TextItem()
                    {
                        Text = job.Name,
                    });
                }

                item2.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item.Name,
                    SubItems = subSubItems,
                    Checked = jobsWithPos.Count <= 1 ? true : false
                });
            }
            TreeItems.Add(item2);

            //Laden aller frames
            string framesQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' GROUP BY `Frame`";
            var frameList = LSC1DatabaseFacade.Read<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, framesQuery);
            var item3 = new TreeViewItem
            {
                Text = "tframe"
            };
            foreach (var item in frameList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithFrame = LSC1DatabaseFacade.FindJobsWithFrame(item.Frame);

                foreach (var job in jobsWithFrame)
                {
                    subSubItems.Add(new TextItem()
                    {
                        Text = job.Name,
                    });
                }

                item3.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item.Frame,
                    SubItems = subSubItems,
                    Checked = jobsWithFrame.Count <= 1 ? true : false
                });
            }
            TreeItems.Add(item3);

            Messenger.Default.Send(new TreeViewBuiltMessage());
        }

        public void DeleteJob(Window wnd)
        {
            var selectedProc = new List<string>();
            if(TreeItems.Count > 1)
                foreach (var item in TreeItems[1].SubItems.Where(it => it.Checked))
                    selectedProc.Add(item.Text);
            
            var selectedPos = new List<string>();
            if (TreeItems.Count > 2)
                foreach (var item in TreeItems[2].SubItems.Where(it => it.Checked))
                    selectedPos.Add(item.Text);

            var selectedFrames = new List<string>();
            if (TreeItems.Count > 3)
                foreach (var item in TreeItems[3].SubItems.Where(it => it.Checked))
                    selectedFrames.Add(item.Text);

            if(TreeItems.Count > 0)
                LSC1DatabaseFacade.DeleteJob(SelectedJob, selectedProc, selectedPos, selectedFrames, TreeItems[0].SubItems[0].Checked);
            else
                LSC1DatabaseFacade.DeleteJob(SelectedJob, selectedProc, selectedPos, selectedFrames, false);


            Messenger.Default.Send(new JobsChangedMessage());

            OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings);
            MessageBox.Show("Löschen war erfolgreich!");
            wnd.Close();
        }
    }
}