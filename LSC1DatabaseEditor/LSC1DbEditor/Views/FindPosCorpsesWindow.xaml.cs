using System.Windows;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;

namespace LSC1DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for FindPosCorpsesWindow.xaml
    /// </summary>
    public partial class FindPosCorpsesWindow : Window
    {
        public FindPosCorpsesWindow()
        {
            InitializeComponent();
            DataContext = new FindPosCorpsesViewModel();
        }
    }
}
