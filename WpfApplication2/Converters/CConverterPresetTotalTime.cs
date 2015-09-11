using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Tai_Shi_Xuan_Ji_Yi.Classes;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterPresetTotalTimeToComment : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int time = (int)value;
            TimeSpan ts = new TimeSpan(0, time, 0);
            return string.Format("总治疗时间：{0}时{1}分", ts.Hours, ts.Minutes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CConverterPresetTotalTimeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int time = (int)value;
            if(time <= CPublicVariables.Configuration.MaxCureTimeAllowed)
            {
                return new SolidColorBrush(Colors.Black);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
