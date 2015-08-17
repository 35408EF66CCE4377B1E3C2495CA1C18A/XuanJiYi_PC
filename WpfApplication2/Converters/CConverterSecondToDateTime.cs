using System;
using System.Windows.Data;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterSecondToDateTime : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int elapsed_sec = (int)value;
            TimeSpan ts = new TimeSpan(0, 0, elapsed_sec);
            return string.Format("{0:%h}时{0:%m}分{0:%s}秒", ts);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
