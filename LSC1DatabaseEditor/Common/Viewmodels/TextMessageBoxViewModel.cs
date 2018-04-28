using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace LSC1DatabaseEditor.Common.Viewmodels
{
    public class TextMessageBoxViewModel : ViewModelBase
    {
        public string Title { get; set; } = "MessageBox";
        public string LabelText { get; set; }
        public string TextBoxText { get; set; }

        public ICommand CloseSuccesfully { get; set; }

        public TextMessageBoxViewModel()
        {
            CloseSuccesfully = new RelayCommand<Window>(CloseWindowSuccessfully);
        }

        private static void CloseWindowSuccessfully(Window wnd)
        {
            wnd.DialogResult = true;
            wnd.Close();
        }
    }
}
