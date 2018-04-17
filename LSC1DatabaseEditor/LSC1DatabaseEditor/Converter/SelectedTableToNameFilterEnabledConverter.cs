using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LSC1DatabaseEditor.Views.Converter
{
    public class SelectedTableToNameFilterEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            TablesEnum selectedTable = (TablesEnum)value;

            return selectedTable == TablesEnum.tproclaserdata
                || selectedTable == TablesEnum.tprocrobot
                || selectedTable == TablesEnum.tprocturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
