using System;
using System.Globalization;
using System.Windows.Data;
using LSC1DatabaseLibrary;

namespace LSC1DatabaseEditor.LSC1DbEditor.Converter
{
    public class SelectedTableToNameFilterEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var selectedTable = (TablesEnum)value;

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
