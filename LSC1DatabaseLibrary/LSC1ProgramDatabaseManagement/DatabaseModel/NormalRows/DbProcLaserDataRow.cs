using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbProcLaserDataRow : DbRow
    {
        public override string TableName
        {
            get { return "tproclaserdata"; }
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

        public string BeamOn
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public string Power
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }

        public string C_Grip
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }

        public DbProcLaserDataRow(string name, string step, string beamOn, string power, string c_Grip) : this()
        {
            Name = name;
            Step = step;
            BeamOn = beamOn;
            Power = power;
            C_Grip = c_Grip;
        }

        public DbProcLaserDataRow()
        {
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("Name");
            ColumnNames.Add("Step");
            ColumnNames.Add("BeamOn");
            ColumnNames.Add("Power");
            ColumnNames.Add("C_Grip");
        }
    }
}
