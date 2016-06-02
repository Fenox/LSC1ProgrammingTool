using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.LSC1CommonTool.Messages;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseLibrary;
using LSC1Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LSC1DatabaseEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = viewModel;

            Messenger.Default.Register<TableSelectionChangedMessage>(this, ChangeTableResource);
        }

        private void ChangeTableResource(TableSelectionChangedMessage msg)
        {
            contentDataGridControl.Content = FindResource(msg.SelectedTable.DataGridName);
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var selectedCells = ((contentDataGridControl.Content) as DataGrid).SelectedCells;

            var val = e.EditingElement as TextBox;

            foreach (var cell in selectedCells)
            {
                DataRowView row = cell.Item as DataRowView;
                string columnName = e.Column.Header.ToString();
                string oldValue = (row.Row[columnName]).ToString();
                row.Row[columnName] = val.Text;
                
                Messenger.Default.Send(new DataGridCellValueChangedMessage(val.Text, oldValue, columnName, row, ((MainWindowViewModel)contentDataGridControl.DataContext).SelectedTable));
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ilist = ((DataGrid)(contentDataGridControl.Content)).SelectedItems;
            List<object> selectedItems = ilist.Cast<object>().ToList();
            
            Messenger.Default.Send(new SelectionChangedMessage(selectedItems));
        }
    }
}
