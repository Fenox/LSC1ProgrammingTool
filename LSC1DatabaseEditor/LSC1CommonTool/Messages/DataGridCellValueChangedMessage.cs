using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.UpdatingRows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace LSC1DatabaseEditor.LSC1CommonTool.Messages
{
    public class DataGridCellValueChangedMessage
    {
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string ColumnName { get; set; }
        public DataRowView Row { get; set; }
        public LSC1TablePropertiesViewModelBase TableVM { get; set; }

        public DataGridCellValueChangedMessage(string newValue, string oldValue, string columnName, DataRowView row, LSC1TablePropertiesViewModelBase table)
        {
            TableVM = table;
            NewValue = newValue;
            OldValue = oldValue;
            ColumnName = columnName;
            Row = row;
        }
    }
}
