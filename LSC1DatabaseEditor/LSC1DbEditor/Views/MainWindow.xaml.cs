using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.LSC1CommonTool.Messages;
using System.Collections;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;
using LSC1DatabaseEditor.Common.Messages;

namespace LSC1DatabaseEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<TableSelectionChangedMessage>(this, ChangeTableResource);
        }

        private void ChangeTableResource(TableSelectionChangedMessage msg)
        {
            contentDataGridControl.Content = FindResource(msg.SelectedTable.DataGridName);
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var selectedCells = (contentDataGridControl.Content as DataGrid).SelectedCells;

            var val = e.EditingElement as TextBox;

            foreach (DataGridCellInfo cell in selectedCells)
            {
                DataRowView row = cell.Item as DataRowView;
                string columnName = e.Column.Header.ToString();
                string oldValue = row.Row[columnName].ToString();
                row.Row[columnName] = val.Text;
                
                Messenger.Default.Send(new DataGridCellValueChangedMessage(val.Text, oldValue, columnName, row, ((MainWindowViewModel)contentDataGridControl.DataContext).SelectedTable));
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList ilist = ((DataGrid)(contentDataGridControl.Content)).SelectedItems;
            var selectedItems = ilist.Cast<object>().ToList();
            
            Messenger.Default.Send(new SelectionChangedMessage(selectedItems));
        }
    }
}
