using GalaSoft.MvvmLight.Command;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1JobRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace LSC1DatabaseEditor.LSC1CommonTool.LoadJob
{
    public class LSC1LoadJobVM
    {
        public List<DbJobNameRow> JobsCollection { get; set; }

        private DbJobNameRow m_SelectedJob;
        public DbJobNameRow SelectedJob
        {
            get { return m_SelectedJob; }
            set
            {
                m_SelectedJob = value;
                LoadJobButtonCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<Window> LoadJobButtonCommand { get; set; }

        public LSC1LoadJobVM()
        {
            JobsCollection = LSC1DatabaseFunctions.GetJobs(LSC1UserSettings.Instance.DBSettings);

            LoadJobButtonCommand = new RelayCommand<Window>(OnLoadJobClick, (wnd) => SelectedJob != null);
        }

        void OnLoadJobClick(Window wnd)
        {
            wnd.DialogResult = true;
            wnd.Close();
        }
    }
}
