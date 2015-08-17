using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Tai_Shi_Xuan_Ji_Yi.Classes;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterBancStateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush backcolor = new SolidColorBrush(Colors.White);
            CCureBandClass.ENUM_STATE bandstate = (CCureBandClass.ENUM_STATE)value;
            string arg = parameter.ToString();
            switch(bandstate)
            {
                case CCureBandClass.ENUM_STATE.Disconnected:
                case CCureBandClass.ENUM_STATE.Overdue:
                    backcolor = new SolidColorBrush(Colors.Red);
                    break;

                case CCureBandClass.ENUM_STATE.Curing:
                case CCureBandClass.ENUM_STATE.Standby:
                    if(arg == "black")
                        backcolor = new SolidColorBrush(Colors.Black);
                    else
                        backcolor = new SolidColorBrush(Colors.White);
                    break;
            }

            return backcolor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
