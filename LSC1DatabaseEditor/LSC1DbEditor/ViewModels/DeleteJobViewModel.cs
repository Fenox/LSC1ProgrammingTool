using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseEditor.LSC1Database.Queries.Job;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DataStructures;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using NLog;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public class DeleteJobViewModel : ViewModelBase
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncExecuter = new LSC1AsyncDBTaskExecuter();
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        private static readonly LSC1DbFunctionCollection functions =
            new LSC1DbFunctionCollection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public ObservableCollection<TreeViewItem> TreeItems { get; set; } = new ObservableCollection<TreeViewItem>();

        public List<DbJobNameRow> Jobs { get; set; }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get => selectedJob;
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
            Initialize();
            DeleteCommand = new RelayCommand<Window>(DeleteJob, (wnd) => SelectedJob != null);
        }

        private async void Initialize()
        {
            Jobs = await AsyncExecuter.DoTaskAsync("Load Jobs", () => new GetJobsQuery().Execute(Connection).ToList());
        }

        private async void JobChanged()
        {
            TreeItems.Clear();

            //Laden des BaseFrames
            string baseFrameQuery = "SELECT FrameT1 FROM `twt` WHERE `WtId` = '" + SelectedJob.JobNr + "'";
            string baseFrame = await AsyncExecuter.DoTaskAsync("Lade alle BaseFrames", () =>
                                    new ReadRowsQuery<DbRow>(baseFrameQuery)
                                        .Execute(Connection)
                                        .Select(val => val.Values[0])
                                        .FirstOrDefault());

            //Berechnen der Anzahl der Vorkommen des BaseFrames in anderen Jobs
            string numOfBaseFrameOccurencesQuery = "SELECT * FROM `twt` WHERE FrameT1 ='" + baseFrame + "'";
            var baseFrameOccurences = await AsyncExecuter.DoTaskAsync("Lade BaseFrame", () =>
                new ReadRowsQuery<DbTwtRow>(numOfBaseFrameOccurencesQuery).Execute(Connection).ToList());
            TreeItems.Add(CreateBaseFrameTreeViewItems(baseFrame, baseFrameOccurences));

            //Laden von tprocdata
            //TODO: Refactor most of this into one method
            string tprocdataQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'proc' GROUP BY `Name`";
            var procDataList = await AsyncExecuter.DoTaskAsync("Lade tprocdata", () =>
                new ReadRowsQuery<DbJobDataRow>(tprocdataQuery).Execute(Connection).ToList());
            TreeItems.Add(CreateTreeViewItem("tproc", procDataList));

            //Laden aller tpos
            string tposQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'pos' GROUP BY `Name`";
            var posDataList = await AsyncExecuter.DoTaskAsync("Lade tpos", () =>
                new ReadRowsQuery<DbJobDataRow>(tposQuery).Execute(Connection).ToList());
            TreeItems.Add(CreateTreeViewItem("tpos", posDataList));


            //TODO: Laden aller turns zu job


            //TODO:Laden aller Pulses zu job

            //Laden aller frames
            string framesQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' GROUP BY `Frame`";
            var frameList = await AsyncExecuter.DoTaskAsync("Lade alle Frames", () =>
                new ReadRowsQuery<DbJobDataRow>(framesQuery).Execute(Connection).ToList());
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
            var occurenceIds = new List<TextItem>();
            foreach (DbTwtRow item in baseFrameOccurences)
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

        private static TreeViewItem CreateFrameTreeViewItems(IEnumerable<DbJobDataRow> frameList)
        {
            var item3 = new TreeViewItem
            {
                Text = "tframe"
            };
            foreach (DbJobDataRow item in frameList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithFrame = functions.FindJobsWithFrame(item.Frame);

                foreach (DbJobNameRow job in jobsWithFrame)
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
            foreach (DbJobDataRow item in procDataList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithProc = functions.FindJobsThatUseName(item.Name);

                foreach (DbJobNameRow job in jobsWithProc)
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
            //TODO: implement
            var selectedProc = new List<string>();
            if (TreeItems.Count > 1)
                selectedProc.AddRange(TreeItems[1].SubItems.Where(it => it.Checked).Select(item => item.Text));

            var selectedPos = new List<string>();
            if (TreeItems.Count > 2)
                selectedPos.AddRange(TreeItems[2].SubItems.Where(it => it.Checked).Select(item => item.Text));

            var selectedFrames = new List<string>();
            if (TreeItems.Count > 3)
                selectedFrames.AddRange(TreeItems[3].SubItems.Where(it => it.Checked).Select(item => item.Text));

            await AsyncExecuter.DoTaskAsync("Lösche Job" ,() =>
            {
                if (TreeItems.Count > 0)
                    new DeleteJobQuery(SelectedJob.Name).Execute(Connection);
            });

            Messenger.Default.Send(new JobsChangedMessage());

            OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            MessageBox.Show("Löschen war erfolgreich!");
            wnd.Close();
        }
    }
}
