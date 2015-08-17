using System.Windows.Data;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterDoubleToTemperature : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double temper = (double)value;
            return temper.ToString("F2") + " ℃";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
