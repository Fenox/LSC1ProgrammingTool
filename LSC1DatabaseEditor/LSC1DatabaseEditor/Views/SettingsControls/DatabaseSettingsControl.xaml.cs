using GalaSoft.MvvmLight.Command;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void TryConnectToDatabase()
        {
            try
            {
                LSC1UserSettings.Instance.Save();
                var connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
                connection.Open();
                MessageBox.Show("Verbindung konnte hergestellt werden.");
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
            }
        }

        public string Database
        {
            get { return LSC1UserSettings.Instance.DatabaseName; }
            set
            {
                LSC1UserSettings.Instance.DatabaseName = value;
            }
        }
        public string UID
        {
            get { return LSC1UserSettings.Instance.DatabaseUID; }
            set
            {
                LSC1UserSettings.Instance.DatabaseUID = value;
            }
        }
        public string Server
        {
            get { return LSC1UserSettings.Instance.DatabaseServer; }
            set
            {
                LSC1UserSettings.Instance.DatabaseServer = value;
            }
        }
        public string Password
        {
            get { return LSC1UserSettings.Instance.DatabasePasswort; }
            set
            {
                LSC1UserSettings.Instance.DatabasePasswort = value;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TryConnectToDatabase();
        }
    }
}
