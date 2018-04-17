using LSC1DatabaseLibrary.CommonMySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbJobNameRow : DbRow
    {
        public override string TableName
        {
            get { return "tjobname"; }
            set { }
        }

        public string JobNr
        {
            get { return Values[0]; }
            set { Values[0] = value; }
        }

        public string Name
        {
            get { return Values[1]; }
            set { Values[1] = value; }
        }


        public DbJobNameRow(string jobNr, string name) : this()
        {
            JobNr = jobNr;
            Name = name;
        }

        public DbJobNameRow()
        {
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("JobNr");
            ColumnNames.Add("Name");
        }
    }
}
