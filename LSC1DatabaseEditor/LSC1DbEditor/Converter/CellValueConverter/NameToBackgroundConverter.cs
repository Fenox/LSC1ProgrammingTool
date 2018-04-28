using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using LSC1DatabaseEditor.LSC1Database;

namespace LSC1DatabaseEditor.LSC1DbEditor.Converter.CellValueConverter
{
    public class NameToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() != typeof(string))
                return Brushes.Red;

            var name = (string)value;

            if (OfflineDatabase.AllPosNames.Contains(name)
                || OfflineDatabase.AllProcNames.Contains(name)
                || OfflineDatabase.AllProcPLCNames.Contains(name)
                || OfflineDatabase.AllProcPulseNames.Contains(name)
                || OfflineDatabase.AllProcTurnNames.Contains(name)
                || name == "?")
                return null;
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
