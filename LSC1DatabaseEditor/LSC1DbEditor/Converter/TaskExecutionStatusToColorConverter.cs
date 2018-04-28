using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LSC1DatabaseEditor.LSC1DbEditor.Converter
{
    public class TaskExecutionStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "Bereit" ? new SolidColorBrush(Color.FromRgb(0, 122, 204)) : new SolidColorBrush(Color.FromRgb(202, 81, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
