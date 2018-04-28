using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace LSC1DatabaseEditor.Common.Converter
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException();
            else
                return new SolidColorBrush((Color) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
