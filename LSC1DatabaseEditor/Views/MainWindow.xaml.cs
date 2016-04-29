using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.Model;
using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var selectedCells = dataGrid.SelectedCells;

            var val = e.EditingElement as TextBox;

            foreach (var cell in selectedCells)
            {
                DataRowView row = cell.Item as DataRowView;
                string columnName = e.Column.Header.ToString();
                string oldValue = (row.Row[columnName]).ToString();
                row.Row[columnName] = val.Text;

                string updateString = string.Empty;

                //Create String that Updates a row 
                updateString = "UPDATE " + viewModel.SelectedTable +
                           " SET `" + columnName + "` = '" + val.Text +
                           "' WHERE ";

                for (int i = 0; i < viewModel.TableData.Columns.Count; i++)
                {
                    string columnNamei = viewModel.TableData.Columns[i].ColumnName;

                    if (columnName == columnNamei)
                    {
                        //Falls die letzte Spalte geändert wurde muss das AND wieder weg
                        if (i == viewModel.TableData.Columns.Count - 1)
                            updateString = updateString.Remove(updateString.Length - 4);

                        continue;
                    }

                    updateString += "`" + columnNamei + "` = '" + row.Row[i] + "'";

                    if (i != viewModel.TableData.Columns.Count - 1)
                        updateString += " AND ";
                }

                LSC1DatabaseConnector db = new LSC1DatabaseConnector();

                try
                {
                    db.ExecuteQuery(updateString);
                }
                catch (Exception ex)
                {
                    row.Row[columnName] = oldValue;
                    MessageBox.Show(ex.ToString());
                }


                //Updaten der OfflineDatenbank, falls wichtiges geändert wurde

                if (e.Column.Header.ToString() == "Name")
                {
                    OfflineDatabase.UpdateTable(viewModel.SelectedTable);
                }
            }
        }
    }
}
