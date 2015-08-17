using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.Realtime
{
    public class CRealtimeTemperatureCollection : ObservableCollection<CRealtimeTemperaturePoint>
    {
        double temper_sum = 0;
        int current_minute = 0;

        /// <summary>
        /// 保存每秒的原始数据
        /// </summary>
        List<CRealtimeTemperaturePoint> _raw_list;

        public CRealtimeTemperatureCollection():base()
        {
            _raw_list = new List<CRealtimeTemperaturePoint>();
        }

        /// <summary>
        /// 重写Add方法用来产生一个approximated集合
        /// 去掉集合中秒对齐的元素，变成分钟对齐，以减小Chart Render压力，提高显示速度
        /// 判断新插入的Point是否属于当前分钟的，如果属于，不要显示，直接累加，一直到分钟数改变时，平均这一分钟内的温度，再进行显示
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, CRealtimeTemperaturePoint item)
        {
            if ((int)item.PickTime.TimeOfDay.TotalMinutes == current_minute)
            {
                temper_sum += item.Temperature;
            }
            else
            {
                temper_sum /= 60;
                CRealtimeTemperaturePoint point = new CRealtimeTemperaturePoint(current_minute * 60, temper_sum);
                base.InsertItem(index, point);

                temper_sum = item.Temperature;
                current_minute = (int)(item.PickTime - Convert.ToDateTime("2000-01-01 00:00:00")).TotalMinutes;
            }

            _raw_list.Insert(index, item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            _raw_list.Clear();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            _raw_list.RemoveAt(index);
        }

        /// <summary>
        /// 返回原始列表
        /// </summary>
        public List<CRealtimeTemperaturePoint> RawList
        {
            get
            {
                return _raw_list;
            }
        }
    }
}
