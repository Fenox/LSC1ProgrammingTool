using System;
using System.Globalization;
using System.Windows.Data;

namespace LSC1DatabaseEditor.Common.Converter
{
    public class StringNullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (string)value;
            return val != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
