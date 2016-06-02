using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbTwtRow : DbRow
    {
        public override string TableName
        {
            get { return "twt"; }
            set { }
        }

        public string WtId
        {
            get { return Values[0]; }
            set { Values[0] = value; }
        }
        public string WtTyp
        {
            get { return Values[1]; }
            set { Values[1] = value; }
        }
        public string FrameT1
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }
        public string FrameT2
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }
        public string JobT1
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }
        public string JobT2
        {
            get { return Values[5]; }
            set { Values[5] = value; }
        }
        public string CPos1
        {
            get { return Values[6]; }
            set { Values[6] = value; }
        }
        public string CPos2
        {
            get { return Values[7]; }
            set { Values[7] = value; }
        }
        public string Typ
        {
            get { return Values[8]; }
            set { Values[8] = value; }
        }
        public string EnableT1
        {
            get { return Values[9]; }
            set { Values[9] = value; }
        }
        public string EnableT2
        {
            get { return Values[10]; }
            set { Values[10] = value; }
        }
        public string CPos3
        {
            get { return Values[11]; }
            set { Values[11] = value; }
        }
        public string CPos4
        {
            get { return Values[12]; }
            set { Values[12] = value; }
        }

        public DbTwtRow(string wtId, string wtTyp, string frameT1, string frameT2, string jobT1, string jobT2, string cpos1, string cpos2, string typ, string enableT1, string enableT2, string cpos3, string cpos4) : this()
        {
            WtId = wtId;
            WtTyp = wtTyp;
            FrameT1 = frameT1;
            FrameT2 = frameT2;
            JobT1 = jobT1;
            JobT2 = jobT2;
            CPos1 = cpos1;
            CPos2 = cpos2;
            Typ = typ;
            EnableT1 = enableT1;
            EnableT2 = enableT2;
            CPos3 = cpos3;
            CPos4 = cpos4;
        }

        public DbTwtRow()
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
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("WtId");
            ColumnNames.Add("WtTyp");
            ColumnNames.Add("FrameT1");
            ColumnNames.Add("FrameT2");
            ColumnNames.Add("JobT1");
            ColumnNames.Add("JobT2");
            ColumnNames.Add("CPos1");
            ColumnNames.Add("CPos2");
            ColumnNames.Add("Typ");
            ColumnNames.Add("EnableT1");
            ColumnNames.Add("EnableT2");
            ColumnNames.Add("CPos3");
            ColumnNames.Add("CPos4");
        }
    }
}
