using GalaSoft.MvvmLight.Command;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.LSC1JobRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using LSC1DatabaseEditor.LSC1Database.Queries.Job;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using MySql.Data.MySqlClient;
using NLog;

namespace LSC1DatabaseEditor.LSC1CommonTool.LoadJob
{
    public class LSC1LoadJobVM
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();
        private static Logger logger = LogManager.GetLogger("Usage");
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public List<DbJobNameRow> JobsCollection { get; set; }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get => selectedJob;
            set
            {
                selectedJob = value;
                LoadJobButtonCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<Window> LoadJobButtonCommand { get; set; }

        public LSC1LoadJobVM()
        {
            Initialize();
            LoadJobButtonCommand = new RelayCommand<Window>(OnLoadJobClick, (wnd) => SelectedJob != null);
        }

        private async void Initialize()
        {
            JobsCollection = await AsyncDbExecuter.DoTaskAsync("Lade Job Namen...",
                () => new GetJobsQuery().Execute(Connection).ToList());
        }

        private static void OnLoadJobClick(Window wnd)
        {
            wnd.DialogResult = true;
            wnd.Close();
        }
    }
}
