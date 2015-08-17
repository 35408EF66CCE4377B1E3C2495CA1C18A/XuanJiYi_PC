using System.Windows.Data;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using Tai_Shi_Xuan_Ji_Yi.Controls;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterActionStateToIcon : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CCureBandClass.ENUM_ACTION stat = (CCureBandClass.ENUM_ACTION)value;
            RunPauseStopIcon.ENUM_State to = RunPauseStopIcon.ENUM_State.Stop;
            switch(stat)
            {
                case CCureBandClass.ENUM_ACTION.Started:
                    to = RunPauseStopIcon.ENUM_State.Run;
                    break;

                case CCureBandClass.ENUM_ACTION.Paused:
                    to = RunPauseStopIcon.ENUM_State.Pause;
                    break;

                case CCureBandClass.ENUM_ACTION.Finished:
                case CCureBandClass.ENUM_ACTION.Stopped:
                    to = RunPauseStopIcon.ENUM_State.Stop;
                    break;
            }
            return to;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
