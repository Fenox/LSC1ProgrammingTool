using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.Model;
using LSC1DatabaseEditor.ViewModel.DataStructures;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            Jobs = LSC1DatabaseFunctions.GetJobs();
            DeleteCommand = new RelayCommand<Window>(DeleteJob, (wnd) => SelectedJob != null);
        }

        void JobChanged()
        {
            LSC1DatabaseConnector db = new LSC1DatabaseConnector();

            //Laden von tprocdata
            string tprocdataQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'proc' GROUP BY `Name`";
            var procDataList = db.ReadRows<DbJobDataRow>(tprocdataQuery);
            var item1 = new TreeViewItem();
            item1.Text = "tproc";
            foreach (var item in procDataList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithProc = LSC1DatabaseFunctions.FindJobsWithName(item.Name);

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
            var posDataList = db.ReadRows<DbJobDataRow>(tposQuery);
            var item2 = new TreeViewItem();
            item2.Text = "tpos";
            foreach (var item in posDataList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithPos = LSC1DatabaseFunctions.FindJobsWithName(item.Name);

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
            var frameList = db.ReadRows<DbJobDataRow>(framesQuery);
            var item3 = new TreeViewItem();
            item3.Text = "tframe";
            foreach (var item in frameList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithFrame = LSC1DatabaseFunctions.FindJobsWithFrame(item.Frame);

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
            foreach (var item in TreeItems[0].SubItems.Where(it => it.Checked))
                selectedProc.Add(item.Text);

            var selectedPos = new List<string>();
            foreach (var item in TreeItems[1].SubItems.Where(it => it.Checked))
                selectedPos.Add(item.Text);

            var selectedFrames = new List<string>();
            foreach (var item in TreeItems[2].SubItems.Where(it => it.Checked))
                selectedFrames.Add(item.Text);

            LSC1DatabaseFunctions.DeleteJob(SelectedJob, selectedProc, selectedPos, selectedFrames.ToList());

            Messenger.Default.Send(new JobsChangedMessage());

            OfflineDatabase.UpdateAll();
            MessageBox.Show("Löschen war erfolgreich!");
            wnd.Close();
        }
    }
}
