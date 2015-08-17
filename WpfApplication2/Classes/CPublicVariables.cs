using System.Collections.ObjectModel;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class CPublicVariables
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum EnumLogMessageType
        {
            Normal, Warning, Error
        }

        /// <summary>
        /// 系统配置参数
        /// </summary>
        public static CConfiguration Configuration;

        /// <summary>
        /// 保存治疗带信息的集合 
        /// </summary>
        public static ObservableCollection<CCureBandClass> CureBandList;
    }
}
