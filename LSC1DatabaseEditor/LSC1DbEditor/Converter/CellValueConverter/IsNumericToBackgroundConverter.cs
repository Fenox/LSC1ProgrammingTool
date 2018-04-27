using ExtensionsAndCodeSnippets.Strings.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LSC1DatabaseEditor.Views.Converter.CellValueConverter
{
    public class IsNumericToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Spezialfall für Step in tjobdata
            if (value.GetType() == typeof(int) || value.GetType() != typeof(string))
                return null;

            string name = (string)value;

            if (Numbers.IsNumeric(name)
                || name == "?")
                return null;
            else
                return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
