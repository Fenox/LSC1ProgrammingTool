using GalaSoft.MvvmLight;
using LSC1DatabaseEditor.DatabaseEditor.Views.SettingsControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace LSC1DatabaseEditor.DatabaseEditor.ViewModels
{
    public class SettingsVM : ViewModelBase
    {
        public UserControl SettingsControl { get; set; } = new DatabaseSettingsControl();

        public List<SettingsControlItemVM> SettingsItems { get; set; } = new List<SettingsControlItemVM>();

        private SettingsControlItemVM selectedSettignsControl;
        public SettingsControlItemVM SelectedSettingsControl
        {
            get
            {
                return selectedSettignsControl;
            }
            set
            {
                selectedSettignsControl = value;
                RaisePropertyChanged("SelectedSettingsControl");

                SettingsControl = selectedSettignsControl.SettingsControl;
                RaisePropertyChanged("SettingsControl");
            }
        }

        public SettingsVM()
        {
            SettingsItems.Add(new SettingsControlItemVM()
            {
                SettingsControl = new DatabaseSettingsControl(),
                Name = "Datenbank"
            });

            SettingsItems.Add(new SettingsControlItemVM()
            {
                SettingsControl = new VisualisationSettingsControl(),
                Name ="Visualisierung"
            });

            SelectedSettingsControl = SettingsItems[0];
        }
    }

    public class SettingsControlItemVM
    {
        public UserControl SettingsControl { get; set; }
        public string Name { get; set; }
    }
}
