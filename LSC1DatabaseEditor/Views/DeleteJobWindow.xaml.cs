using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        void DoStuff(TreeViewBuiltMessage msg)
        {
            foreach (TreeViewItem item in treeView1.Items)
            {
                item.IsExpanded = true;
            }
        }
    }
}
