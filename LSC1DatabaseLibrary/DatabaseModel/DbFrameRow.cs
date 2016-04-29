using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbFrameRow : DbRow
    {
        public override string TableName
        {
            get { return "tframe"; }
            set { }
        }

        public string Name
        {
            get { return Values[0]; }
            set { Values[0] = value; }
        }

        public string Typ
        {
            get { return Values[1]; }
            set { Values[1] = value; }
        }

        public string X
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public string Y
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }

        public string Z
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }

        public string RX
        {
            get { return Values[5]; }
            set { Values[5] = value; }
        }

        public string RY
        {
            get { return Values[6]; }
            set { Values[6] = value; }
        }

        public string RZ
        {
            get { return Values[7]; }
            set { Values[7] = value; }
        }

        public DbFrameRow(string name, string typ, string x, string y, string z, string rx, string ry, string rz) : this()
        {
            Name = name;
            Typ = typ;

            X = x;
            Y = y;
            Z = z;

            RX = rx;
            RY = ry;
            RZ = rz;
        }

        public DbFrameRow()
        {
            Values.Add("");
            Values.Add("");

            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            Values.Add("False");

            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            Values.Add("0");
            Values.Add("0");
            Values.Add("0");

            ColumnNames.Add("Name");
            ColumnNames.Add("Typ");

            ColumnNames.Add("x");
            ColumnNames.Add("y");
            ColumnNames.Add("z");

            ColumnNames.Add("rx");
            ColumnNames.Add("ry");
            ColumnNames.Add("rz");

            ColumnNames.Add("Locked");

            ColumnNames.Add("P0x");
            ColumnNames.Add("P0y");
            ColumnNames.Add("P0z");

            ColumnNames.Add("P0rx");
            ColumnNames.Add("P0ry");
            ColumnNames.Add("P0rz");

            ColumnNames.Add("PXx");
            ColumnNames.Add("PXy");
            ColumnNames.Add("PXz");

            ColumnNames.Add("PXrx");
            ColumnNames.Add("PXry");
            ColumnNames.Add("PXrz");

            ColumnNames.Add("PYx");
            ColumnNames.Add("PYy");
            ColumnNames.Add("PYz");

            ColumnNames.Add("PYrx");
            ColumnNames.Add("PYry");
            ColumnNames.Add("PYrz");
        }
    }
}
