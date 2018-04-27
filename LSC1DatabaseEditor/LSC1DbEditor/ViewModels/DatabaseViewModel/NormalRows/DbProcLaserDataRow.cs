﻿using LSC1DatabaseLibrary.CommonMySql;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class DbProcLaserDataRow : DbProcRow
    {
        public override string TableName
        {
            get => "tproclaserdata";
            set { }
        }

        public string BeamOn
        {
            get => Values[2];
            set => Values[2] = value;
        }

        public string Power
        {
            get => Values[3];
            set => Values[3] = value;
        }

        public string C_Grip
        {
            get => Values[4];
            set => Values[4] = value;
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
