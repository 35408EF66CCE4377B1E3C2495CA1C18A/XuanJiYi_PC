using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence;

namespace Tai_Shi_Xuan_Ji_Yi.Controls
{
    public class ViewModel_TemperatureSequenceEditor : Classes.CommonBase
    {
        CTemperatureSequence _Sequence;
        string strLastErr = string.Empty;

        public ViewModel_TemperatureSequenceEditor()
        {
            /*
            _Sequence = new CTemperatureSequence();
            _Sequence.Add(new CTemperatureSequenceKeyPoint() { StartTime = DateTime.Now, HoldTime = 30, TargetTemperature = 41 });
            _Sequence.Add(new CTemperatureSequenceKeyPoint() { StartTime = DateTime.Now, HoldTime = 32, TargetTemperature = 42 });
            _Sequence.Add(new CTemperatureSequenceKeyPoint() { StartTime = DateTime.Now, HoldTime = 33, TargetTemperature = 43 });
            _Sequence.Add(new CTemperatureSequenceKeyPoint() { StartTime = DateTime.Now, HoldTime = 34, TargetTemperature = 44 });
            _Sequence.Add(new CTemperatureSequenceKeyPoint() { StartTime = DateTime.Now, HoldTime = 35, TargetTemperature = 41 });
           */
           //new CDatabase().GetTemperatureSequence("测试1", out _Sequence);
            
            _Sequence = new CTemperatureSequence();
        }

        /// <summary>
        /// Load sequence with name
        /// </summary>
        /// <param name="SeqName"></param>
        /// <returns></returns>
        public bool LoadSequence(string SeqName)
        {
            bool ret = false;
            using(CDatabase db = new CDatabase())
            {
                if(db.GetTemperatureSequence(SeqName, out _Sequence))
                {
                    this.SequenceName = SeqName;
                    ret = true;
                }
                else
                {
                    strLastErr = db.LastError;
                    ret = false;
                }
            }
            RaisePropertyChangedEvent("Sequence");
            return ret;
        }

        /// <summary>
        /// 设置或获取温度序列
        /// </summary>
        public CTemperatureSequence Sequence
        {
            get
            {
                if(_Sequence == null)
                {
                    _Sequence = new CTemperatureSequence();
                }
                return _Sequence;
            }
            set
            {
                _Sequence = value;
                RaisePropertyChangedEvent("Sequence");
            }
        }

        /// <summary>
        /// 设置或获取温度序列名称
        /// </summary>
        public string SequenceName
        {
            set;
            get;
        }
    }

}
