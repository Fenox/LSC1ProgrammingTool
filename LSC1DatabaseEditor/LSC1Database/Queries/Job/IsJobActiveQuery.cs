﻿using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC1DatabaseEditor.LSC1Database.Queries.Job
{
    public class IsJobActiveQuery : MySqlQuery<bool>
    {
        private string jobNr;

        public IsJobActiveQuery(string jobNr)
        {
            this.jobNr = jobNr;
        }

        protected override bool ProtectedExecution(MySqlConnection connection)
        {
            const string query = "SELECT COUNT(*) FROM `twt` WHERE(JobT1 = @JobNr1 OR JobT2 = @JobNr2) AND (WtId IN(SELECT WtId FROM `ttable`) OR WtId IN(SELECT BackWtId FROM `ttable`))";
            var cmd = new MySqlCommand(query);
            var parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("JobNr1", jobNr));
            parameters.Add(new MySqlParameter("JobNr2", jobNr));

            var countQuery = new CountQuery(query, parameters.ToArray());

            return countQuery.Execute(connection) > 1;
        }
    }
}