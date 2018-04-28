﻿using System.Data;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public class GetTableQuery : MySqlQuery<DataTable>
    {
        private readonly string query;

        public GetTableQuery(string query)
        {
            this.query = query;
        }


        protected override DataTable ProtectedExecution(MySqlConnection connection)
        {
            var dt = new DataTable();

            var adapter = new MySqlDataAdapter(query, connection);

            connection.Close();
            connection.Open();
            adapter.Fill(dt);

            return dt;
        }
    }
}
