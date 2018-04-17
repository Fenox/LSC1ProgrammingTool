using ExtensionsAndCodeSnippets.RandomTools;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.DatabaseEditor.Views;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseEditor.Views;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1Library;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Input;

namespace LSC1DatabaseEditor.DatabaseEditor.ViewModels
{
    public class LSC1EditorMenuVM
    {
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

        void OpenVisualizationWindow()
        {
            LSC1SimulatorWindow form = new LSC1SimulatorWindow();
            form.Show();
        }

        void OpenSettings()
        {
            LSC1SettingsWindow wnd = new LSC1SettingsWindow();
            wnd.Show();
        }

        public void OpenFindJobCorpses()
        {
            var windows = new FindJobCorpsesWindow();
            windows.Show();
        }

        void FindAndNameJobCorpses()
        {
            var jobCorpsIds = LSC1DatabaseFacade.FindJobCorpses();

            foreach (var jobCorpsId in jobCorpsIds)
            {
                //Eintrag in JobName erstellen
                string insertQuery = "INSERT INTO `tjobname` VALUES('" + jobCorpsId + "', 'Job" + jobCorpsId + RandomSnippets.RandomString(1) + "')";
                LSC1DatabaseFacade.SimpleQuery(insertQuery);
            }


            Messenger.Default.Send(new JobsChangedMessage() { });
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

        void CreateNewFrame()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer Frame";
            dataContext.LabelText = "Geben sie den Namen des neuen Frames ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseFacade.Insert(LSC1UserSettings.Instance.DBSettings, new DbFrameRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllFrameNames(LSC1UserSettings.Instance.DBSettings);
            }
        }

        void CreateNewPos()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer Pos Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen Pos Eintrages ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseFacade.Insert(LSC1UserSettings.Instance.DBSettings, new DbPosRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllPosNames(LSC1UserSettings.Instance.DBSettings);
            }
        }

        void CreateNewProc()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer Proc Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen Proc Eintrages ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseFacade.Insert(LSC1UserSettings.Instance.DBSettings, new DbProcRobotRow() { Name = dataContext.TextBoxText });
                LSC1DatabaseFacade.Insert(LSC1UserSettings.Instance.DBSettings, new DbProcLaserDataRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllProcNames(LSC1UserSettings.Instance.DBSettings);
            }
        }

        void CreateNewProcPlc()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer ProcPLC Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen ProcPLC Eintrages ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseFacade.Insert(LSC1UserSettings.Instance.DBSettings, new DbProcPlcRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllProcPLCNames(LSC1UserSettings.Instance.DBSettings);
            }
        }

        void CreateNewMoveparam()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer MoveParam Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen MoveParam Eintrages ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseFacade.Insert(LSC1UserSettings.Instance.DBSettings, new DbMoveParamRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllMoveParamNames(LSC1UserSettings.Instance.DBSettings);
            }
        }

        public void OpenCopyJobWindow()
        {
            var window = new CopyJobWindow(); //HACK neues fenster sollte folgendermaßen erstellt werden http://stackoverflow.com/questions/25845689/opening-new-window-in-mvvm-wpf
            window.Show();
        }

        public void OpenDeleteJobWindow()
        {
            var window = new DeleteJobWindow();
            window.Show();
        }

        public void SaveVersion()
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "SQL Files(*.sql)|*.sql"
            };

            if (dialog.ShowDialog() == true)
            {
                using (MySqlConnection connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand())
                using (MySqlBackup mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = connection;
                    connection.Open();
                    mb.ExportToFile(dialog.FileName);
                    connection.Close();
                    MessageBox.Show("Datenbank erfolgreich gespeichert.");
                }
            }
        }

        public void LoadVersion()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "SQL Files(*.sql)|*.sql"
            };

            if(dialog.ShowDialog() == true)
            {
                using (MySqlConnection connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString))
                using (MySqlCommand cmd = new MySqlCommand())
                using (MySqlBackup mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = connection;
                    connection.Open();
                    mb.ImportFromFile(dialog.FileName);
                    connection.Close();
                    MessageBox.Show("Datebank erfolgreich importiert.");
                }
            }
        }
    }
}
