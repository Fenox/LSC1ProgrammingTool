using System.Windows;

namespace LSC1DatabaseEditor.LSC1CommonTool.LoadJob
{
    /// <summary>
    /// Interaction logic for LSC1LoadJobWindow.xaml
    /// </summary>
    public partial class LSC1LoadJobWindow : Window
    {
        public LSC1LoadJobWindow()
        {
            InitializeComponent();

            DataContext = new LSC1LoadJobVM();
        }
    }
}
