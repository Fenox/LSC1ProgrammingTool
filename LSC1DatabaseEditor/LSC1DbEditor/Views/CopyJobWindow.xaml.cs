using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;
using LSC1DatabaseEditor.Common.Messages;

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
