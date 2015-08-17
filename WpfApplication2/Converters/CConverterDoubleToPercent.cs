using System.Windows.Data;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterDoubleToPercent : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // 显示的小数位数，如果没有指定或指定的参数不是数字，默认为1位
            string digt = parameter.ToString();
            int val;
            if (!int.TryParse(digt, out val))
                digt = "1";

            double per = (double)value;
            return per.ToString("F" + digt) + "%"; 
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
