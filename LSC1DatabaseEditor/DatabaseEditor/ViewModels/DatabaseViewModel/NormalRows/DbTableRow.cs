using LSC1DatabaseLibrary.CommonMySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbTableRow : DbRow
    {
        public override string TableName
        {
            get { return "ttable"; }
            set { }
        }


        public string TurnTable
        {
            get { return Values[0]; }
            set { Values[0] = value; }
        }

        public string WtId
        {
            get { return Values[1]; }
            set { Values[1] = value; }
        }

        public string Status
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public string Gas
        {
            get { return Values[3]; }
            set { Values[3] = value; }
        }

        public string BackWtId
        {
            get { return Values[4]; }
            set { Values[4] = value; }
        }

        public string BackStatus
        {
            get { return Values[5]; }
            set { Values[5] = value; }
        }

        public DbTableRow(string turnTable, string wtId, string status, string gas, string backWtId, string backStatus) : this()
        {
            TurnTable = turnTable;
            WtId = wtId;
            Status = status;
            Gas = gas;
            BackWtId = backWtId;
            BackStatus = backStatus;
        }

        public DbTableRow()
        {
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("TurnTable");
            ColumnNames.Add("WtId");
            ColumnNames.Add("Status");
            ColumnNames.Add("Gas");
            ColumnNames.Add("BackWtId");
            ColumnNames.Add("BackStatus");
        }
    }
}
