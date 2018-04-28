using System;
using System.Globalization;
using System.Windows.Data;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels;
using LSC1DatabaseLibrary;

namespace LSC1DatabaseEditor.LSC1DbEditor.Converter
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
