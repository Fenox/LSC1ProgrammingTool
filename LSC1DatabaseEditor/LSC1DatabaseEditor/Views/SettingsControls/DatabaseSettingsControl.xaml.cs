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
        public DatabaseSettingsControl()
        {
            InitializeComponent();

            DataContext = this;
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
    }
}
