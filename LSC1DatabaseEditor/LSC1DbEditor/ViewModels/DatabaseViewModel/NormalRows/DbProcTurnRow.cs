using LSC1DatabaseLibrary.CommonMySql;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class DbProcTurnRow : DbProcRow
    {

        public override string TableName
        {
            get => "tprocturn";
            set { }
        }

        public string Pos
        {
            get => Values[2];
            set => Values[2] = value;
        }

        public string Angle
        {
            get => Values[3];
            set => Values[3] = value;
        }

        public string Speed
        {
            get => Values[4];
            set => Values[4] = value;
        }

        public string Velocity
        {
            get => Values[5];
            set => Values[5] = value;
        }

        public string Power
        {
            get => Values[6];
            set => Values[6] = value;
        }

        public string BeamOn
        {
            get => Values[7];
            set => Values[7] = value;
        }

        public string WEM
        {
            get => Values[8];
            set => Values[8] = value;
        }

        public string PulseTime
        {
            get => Values[9];
            set => Values[9] = value;
        }

        public string AnzPulse
        {
            get => Values[10];
            set => Values[10] = value;
        }
        
        public DbProcTurnRow(string name, string step, string pos, string angle, string speed, string velocity, string power, string beamOn, string wem, string pulseTime, string anzPulse) : this()
        {
            Name = name;
            Step = step;
            Pos = pos;
            Angle = angle;
            Speed = speed;
            Velocity = velocity;
            Power = power;
            BeamOn = beamOn;
            WEM = wem;
            PulseTime = pulseTime;
            AnzPulse = anzPulse;
        }

        public DbProcTurnRow()
        {
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("Name");
            ColumnNames.Add("Step");
            ColumnNames.Add("Pos");
            ColumnNames.Add("Angle");
            ColumnNames.Add("Speed");
            ColumnNames.Add("Velocity");
            ColumnNames.Add("Power");
            ColumnNames.Add("BeamOn");
            ColumnNames.Add("WEM");
            ColumnNames.Add("PulseTime");
            ColumnNames.Add("AnzPulse");
        }
    }
}
