﻿using LSC1Library;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace LSC1DatabaseEditor.Views.Converter.CellValueConverter
{
    public class ToolToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(string))
                return Brushes.Red;

            string name = (string)value;

            if (OfflineDatabase.AllToolNames.Contains(name)
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
