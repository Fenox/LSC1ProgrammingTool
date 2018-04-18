using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Controller;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.ViewModel.DataStructures;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1Library;
using NLog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LSC1DatabaseEditor.ViewModel
{
    public class DeleteJobViewModel : ViewModelBase
    {
        private static LSC1AsyncTaskExecuter asyncExecuter = new LSC1AsyncTaskExecuter();
        private static Logger logger = LogManager.GetLogger("Usage");

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

        private async void JobChanged()
        {
            TreeItems.Clear();

            //Laden des BaseFrames
            string baseFrameQuery = "SELECT FrameT1 FROM `twt` WHERE `WtId` = '" + SelectedJob.JobNr + "'";
            string baseFrame = (await asyncExecuter.DoTaskAsync("Lade alle BaseFrames", 
                LSC1DatabaseFacade.ReadAsync<DbRow>(LSC1UserSettings.Instance.DBSettings, baseFrameQuery)))
                .Select(val => val.Values[0])
                .FirstOrDefault();

            //Berechnen der Anzahl der Vorkommen des BaseFrames in anderen Jobs
            string numOfBaseFrameOccurencesQuery = "SELECT * FROM `twt` WHERE FrameT1 ='" + baseFrame + "'";
            var baseFrameOccurences = (await asyncExecuter.DoTaskAsync("Lade BaseFrame", 
                LSC1DatabaseFacade.ReadAsync<DbTwtRow>(LSC1UserSettings.Instance.DBSettings, numOfBaseFrameOccurencesQuery)));
            TreeItems.Add(CreateBaseFrameTreeViewItems(baseFrame, baseFrameOccurences));

            //Laden von tprocdata
            //TODO: Refactor most of this into one method
            string tprocdataQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'proc' GROUP BY `Name`";
            var procDataList = (await asyncExecuter.DoTaskAsync("Lade tprocdata", 
                LSC1DatabaseFacade.ReadAsync<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, tprocdataQuery)));
            TreeItems.Add(CreateTreeViewItem("tproc", procDataList));

            //Laden aller tpos
            string tposQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'pos' GROUP BY `Name`";
            var posDataList = (await asyncExecuter.DoTaskAsync("Lade tpos", 
                LSC1DatabaseFacade.ReadAsync<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, tposQuery)));
            TreeItems.Add(CreateTreeViewItem("tpos", posDataList));

            //Laden aller turns zu job


            //Laden aller Pulses zu job

            //Laden aller frames
            string framesQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' GROUP BY `Frame`";
            var frameList = (await asyncExecuter.DoTaskAsync("Lade alle Frames", 
                LSC1DatabaseFacade.ReadAsync<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, framesQuery)));
            TreeViewItem item3 = CreateFrameTreeViewItems(frameList);
            TreeItems.Add(item3);

            Messenger.Default.Send(new TreeViewBuiltMessage());
        }

        private static TreeViewItem CreateBaseFrameTreeViewItems(string baseFrame, List<DbTwtRow> baseFrameOccurences)
        {
            var item0 = new TreeViewItem
            {
                Text = "base frame"
            };
            List<TextItem> occurenceIds = new List<TextItem>();
            foreach (var item in baseFrameOccurences)
            {
                occurenceIds.Add(new TextItem()
                {
                    Text = item.WtId
                });
            }
            if (baseFrameOccurences.Count > 0)
            {
                item0.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = baseFrame,
                    Checked = baseFrameOccurences.Count() == 1,
                    SubItems = new ObservableCollection<TextItem>(occurenceIds)
                });
            }
            return item0;
        }

        private static TreeViewItem CreateFrameTreeViewItems(List<DbJobDataRow> frameList)
        {
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
                    Checked = jobsWithFrame.Count <= 1
                });
            }

            return item3;
        }

        private static TreeViewItem CreateTreeViewItem(string name, List<DbJobDataRow> procDataList)
        {
            var item1 = new TreeViewItem
            {
                Text = name
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
                    Checked = jobsWithProc.Count <= 1
                });
            }

            return item1;
        }

        public async void DeleteJob(Window wnd)
        {
            var selectedProc = new List<string>();
            if (TreeItems.Count > 1)
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

            await asyncExecuter.DoTaskAsync("Lösche Job" ,() =>
            {
                if (TreeItems.Count > 0)
                    LSC1DatabaseFacade.DeleteJob(SelectedJob, selectedProc, selectedPos, selectedFrames, TreeItems[0].SubItems[0].Checked);
                else
                    LSC1DatabaseFacade.DeleteJob(SelectedJob, selectedProc, selectedPos, selectedFrames, false);
            });

            Messenger.Default.Send(new JobsChangedMessage());

            OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings);
            MessageBox.Show("Löschen war erfolgreich!");
            wnd.Close();
        }
    }
}
