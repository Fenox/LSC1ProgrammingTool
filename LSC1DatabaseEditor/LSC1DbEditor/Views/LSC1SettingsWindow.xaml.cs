using System.Windows;
using System.ComponentModel;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;

namespace LSC1DatabaseEditor.DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for LSC1Settings.xaml
    /// </summary>
    public partial class LSC1SettingsWindow : Window
    {
        public LSC1SettingsWindow()
        {
            InitializeComponent();

            DataContext = new SettingsVM();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            LSC1UserSettings.Instance.Save();
            base.OnClosing(e);
        }
    }
}
