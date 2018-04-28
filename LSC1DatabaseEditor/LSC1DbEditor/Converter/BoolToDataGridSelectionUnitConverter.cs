using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace LSC1DatabaseEditor.LSC1DbEditor.Converter
{
    public class BoolToDataGridSelectionUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                throw new ArgumentException("Cannot convert null to selection");

            return (bool)value ? DataGridSelectionUnit.FullRow : DataGridSelectionUnit.Cell;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentException("Cannot convert null");

            var val = (DataGridSelectionUnit)value;

            return val == DataGridSelectionUnit.FullRow;
        }
    }
}
