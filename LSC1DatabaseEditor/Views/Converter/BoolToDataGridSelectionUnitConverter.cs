using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace LSC1DatabaseEditor.Views.Converter
{
    public class BoolToDataGridSelectionUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? DataGridSelectionUnit.FullRow : DataGridSelectionUnit.Cell;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridSelectionUnit val = (DataGridSelectionUnit)value;

            return val == DataGridSelectionUnit.FullRow;
        }
    }
}
