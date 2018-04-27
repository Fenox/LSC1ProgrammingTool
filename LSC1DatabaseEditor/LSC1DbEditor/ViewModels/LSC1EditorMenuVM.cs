using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.DatabaseEditor.Views;
using LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseEditor.Views;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using NLog;
using System.Windows;
using System.Windows.Input;
using LSC1DatabaseEditor.LSC1Database;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public class LSC1EditorMenuVM
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();
        private static readonly Logger Logger = LogManager.GetLogger("Usage");
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);


        //Menu Datei Commands
        public ICommand CloseWindowCommand { get; set; }
        public ICommand OpenSettingsCommand { get; set; }
        public ICommand OpenVisualisationCommand { get; set; }

        //Menu Leichen Commands
        public ICommand FindJobCorpsesCommand { get; set; }
        public ICommand FindAndNameJobCorpsesCommand { get; set; }
        public ICommand FindProcCorpsesCommand { get; set; }
        public ICommand FindPosCorpsesCommand { get; set; }

        //Menu Erstellen Commands
        public RelayCommand CreateFrameCommand { get; set; }
        public RelayCommand CreatePosCommand { get; set; }
        public RelayCommand CreateProcCommand { get; set; }
        public RelayCommand CreateProcPlcCommand { get; set; }
        public RelayCommand CreateMoveparamCommand { get; set; }

        //Menu Job Commands
        public ICommand CopyJobCommand { get; set; }
        public ICommand DeleteJobCommand { get; set; }

        //Menu Versioning
        public ICommand SaveVersionCommand { get; set; }
        public ICommand LoadVersionCommand { get; set; }

        public LSC1EditorMenuVM()
        {
            //Menu Datei Commands
            CloseWindowCommand = new RelayCommand<Window>((wnd) => wnd.Close());
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenVisualisationCommand = new RelayCommand(OpenVisualizationWindow);

            //Menu Corpses Commands
            FindJobCorpsesCommand = new RelayCommand(OpenFindJobCorpses);
            FindAndNameJobCorpsesCommand = new RelayCommand(FindAndNameJobCorpses);
            FindProcCorpsesCommand = new RelayCommand(OpenFindProcCorpses);
            FindPosCorpsesCommand = new RelayCommand(OpenFindPosCorpses);

            //Menu Erstellen Commands
            CreateFrameCommand = new RelayCommand(CreateNewFrame);
            CreatePosCommand = new RelayCommand(CreateNewPos);
            CreateProcCommand = new RelayCommand(CreateNewProc);
            CreateProcPlcCommand = new RelayCommand(CreateNewProcPlc);
            CreateMoveparamCommand = new RelayCommand(CreateNewMoveparam);

            //Menu Job Commands
            CopyJobCommand = new RelayCommand(OpenCopyJobWindow);
            DeleteJobCommand = new RelayCommand(OpenDeleteJobWindow);

            //Menu Versioning
            SaveVersionCommand = new RelayCommand(SaveVersion);
            LoadVersionCommand = new RelayCommand(LoadVersion);
        }

        private static void OpenVisualizationWindow()
        {
            var form = new LSC1SimulatorWindow();
            form.Show();
        }

        private void OpenSettings()
        {
            var wnd = new LSC1SettingsWindow();
            wnd.Show();
        }

        public void OpenFindJobCorpses()
        {
            var windows = new FindJobCorpsesWindow();
            windows.Show();
        }

        private static async void FindAndNameJobCorpses()
        {
            await AsyncDbExecuter.DoTaskAsync("Benenne Job Orphan Ids...", () =>
            {
                var jobOrphansIds = new FindJobOrphansQuery().Execute(Connection);

                foreach (string jobOrphanId in jobOrphansIds)
                {
                    //Eintrag in JobName erstellen
                    const string insertQuery = "INSERT INTO `tjobname` VALUES('@NewId', 'Job@NewId')";
                    new NonReturnSimpleQuery(insertQuery,
                        new MySqlParameter("NewId", jobOrphanId))
                            .Execute(Connection);
                }
            });

            Messenger.Default.Send(new JobsChangedMessage());
        }

        public void OpenFindProcCorpses()
        {
            var windows = new FindProcCorpsesWindow();
            windows.Show();
        }


        public void OpenFindPosCorpses()
        {
            var windows = new FindPosCorpsesWindow();
            windows.Show();
        }

        private static async void CreateNewFrame()
        {
            var form = new MyTextMessageBox();
            var dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer Frame";
            dataContext.LabelText = "Geben sie den Namen des neuen Frames ein";

            var result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                await AsyncDbExecuter.DoTaskAsync("Füge neuen ProcRobot- und ProcLaser-Parameter ein...",
                   () => new InsertQuery(new DbFrameRow { Name = dataContext.TextBoxText }).Execute(Connection));

                OfflineDatabase.UpdateAllFrameNames(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            }

            Logger.Info("Created new frame {0}", dataContext.TextBoxText);
        }

        private static async void CreateNewPos()
        {
            var form = new MyTextMessageBox();
            var dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer Pos Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen Pos Eintrages ein";

            var result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                await AsyncDbExecuter.DoTaskAsync("Füge neuen ProcRobot- und ProcLaser-Parameter ein...",
                   () => new InsertQuery(new DbPosRow { Name = dataContext.TextBoxText }).Execute(Connection));
                OfflineDatabase.UpdateAllPosNames(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            }

            Logger.Info("Created new pos {0}", dataContext.TextBoxText);
        }

        public static async void CreateNewProc()
        {
            var form = new MyTextMessageBox();
            if (!(form.DataContext is TextMessageBoxViewModel dataContext)) return;

            dataContext.Title = "Neuer Proc Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen Proc Eintrages ein";

            var result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                await AsyncDbExecuter.DoTaskAsync("Füge neuen ProcRobot- und ProcLaser-Parameter ein...",
                   () =>
                   {
                       new InsertQuery(new DbProcRobotRow() {Name = dataContext.TextBoxText})
                           .Execute(Connection);
                       new InsertQuery(new DbProcLaserDataRow() { Name = dataContext.TextBoxText })
                           .Execute(Connection);
                   });
               
                OfflineDatabase.UpdateAllProcNames(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            }

            Logger.Info("Created new proc {0}", dataContext.TextBoxText);
        }

        private static async void CreateNewProcPlc()
        {
            var form = new MyTextMessageBox();
            var dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer ProcPLC Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen ProcPLC Eintrages ein";

            var result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                await AsyncDbExecuter.DoTaskAsync("Füge neuen ProcPlc-Parameter ein...",
                    () => new InsertQuery(new DbProcPlcRow() { Name = dataContext.TextBoxText }).Execute(Connection));
                OfflineDatabase.UpdateAllProcPLCNames(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            }

            Logger.Info("Created new procplc {0}", dataContext.TextBoxText);
        }

        private static async void CreateNewMoveparam()
        {
            var form = new MyTextMessageBox();
            var dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer MoveParam Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen MoveParam Eintrages ein";

            var result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                await AsyncDbExecuter.DoTaskAsync("Füge neuen Move-Parameter ein...", 
                    () =>  new InsertQuery(new DbMoveParamRow() {Name = dataContext.TextBoxText}).Execute(Connection));

                OfflineDatabase.UpdateAllMoveParamNames(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            }

            Logger.Info("Created new moveparam {0}", dataContext.TextBoxText);
        }

        public void OpenCopyJobWindow()
        {
            //TODO: resolve this hack
            //HACK neues fenster sollte folgendermaßen erstellt werden http://stackoverflow.com/questions/25845689/opening-new-window-in-mvvm-wpf
            var window = new CopyJobWindow(); 
            window.Show();
        }

        public void OpenDeleteJobWindow()
        {
            var window = new DeleteJobWindow();
            window.Show();
        }

        public async void SaveVersion()
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "SQL Files(*.sql)|*.sql"
            };

            if (dialog.ShowDialog() != true) return;

            await AsyncDbExecuter.DoTaskAsync("Exportiere Datenbank", () =>
            {
                using (var connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString))
                using (var cmd = new MySqlCommand())
                using (var mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = connection;
                    connection.Open();
                    mb.ExportToFile(dialog.FileName);
                    connection.Close();
                }
            });

            MessageBox.Show("Datenbank erfolgreich gespeichert.");
            Logger.Info("Saved database to", dialog.FileName);
        }

        public async void LoadVersion()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "SQL Files(*.sql)|*.sql"
            };

            if (dialog.ShowDialog() != true) return;

            await AsyncDbExecuter.DoTaskAsync("Importiere Datenbank", () =>
            {
                using (var connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString))
                using (var cmd = new MySqlCommand())
                using (var mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = connection;
                    connection.Open();
                    mb.ImportFromFile(dialog.FileName);
                    connection.Close();
                }
            });

            MessageBox.Show("Datebank erfolgreich importiert.");
            Logger.Info("Loaded database from", dialog.FileName);
        }
    }
}
