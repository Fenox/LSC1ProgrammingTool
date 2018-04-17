using LSC1DatabaseLibrary.CommonMySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbProcTurnRow : DbRow
    {

        public override string TableName
        {
            get { return "tprocturn"; }
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

        public string Pos
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public string Angle
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }

        public string Speed
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }

        public string Velocity
        {
            get { return Values[5]; }
            set { Values[5] = value; }
        }

        public string Power
        {
            get { return Values[6]; }
            set { Values[6] = value; }
        }

        public string BeamOn
        {
            get { return Values[7]; }
            set { Values[7] = value; }
        }

        public string WEM
        {
            get { return Values[8]; }
            set { Values[8] = value; }
        }

        public string PulseTime
        {
            get { return Values[9]; }
            set { Values[9] = value; }
        }

        public string AnzPulse
        {
            get { return Values[10]; }
            set { Values[10] = value; }
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
