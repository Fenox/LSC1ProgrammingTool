using System.Windows;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;

namespace LSC1DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for FindJobCorpsesWindow.xaml
    /// </summary>
    public partial class FindJobCorpsesWindow : Window
    {
        public FindJobCorpsesWindow()
        {
            InitializeComponent();

            DataContext = new FindJobCorpsesViewModel();
        }
    }
}
