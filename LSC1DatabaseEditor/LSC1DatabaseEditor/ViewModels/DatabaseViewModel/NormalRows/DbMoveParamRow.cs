using LSC1DatabaseLibrary.CommonMySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbMoveParamRow : DbRow
    {
        public override string TableName
        {
            get { return "tmoveparam"; }
            set { }
        }

        public string Name
        {
            get { return Values[0]; }
            set { Values[0] = value; }
        }

        public string Accel
        {
            get { return Values[1]; }
            set { Values[1] = value; }
        }

        public string Vel
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public string Deccel
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }

        public string Tvel
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }

        public string Leave
        {
            get { return Values[5]; }
            set { Values[5] = value; }
        }

        public string Reach
        {
            get { return Values[6]; }
            set { Values[6] = value; }
        }

        public DbMoveParamRow(string name, string accel, string vel, string deccel, string tvel, string leave, string reach) : this()
        {
            Name = name;
            Accel = accel;
            Vel = vel;
            Deccel = deccel;
            Tvel = tvel;
            Leave = leave;
            Reach = reach;
        }

        public DbMoveParamRow()
        {
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("Name");
            ColumnNames.Add("accel");
            ColumnNames.Add("vel");
            ColumnNames.Add("deccel");
            ColumnNames.Add("tvel");
            ColumnNames.Add("leave");
            ColumnNames.Add("reach");
        }
    }
}
