using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;

namespace LSC1DatabaseEditor.Messages
{
    public class TableSelectionChangedMessage
    {
        public LSC1TablePropertiesViewModelBase SelectedTable { get; set; }

        public TableSelectionChangedMessage(LSC1TablePropertiesViewModelBase selectedTable)
        {
            SelectedTable = selectedTable;
        }
    }
}
