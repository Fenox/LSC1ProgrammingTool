using LSC1DatabaseLibrary.CommonMySql;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class DbProcPulseRow : DbRow
    {
        public override string TableName
        {
            get => "tprocpulse";
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

        public string PulseTime
        {
            get => Values[2];
            set => Values[2] = value;
        }

        public string Power
        {
            get => Values[3];
            set => Values[3] = value;
        }
                
        public DbProcPulseRow(string name, string step, string pulseTime, string power) : this()
        {
            Name = name;
            Step = step;
            PulseTime = pulseTime;
            Power = power;
        }

        public DbProcPulseRow()
        {
            Values.Add("");
            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            ColumnNames.Add("Name");
            ColumnNames.Add("Step");
            ColumnNames.Add("PulseTime");
            ColumnNames.Add("Power");
        }
    }
}
