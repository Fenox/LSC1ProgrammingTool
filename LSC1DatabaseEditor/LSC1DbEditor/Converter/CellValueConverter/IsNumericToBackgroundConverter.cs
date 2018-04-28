using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ExtensionsAndCodeSnippets.Strings.Extensions;

namespace LSC1DatabaseEditor.LSC1DbEditor.Converter.CellValueConverter
{
    public class IsNumericToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Spezialfall für Step in tjobdata
            if (value != null && (value is int || value.GetType() != typeof(string)))
                return null;

            var name = (string)value;

            if (Numbers.IsNumeric(name)
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
