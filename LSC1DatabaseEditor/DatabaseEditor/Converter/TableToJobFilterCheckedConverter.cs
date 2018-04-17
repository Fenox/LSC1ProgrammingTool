using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LSC1DatabaseEditor.Views.Converter
{
    public class TableToJobFilterCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            TablesEnum selectedTable = ((LSC1TablePropertiesViewModelBase)value).Table;

            return selectedTable == TablesEnum.tframe
                    || selectedTable == TablesEnum.tjobdata
                    || selectedTable == TablesEnum.tmoveparam
                    || selectedTable == TablesEnum.tpos
                    || selectedTable == TablesEnum.tproclaserdata
                    || selectedTable == TablesEnum.tprocplc
                    || selectedTable == TablesEnum.tprocpulse
                    || selectedTable == TablesEnum.tprocrobot
                    || selectedTable == TablesEnum.tprocturn
                    || selectedTable == TablesEnum.ttool
                    || selectedTable == TablesEnum.twt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
