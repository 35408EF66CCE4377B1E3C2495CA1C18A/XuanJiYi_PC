using System;
using System.Globalization;
using System.Windows.Data;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterMinutesToHours : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int minutes = (int)value;
            TimeSpan ts = new TimeSpan(0, minutes, 0);
            return string.Format("{0}小时", ts.TotalHours.ToString("F0"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
