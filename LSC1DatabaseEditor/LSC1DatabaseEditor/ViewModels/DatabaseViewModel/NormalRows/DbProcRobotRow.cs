using LSC1DatabaseLibrary.CommonMySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbProcRobotRow : DbRow
    {
        public override string TableName
        {
            get { return "tprocRobot"; }
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

        public string Kind
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public string Interpol
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }

        public string X
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }

        public string Y
        {
            get { return Values[5]; }
            set { Values[5] = value; }
        }

        public string Z
        {
            get { return Values[6]; }
            set { Values[6] = value; }
        }

        public string RX
        {
            get { return Values[7]; }
            set { Values[7] = value; }
        }

        public string RY
        {
            get { return Values[8]; }
            set { Values[8] = value; }
        }

        public string RZ
        {
            get { return Values[9]; }
            set { Values[9] = value; }
        }

        public string Accel
        {
            get { return Values[10]; }
            set { Values[10] = value; }
        }

        public string Vel
        {
            get { return Values[11]; }
            set { Values[11] = value; }
        }

        public string Deccel
        {
            get { return Values[12]; }
            set { Values[12] = value; }
        }

        public string Tvel
        {
            get { return Values[13]; }
            set { Values[13] = value; }
        }

        public string Leave
        {
            get { return Values[14]; }
            set { Values[14] = value; }
        }

        public string Reach
        {
            get { return Values[15]; }
            set { Values[15] = value; }
        }

        public DbProcRobotRow(string name, string step, string kind, string interpol, string x_a1, string y_a2, string z_a3, string rx_a4, string ry_a5, string rz_a6, string accel, string vel, string deccel, string tvel, string leave, string reach) : this()
        {
            Name = name;
            Step = step;
            Kind = kind;
            Interpol = interpol;
            X = x_a1;
            Y = y_a2;
            Z = z_a3;
            RX = rx_a4;
            RY = ry_a5;
            RZ = rz_a6;
            Accel = accel;
            Vel = vel;
            Deccel = deccel;
            Tvel = tvel;
            Leave = leave;
            Reach = reach;
        }

        public DbProcRobotRow()
        {
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("lin");
            Values.Add("0");
            Values.Add("0");
            Values.Add("0");
            Values.Add("0");
            Values.Add("0");
            Values.Add("0");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("Name");
            ColumnNames.Add("Step");
            ColumnNames.Add("Kind");
            ColumnNames.Add("Interpol");
            ColumnNames.Add("x_a1");
            ColumnNames.Add("y_a2");
            ColumnNames.Add("z_a3");
            ColumnNames.Add("rx_a4");
            ColumnNames.Add("ry_a5");
            ColumnNames.Add("rz_a6");
            ColumnNames.Add("accel");
            ColumnNames.Add("vel");
            ColumnNames.Add("deccel");
            ColumnNames.Add("tvel");
            ColumnNames.Add("leave");
            ColumnNames.Add("reach");
        }
    }
}
