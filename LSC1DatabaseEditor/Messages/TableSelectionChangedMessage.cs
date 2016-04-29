using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseEditor.Messages
{
    public class TableSelectionChangedMessage
    {
        public TablesEnum SelectedTable { get; set; }

        public TableSelectionChangedMessage(TablesEnum selectedTable)
        {
            SelectedTable = selectedTable;
        }
    }
}
