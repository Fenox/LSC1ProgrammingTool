using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
