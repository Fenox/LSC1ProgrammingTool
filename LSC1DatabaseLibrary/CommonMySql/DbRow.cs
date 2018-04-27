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

    public class DbProcRow : DbRow
    {
            public string Name
            {
                get => Values[0];
                set => Values[0] = value;
            }

            public string Step
            {
                get => Values[1];
                set => Values[1] = value;
            }
        }
}
