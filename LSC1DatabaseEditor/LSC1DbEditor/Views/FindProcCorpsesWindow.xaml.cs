using System.Windows;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;

namespace LSC1DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for FindProcCorpsesWindow.xaml
    /// </summary>
    public partial class FindProcCorpsesWindow : Window
    {
        public FindProcCorpsesWindow()
        {
            InitializeComponent();
            DataContext = new FindProcCorpsesViewModel();
        }
    }
}
