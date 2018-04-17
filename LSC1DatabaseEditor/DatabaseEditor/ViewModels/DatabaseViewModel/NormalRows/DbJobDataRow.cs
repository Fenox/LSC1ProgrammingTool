using LSC1DatabaseLibrary.CommonMySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbJobDataRow : DbRow
    {
        public override string TableName
        {
            get { return "tjobdata"; }
            set { }
        }

        public string JobNr
        {
            get { return Values[0]; }
            set { Values[0] = value; }
        }

        public string Step
        {
            get { return Values[1]; }
            set { Values[1] = value; }
        }

        public string Who
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public string What
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }

        public string Name
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }

        public string MoveParam
        {
            get { return Values[5]; }
            set { Values[5] = value; }
        }

        public string Frame
        {
            get { return Values[6]; }
            set { Values[6] = value; }
        }

        public string Tool
        {
            get { return Values[7]; }
            set { Values[7] = value; }
        }

        public string Interpol
        {
            get { return Values[8]; }
            set { Values[8] = value; }
        }

        public string WEM
        {
            get { return Values[9]; }
            set { Values[9] = value; }
        }

        public string LaserProgNr
        {
            get { return Values[10]; }
            set { Values[10] = value; }
        }

        public DbJobDataRow(string jobNr, string step, string who, string what, string name, string moveparam, string frame, string tool, string interpol, string wem, string laserProgNr) : this()
        {
            JobNr = jobNr;
            Step = step;
            Who = who;
            What = what;
            Name = name;
            MoveParam = moveparam;
            Frame = frame;
            Tool = tool;
            Interpol = interpol;
            WEM = wem;
            LaserProgNr = laserProgNr;
        }

        public DbJobDataRow()
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

            ColumnNames.Add("JobNr");
            ColumnNames.Add("Step");
            ColumnNames.Add("Who");
            ColumnNames.Add("What");
            ColumnNames.Add("Name");
            ColumnNames.Add("MoveParam");
            ColumnNames.Add("Frame");
            ColumnNames.Add("Tool");
            ColumnNames.Add("Interpol");
            ColumnNames.Add("WEM");
            ColumnNames.Add("LaserProgNr");
        }
    }
}
