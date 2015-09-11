using System.Windows.Data;
using Tai_Shi_Xuan_Ji_Yi.Classes;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterCurebandStateToString : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ret = string.Empty;
            CCureBandClass.ENUM_STATE state = (CCureBandClass.ENUM_STATE)value;
            switch(state)
            {
                case CCureBandClass.ENUM_STATE.Disconnected:
                    ret = "无治疗带";
                    break;

                case CCureBandClass.ENUM_STATE.Curing:
                    ret = "正在治疗";
                    break;

                case CCureBandClass.ENUM_STATE.Overdue:
                    ret = "治疗带过期";
                    break;

                case CCureBandClass.ENUM_STATE.Standby:
                    ret = "准备好";
                    break;

                case CCureBandClass.ENUM_STATE.Heating:
                    ret = "预加热";
                    break;
            }

            return ret;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
