using System.Windows;
using LSC1DatabaseEditor.Common.Viewmodels;

namespace LSC1DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for MyTextMessageBox.xaml
    /// </summary>
    public partial class MyTextMessageBox : Window
    {
        public MyTextMessageBox()
        {
            InitializeComponent();
            DataContext = new TextMessageBoxViewModel();
        }
    }
}
