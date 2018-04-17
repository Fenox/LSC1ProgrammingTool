using GalaSoft.MvvmLight;
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
    public class CopyJobViewModel : ViewModelBase
    {
        public List<DbJobNameRow> Jobs { get; set; }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get { return selectedJob; }
            set
            {
                selectedJob = value;
                JobNameChanged();
                RaisePropertyChanged("SelectedJob");
                CopyJobCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<Window> CopyJobCommand { get; set; }

        private string newJobName;
        public string NewJobName
        {
            get { return newJobName; }
            set
            {
                newJobName = value;
                RaisePropertyChanged("NewJobName");
                CopyJobCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<TreeViewItem> TreeItems { get; set; } = new ObservableCollection<TreeViewItem>();

        public CopyJobViewModel()
        {
            CopyJobCommand = new RelayCommand<Window>(CopyJob, (wnd) => SelectedJob != null && NewJobName != null && NewJobName.Length > 0);
            Jobs = LSC1DatabaseFacade.GetJobs();

            Messenger.Default.Register<TextChangedMessage>(this, (m) =>
            {
                NewJobName = m.NewText;
                CopyJobCommand.RaiseCanExecuteChanged();
            });
        }

        void JobNameChanged()
        {
            TreeItems.Clear();

            //Laden der twt Frames
            string twtQueryFrame1 = "SELECT FrameT1 FROM `twt` WHERE `WtId` = '" + SelectedJob.JobNr + "'";

            var frame1 = LSC1DatabaseFacade.Read<DbRow>(LSC1UserSettings.Instance.DBSettings, twtQueryFrame1)
                .Select(val => val.Values[0]);

            //Nur wenn ein Eintrag zum kopieren besteht.

            var itemnNeg1 = new TreeViewItem
            {
                Text = "twt Frame"
            };

            itemnNeg1.SubItems.Add(new CheckableItemWithSub()
            {
                Text = frame1.Count() > 0 ? frame1.First() : "Neuer Eintrag wird erstellt",
                Checked = true
            });    

            TreeItems.Add(itemnNeg1);

            //Laden der Frames
            string tframesQuery = "SELECT Frame FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' GROUP By `Frame`";
            var frameDataList = LSC1DatabaseFacade.Read<DbRow>(LSC1UserSettings.Instance.DBSettings, tframesQuery)
                .Select(val => val.Values[0]);
            var item0 = new TreeViewItem
            {
                Text = "tframe"
            };
            foreach (var item in frameDataList)
            {
                item0.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item,
                    Checked = true
                });
            }
            TreeItems.Add(item0);


            //Laden von tprocdata
            string tprocdataQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'proc' GROUP By `Name`";
            var procDataList = LSC1DatabaseFacade.Read<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, tprocdataQuery);
            var item1 = new TreeViewItem
            {
                Text = "tproc"
            };
            foreach (var item in procDataList)
            {
                item1.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item.Name,
                    Checked = true
                });
            }
            TreeItems.Add(item1);

            //Laden aller tpos
            string tposQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'pos' GROUP By `Name`";
            var posDataList = LSC1DatabaseFacade.Read<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, tposQuery);
            var item2 = new TreeViewItem
            {
                Text = "tpos"
            };
            foreach (var item in posDataList)
            {
                item2.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item.Name,
                    Checked = true
                });
            }
            TreeItems.Add(item2);
        }

        public void CopyJob(Window wnd)
        {
            var keepFrame = new Dictionary<string, bool>();
            foreach (var item in TreeItems[1].SubItems)
                keepFrame.Add(item.Text, !item.Checked);

            var keepProc = new Dictionary<string, bool>();
            foreach (var item in TreeItems[2].SubItems)
                keepProc.Add(item.Text, !item.Checked);

            var keepPos = new Dictionary<string, bool>();
            foreach (var item in TreeItems[3].SubItems)
                keepPos.Add(item.Text, !item.Checked);


            LSC1DatabaseFacade.CopyJob(NewJobName, SelectedJob.Name, keepPos, keepProc, keepFrame, !TreeItems[0].SubItems[0].Checked);

            Messenger.Default.Send(new JobsChangedMessage() { AddedJob = NewJobName });

            OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings);
            MessageBox.Show("Kopieren war erfolgreich!");
            wnd.Close();
        }
    }
}
