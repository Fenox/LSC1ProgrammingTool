using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace LSC1DatabaseEditor.ViewModel
{
    public class TextMessageBoxViewModel : ViewModelBase
    {
        public string Title { get; set; } = "MessageBox";
        public string LabelText { get; set; }
        public string TextBoxText { get; set; }

        public ICommand CloseSuccesfully { get; set; }

        //public DialogResultEnum DialogResult { get; set; } = DialogResultEnum.Failed;

        public TextMessageBoxViewModel()
        {
            CloseSuccesfully = new RelayCommand<Window>(CloseWindowSuccessfully);
        }

        void CloseWindowSuccessfully(Window wnd)
        {
            wnd.DialogResult = true;
            wnd.Close();
        }
    }
}
