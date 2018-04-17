using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public class ReadRowsQuery<TItem> : IMySqlQuery<List<TItem>> 
        where TItem: DbRow, new()
    {
        public string Query { get; set; }

        public ReadRowsQuery(string query)
        {
            Query = query;
        }

        public List<TItem> Execute(MySqlConnection connection)
        {
            List<TItem> items = new List<TItem>();
            MySqlCommand cmd = new MySqlCommand(Query, connection);
            try
            {
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                        continue;

                    var newItem = new TItem();
                    if (newItem.Values.Count < reader.FieldCount)
                        newItem.Values = new ObservableCollection<string>(new string[reader.FieldCount].ToList());
                    for (int i = 0; i < reader.FieldCount; i++)
                        newItem.Values[i] = reader.GetString(i);

                    items.Add(newItem);
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }

            return items;
        }
    }
}
