using LSC1DatabaseLibrary.CommonMySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbProcPlcRow : DbRow
    {
        public override string TableName
        {
            get { return "tprocplc"; }
            set { }
        }

        public string Name
        {
            get { return Values[0]; }
            set { Values[0] = value; }
        }

        public string Step
        {
            get { return Values[1]; }
            set { Values[1] = value; }
        }

        public string Actor
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public string Value
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }

        public string Parameter
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }

        public DbProcPlcRow(string name, string step, string actor, string value, string parameter) : this()
        {
            Name = name;
            Step = step;
            Actor = actor;
            Value = value;
            Parameter = parameter;
        }

        public DbProcPlcRow()
        {
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("Name");
            ColumnNames.Add("Step");
            ColumnNames.Add("Actor");
            ColumnNames.Add("Value");
            ColumnNames.Add("Parameter");
        }
    }
}
