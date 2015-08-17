using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.Realtime
{
    public class CRealtimeTemperaturePoint
    {
        public CRealtimeTemperaturePoint(int Time, double Temper)
        {
            DateTime now = Convert.ToDateTime("2000-01-01 00:00:00");
            now = now.AddSeconds(Time);
            this.PickTime = now;
            this.Temperature = Temper;
        }


        /// <summary>
        /// 返回记录时间
        /// </summary>
        public DateTime PickTime
        {
            private set;
            get;
        }

        /// <summary>
        /// 返回温度
        /// </summary>
        public double Temperature
        {
            set;
            get;
        }
    }
}
