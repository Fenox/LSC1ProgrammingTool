using LSC1DatabaseLibrary.CommonMySql;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class DbTableRow : DbRow
    {
        public override string TableName
        {
            get => "ttable";
            set { }
        }


        public string TurnTable
        {
            get => Values[0];
            set => Values[0] = value;
        }

        public string WtId
        {
            get => Values[1];
            set => Values[1] = value;
        }

        public string Status
        {
            get => Values[2];
            set => Values[2] = value;
        }

        public string Gas
        {
            get => Values[3];
            set => Values[3] = value;
        }

        public string BackWtId
        {
            get => Values[4];
            set => Values[4] = value;
        }

        public string BackStatus
        {
            get => Values[5];
            set => Values[5] = value;
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
