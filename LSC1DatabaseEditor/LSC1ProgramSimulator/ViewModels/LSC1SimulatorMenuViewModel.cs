using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using LSC1DatabaseEditor.LSC1CommonTool.LoadJob;
using LSC1DatabaseEditor.Messages;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels
{
    public class LSC1SimulatorMenuViewModel
    {
        public ICommand LoadModelCommand { get; set; }
        public ICommand LoadJobCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }

        public LSC1SimulatorMenuViewModel()
        {
            LoadJobCommand = new RelayCommand(OnLoadJobClick);
            LoadModelCommand = new RelayCommand(OnLoadModelClick);
            CloseWindowCommand = new RelayCommand<Window>((wnd) => wnd.Close());
        }

        void OnLoadJobClick()
        {
            LSC1LoadJobWindow wnd = new LSC1LoadJobWindow();
            bool? result = wnd.ShowDialog();

            if(result.HasValue && result.Value)
            {
                var selectedJob = ((LSC1LoadJobVM)wnd.DataContext).SelectedJob;

                //Wird an Treeview und Grid gesendet
                Messenger.Default.Send(new LSC1JobChangedMessage(selectedJob), LSC1SimulatorViewModel.MessageToken);
            }
        }

        void OnLoadModelClick()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? dialogResult = dialog.ShowDialog().HasValue;
            if (dialogResult.HasValue && dialogResult.Value && dialog.FileName.Length > 0)
            {
                Messenger.Default.Send(new Model3DLoadedMessage(dialog.FileName), LSC1SimulatorViewModel.MessageToken);
            }
        }
    }
}
