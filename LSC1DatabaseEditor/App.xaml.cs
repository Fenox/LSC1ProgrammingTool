using LSC1Library;
using System.Windows;

namespace LSC1DatabaseEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings);
        }
        
    }
}
