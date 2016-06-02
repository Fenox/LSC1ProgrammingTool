using LSC1DatabaseEditor.DatabaseEditor.ViewModels;
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
using System.Windows.Shapes;
using System.ComponentModel;

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
