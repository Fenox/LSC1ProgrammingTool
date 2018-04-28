using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using System.Windows;
using System.Windows.Controls;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;

namespace LSC1DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for CopyJobWindow.xaml
    /// </summary>
    public partial class CopyJobWindow : Window
    {
        private CopyJobViewModel viewModel = new CopyJobViewModel();

        public CopyJobWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Messenger.Default.Send(new TextChangedMessage()
            {
                NewText = ((TextBox)e.Source).Text,
            });
        }
    }
}
