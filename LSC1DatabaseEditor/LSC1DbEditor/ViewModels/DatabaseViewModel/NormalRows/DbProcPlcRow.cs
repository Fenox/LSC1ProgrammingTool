using LSC1DatabaseLibrary.CommonMySql;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class DbProcPlcRow : DbRow
    {
        public override string TableName
        {
            get => "tprocplc";
            set { }
        }

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

        public string Actor
        {
            get => Values[2];
            set => Values[2] = value;
        }

        public string Value
        {
            get => Values[3];
            set => Values[3] = value;
        }

        public string Parameter
        {
            get => Values[4];
            set => Values[4] = value;
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
