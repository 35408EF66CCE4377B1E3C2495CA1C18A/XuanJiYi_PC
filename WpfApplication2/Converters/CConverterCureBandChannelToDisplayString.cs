using System.Windows.Data;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    /// <summary>
    /// 由于CureBandClass对象中保存的Channel从0开始计数，
    /// 因此该Converter将Channel的值加1，这样看起来符合逻辑。
    /// </summary>
    class CConverterCureBandChannelToDisplayString : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int ch = (int)value;
            return (ch + 1).ToString();
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
