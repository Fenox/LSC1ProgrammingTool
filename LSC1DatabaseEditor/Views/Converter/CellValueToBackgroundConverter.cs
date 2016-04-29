using LSC1DatabaseEditor.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace LSC1DatabaseEditor.Views.Converter
{
    public class CellValueToBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = (DataGridCell)values[0];
            string columnHeader = cell.Column.Header.ToString();

            string content = "";
            if(cell.Content != null)
                content = cell.Content.ToString();

            bool valid = false;
            switch (columnHeader)
            {
                case "MoveParam":
                    valid = OfflineDatabase.AllMoveParamNames.Contains(content);
                    break;
                default:
                    break;
            }

            return valid ? Brushes.Red : null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
