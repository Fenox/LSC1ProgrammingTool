using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using MySql.Data.MySqlClient;
using NLog;
using System.Windows;
using System.Windows.Controls;

namespace LSC1DatabaseEditor.DatabaseEditor.Views.SettingsControls
{
    /// <summary>
    /// Interaction logic for DatabaseSettingsControl.xaml
    /// </summary>
    public partial class DatabaseSettingsControl : UserControl
    {
        private static Logger logger = LogManager.GetLogger("Usage");

        public DatabaseSettingsControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        private bool TryConnectToDatabase()
        {
            try
            {
                LSC1UserSettings.Instance.Save();
                var connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
                connection.Open();
                MessageBox.Show("Verbindung konnte hergestellt werden.");
                return true;
            }
            catch (MySqlException e)
            {
                switch (e.ErrorCode)
                {
                    case 0:
                        MessageBox.Show("Keine Verbindung zur Datenbank möglich");
                        break;
                    case 1045:
                        MessageBox.Show("Ungeültiger Nutzername/Passwort");
                        break;
                    default:
                        MessageBox.Show("Unbekannter Fehler beim Verbinden mit der Datenbank.");
                        break;
                }

                logger.Error(e, "Faild to connect to database");
                return false;
            }
        }

        public string Database
        {
            get => LSC1UserSettings.Instance.DatabaseName;
            set => LSC1UserSettings.Instance.DatabaseName = value;
        }
        public string UID
        {
            get => LSC1UserSettings.Instance.DatabaseUID;
            set => LSC1UserSettings.Instance.DatabaseUID = value;
        }
        public string Server
        {
            get => LSC1UserSettings.Instance.DatabaseServer;
            set => LSC1UserSettings.Instance.DatabaseServer = value;
        }
        public string Password
        {
            get => LSC1UserSettings.Instance.DatabasePasswort;
            set => LSC1UserSettings.Instance.DatabasePasswort = value;
        }

        private void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            TryConnectToDatabase();
        }

        private void UseNewConnectionSettings_Click(object sender, RoutedEventArgs e)
        {
            if(TryConnectToDatabase())
            {
                Messenger.Default.Send(new ConnectionChangedMessage());
            }
        }
    }
}
