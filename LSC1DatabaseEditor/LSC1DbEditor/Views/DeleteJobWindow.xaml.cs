using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using System.Windows;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DataStructures;

namespace LSC1DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for DeleteJobWindow.xaml
    /// </summary>
    public partial class DeleteJobWindow : Window
    {
        public DeleteJobWindow()
        {
            InitializeComponent();

            DataContext = new DeleteJobViewModel();

            Messenger.Default.Register<TreeViewBuiltMessage>(this, DoStuff);
        }

        private void DoStuff(TreeViewBuiltMessage msg)
        {
            foreach (TreeViewItem item in treeView1.Items)
            {
                item.IsExpanded = true;
            }
        }
    }
}
