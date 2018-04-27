using LSC1DatabaseLibrary.CommonMySql;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class DbProcPulseRow : DbProcRow
    {
        public override string TableName
        {
            get => "tprocpulse";
            set { }
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
