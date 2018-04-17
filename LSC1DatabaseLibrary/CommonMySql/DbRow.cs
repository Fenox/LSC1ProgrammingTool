using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.CommonMySql
{
    public class DbRow
    {
        public virtual string TableName { get; set; }

        public List<string> ColumnNames { get; set; } = new List<string>();

        public ObservableCollection<string> Values { get; set; } = new ObservableCollection<string>();
    }
}
