using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence
{
    [Serializable]
    public class CTemperatureSequence : ObservableCollection<CTemperatureSequenceKeyPoint>, ICloneable, INotifyPropertyChanged
    {

        string seq_name = string.Empty;

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
            base.OnCollectionChanged(e);
            UpdateStartTime();
        }

        /// <summary>
        /// 设置或返回预设温度曲线的名称
        /// </summary>
        public string SequenceName
        {
            get
            {
                return seq_name;
            }
            set
            {
                seq_name = value;

            }
        }

        /// <summary>
        /// 重新计算起始时间
        /// </summary>
        public void UpdateStartTime()
        {
            DateTime time = Convert.ToDateTime("2000-01-01 00:00:00");
            for(int i = 0; i < this.Count; i++)
            {
                if(i == 0)
                {
                    this[0].StartTime = time;
                }
                else
                {
                    /* 
                    * HoldTime == 0说明已经到达序列尾部，最后一个项并没有实际意义，仅为绘制曲线考虑
                    * 因为采用阶梯线，所以最后一个温度序列需要多画一段持续的横线，最后一个项就是为了绘制多出来的一条横线
                    */
                    if (this[i].HoldTime == 0)

                        // TODO: Needs to be optimized
                        /*
                        * 为什么此处-1呢？
                        * 因为绘制实时曲线时是从0分钟开始，而不是从1分钟；如果不-1，则预设曲线会比实时曲线多出1分钟
                        * 例如需要工作60分钟，实时温度曲线采集是0~59分钟，而预设温度曲线是0~60分钟显示，所以预设会多出一分钟
                        * 其实正确的方法是实时温度采集1~60分钟，预设温度曲线显示1~60分钟，均从1分钟开始，但改动比较复杂，所以先这样
                        */
                        this[i].StartTime = this[i - 1].StartTime.AddMinutes(this[i - 1].HoldTime - 1);
                    else
                        this[i].StartTime = this[i - 1].StartTime.AddMinutes(this[i - 1].HoldTime); 
                }
            }
        }

        /// <summary>
        /// 当前消耗的时间属于哪个阶段
        /// </summary>
        /// <param name="CureElapsed">当前治疗消耗的时间（秒）</param>
        /// <param name="LastStage">上次的阶段</param>
        /// <param name="CurrentStage">返回当前阶段</param>
        /// <param name="StageChanged">阶段是否改变</param>
        /// <param name="TimeRemained">当前阶段剩余的秒数</param>
        /// <param name="ProgressInStage">当前阶段完成的进度,以%为单位</param>
        public void WhichStageBelongTo(int CureElapsed, int LastStage, out int CurrentStage, out bool StageChanged, out int TimeRemained, out double ProgressInStage)
        {
            double elapsed_in_stage = 0;

            ProgressInStage = 0;
            CurrentStage = -1;
            StageChanged = false;
            TimeRemained = 0;

            for (int i = 0; i < this.Count; i++)
            {
                int total_sec = (int)this[i].StartTime.TimeOfDay.TotalSeconds + (int)(this[i].HoldTime * 60);   // 第i阶段的耗时，用于判断当前CureElasped时间是否在i阶段内
                if(total_sec < CureElapsed)  // 如果已经消耗的秒数大于到达阶段i需要的秒数，则说明当前阶段≥i ，需要继续找下去
                {
                    continue;
                }
                else
                {
                    CurrentStage = i;

                    elapsed_in_stage = (CureElapsed - (int)this[CurrentStage].StartTime.TimeOfDay.TotalSeconds);

                    /* 计算本阶段完成的进度 */
                    ProgressInStage = elapsed_in_stage / (this[CurrentStage].HoldTime * 60) * 100;

                    // 本阶段还剩余的秒数
                    TimeRemained = (int)(this[CurrentStage].HoldTime * 60 - elapsed_in_stage);

                    /* 判断是否进入下一个阶段 */
                    if (CurrentStage != LastStage)
                        StageChanged = true;
                    else
                        StageChanged = false;

                    break;
                }
            }
        }

        public object Clone()
        {
            CTemperatureSequence seq = new CTemperatureSequence();
            foreach(CTemperatureSequenceKeyPoint item in this)
            {
                seq.Add(item);
            }

            return seq;
        }
    }
}
