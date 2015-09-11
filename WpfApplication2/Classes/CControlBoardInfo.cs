using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class CControlBoardInfo
    {
        /// <summary>
        /// 保存治疗带状态的列表
        /// </summary>
        CCureBandClass.ENUM_STATE[] state_collection = new CCureBandClass.ENUM_STATE[8];

        /// <summary>
        /// 治疗带使用总时间
        /// </summary>
        int[] used_time_collection = new int[8];

        /// <summary>
        /// 保存温度的列表
        /// </summary>
        double[] temper_collection = new double[8];

        /// <summary>
        /// 更新时间戳
        /// </summary>
        long timestamp;

        /// <summary>
        /// 是否打开串口
        /// </summary>
        private bool blnrs232_connected;

        /// <summary>
        /// 分析控制板返回的字符串
        /// </summary>
        /// <param name="Query"></param>
        public void AnalyzeQuaryString(string Query)
        {
            string[] grp = Query.Split(';');
            for (int i = 0; i < grp.Length; i++)
            {
                string[] info = grp[i].Split(',');
                // 获取治疗带状态
                if (info[0].ToLower() == "e")       // The cure band is disconnected
                {
                    state_collection[i] = CCureBandClass.ENUM_STATE.Disconnected;
                    used_time_collection[i] = 0;
                    temper_collection[i] = 0;
                }
                else
                {
                    if (info[0] == "1")                 // Curing
                        state_collection[i] = CCureBandClass.ENUM_STATE.Curing;
                    else if (info[0] == "2")            // The cure band is heating, the temperature has not reached the target.
                        state_collection[i] = CCureBandClass.ENUM_STATE.Heating;
                    else                                    // Standby, Ready to cure
                    {
                         state_collection[i] = CCureBandClass.ENUM_STATE.Standby;       
                    }

                    used_time_collection[i] = Convert.ToInt32(info[1]);

                    // 获取治疗带温度
                    double t = Convert.ToDouble(info[2]) / 100;
                    temper_collection[i] = t;
                }
            }

            timestamp = (int)(DateTime.UtcNow - new DateTime(2015, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }

        /// <summary>
        /// 返回每一个治疗通道的状态
        /// 
        /// </summary>
        public  CCureBandClass.ENUM_STATE[] State
        {
            get
            {
                return state_collection;
            }
        }

        /// <summary>
        /// 返回治疗带使用时间（单位同 CPublicVariables.Configuration.MaxCureBandServieTime）
        /// </summary>
        public int[] CureBandServiceTime
        {
            get
            {
                return used_time_collection;

            }
        }

        /// <summary>
        /// 返回实时温度
        /// </summary>
        public double[] Temperature
        {
            get
            {
                return temper_collection;
            }
        }

        /// <summary>
        /// 返回时间戳
        /// </summary>
        public long TimeStamp
        {
            get
            {
                return timestamp;

            }
        }

        /// <summary>
        /// 控制板的串口连接是否正常
        /// </summary>
        public bool RS232Connected
        {
            set
            {
                blnrs232_connected = value;
            }
            get
            {
                return blnrs232_connected;
            }
        }
    }
}
