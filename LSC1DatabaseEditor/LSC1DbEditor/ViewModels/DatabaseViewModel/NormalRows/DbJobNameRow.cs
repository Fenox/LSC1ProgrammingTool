using LSC1DatabaseLibrary.CommonMySql;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class DbJobNameRow : DbRow
    {
        public override string TableName
        {
            get => "tjobname";
            set { }
        }

        public string JobNr
        {
            get => Values[0];
            set => Values[0] = value;
        }

        public string Name
        {
            get => Values[1];
            set => Values[1] = value;
        }


        public DbJobNameRow(string jobNr, string name) : this()
        {
            JobNr = jobNr;
            Name = name;
        }

        public DbJobNameRow()
        {
            Values.Add("");
            Values.Add("");

            ColumnNames.Add("JobNr");
            ColumnNames.Add("Name");
        }
    }
}
