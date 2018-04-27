using LSC1DatabaseLibrary.CommonMySql;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class DbPosRow : DbRow
    {
        public override string TableName
        {
            get => "tpos";
            set { }
        }

        public string Name
        {
            get => Values[0];
            set => Values[0] = value;
        }

        public string Kind
        {
            get => Values[1];
            set => Values[1] = value;
        }

        public string X
        {
            get => Values[2];
            set => Values[2] = value;
        }

        public string Y
        {
            get => Values[3];
            set => Values[3] = value;
        }

        public string Z
        {
            get => Values[4];
            set => Values[4] = value;
        }

        public string RX
        {
            get => Values[5];
            set => Values[5] = value;
        }

        public string RY
        {
            get => Values[6];
            set => Values[6] = value;
        }

        public string RZ
        {
            get => Values[7];
            set => Values[7] = value;
        }

        public string Locked
        {
            get => Values[8];
            set => Values[8] = value;
        }

        public DbPosRow(string name, string kind, string x, string y, string z, string rx, string ry, string rz, string locked) : this()
        {
            Name = name;
            Kind = kind;
            X = x;
            Y = y;
            Z = z;
            RX = rx;
            RY = ry;
            RZ = rz;
            Locked = locked;
        }

        public DbPosRow()
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

            ColumnNames.Add("Name");
            ColumnNames.Add("Kind");
            ColumnNames.Add("x_a1");
            ColumnNames.Add("y_a2");
            ColumnNames.Add("z_a3");
            ColumnNames.Add("rx_a4");
            ColumnNames.Add("ry_a5");
            ColumnNames.Add("rz_a6");
            ColumnNames.Add("Locked");
        }
    }
}
