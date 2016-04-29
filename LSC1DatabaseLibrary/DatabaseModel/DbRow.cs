using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public abstract class DbRow
    {
        public abstract string TableName { get; set; }

        public int NumElements
        {
            get { return ColumnNames.Count; }
            set { }
        }

        List<string> columnNames = new List<string>();
        public List<string> ColumnNames
        {
            get { return columnNames; }
            set { columnNames = value; }
        }

        public List<string> Values { get; set; } = new List<string>();
    }
}
