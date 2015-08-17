using System.Windows.Data;
using Tai_Shi_Xuan_Ji_Yi.Classes;

namespace Tai_Shi_Xuan_Ji_Yi.Converters
{
    class CConverterBandActionToButtonState : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string cat = parameter.ToString();

            string title = "";
            bool is_enable = false;

            if (cat == "Start_Enabled")
            {
                CCureBandClass.ENUM_ACTION action = (CCureBandClass.ENUM_ACTION)value;
                switch (action)
                {

                    case CCureBandClass.ENUM_ACTION.Started:
                        is_enable = false;
                        break;

                    case CCureBandClass.ENUM_ACTION.Stopped:
                    case CCureBandClass.ENUM_ACTION.Paused:
                    case CCureBandClass.ENUM_ACTION.Finished:
                        is_enable = true;
                        break;
                }

                return is_enable;
            }
            else if(cat == "Start_Title")
            {
                CCureBandClass.ENUM_ACTION action = (CCureBandClass.ENUM_ACTION)value;
                switch (action)
                {
                    case CCureBandClass.ENUM_ACTION.Stopped:
                    case CCureBandClass.ENUM_ACTION.Started:
                    case CCureBandClass.ENUM_ACTION.Finished:
                        title = "启动治疗";
                        break;

                    
                    case CCureBandClass.ENUM_ACTION.Paused:
                        title = "继续治疗";
                        break;
                }
                return title;
            }
            else if (cat == "Stop_Enabled")
            {
                CCureBandClass.ENUM_ACTION action = (CCureBandClass.ENUM_ACTION)value;
                switch (action)
                {
                    case CCureBandClass.ENUM_ACTION.Paused:
                    case CCureBandClass.ENUM_ACTION.Started:
                        is_enable = true;
                        break;

                    case CCureBandClass.ENUM_ACTION.Stopped:
                    case CCureBandClass.ENUM_ACTION.Finished:
                        is_enable = false;
                        break;
                }
                return is_enable;
            }
            else if(cat == "Stop_Title")
            {
                CCureBandClass.ENUM_ACTION action = (CCureBandClass.ENUM_ACTION)value;
                switch (action)
                {
                    case CCureBandClass.ENUM_ACTION.Started:
                        title = "暂停治疗";
                        break;

                    case CCureBandClass.ENUM_ACTION.Stopped:
                    case CCureBandClass.ENUM_ACTION.Paused:
                    case CCureBandClass.ENUM_ACTION.Finished:
                        title = "停止治疗";
                        break;
                }
                return title;
            }
            else if(cat == "New_Enabled")
            {
                CCureBandClass.ENUM_ACTION action = (CCureBandClass.ENUM_ACTION)value;
                switch (action)
                {
                    case CCureBandClass.ENUM_ACTION.Paused:
                    case CCureBandClass.ENUM_ACTION.Started:
                        is_enable = false;
                        break;

                    case CCureBandClass.ENUM_ACTION.Stopped:
                    case CCureBandClass.ENUM_ACTION.Finished:
                        is_enable = true;
                        break;
                }
                return is_enable;
            }

            return null;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
