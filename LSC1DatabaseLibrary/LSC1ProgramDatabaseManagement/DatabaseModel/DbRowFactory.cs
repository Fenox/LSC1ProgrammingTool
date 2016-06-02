using LSC1DatabaseLibrary.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel
{
    public class DbRowFactory
    {
        public T CreateRow<T>() where T : DbRow, new()
        {
            return new T();
        }

        public T CreateUpdatedRow<T>(LSC1DatabaseConnectionSettings settings) where T : UpdatedDbRow, new()
        {
            T newRow = new T();
            newRow.connectionSettings = settings;
            return newRow;
        }
    }
}
