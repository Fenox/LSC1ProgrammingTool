using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;
using System.Data;

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
