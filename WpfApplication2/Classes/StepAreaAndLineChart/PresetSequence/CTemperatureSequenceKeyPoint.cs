using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence
{
    /// <summary>
    /// 温度控制序列关键点
    /// </summary>
    [Serializable]
    public class CTemperatureSequenceKeyPoint : Classes.CommonBase, IDXDataErrorInfo 
    {
        DateTime _StartTime;
        double _TargetTemperature;
        double _HoldTime;

        #region IDXDataErrorInfo
        void IDXDataErrorInfo.GetError(ErrorInfo info)
        {
        }

        void SetErrorInfo(ErrorInfo info, string errorText, ErrorType errorType)
        {
            info.ErrorText = errorText;
            info.ErrorType = errorType;
        }

        void IDXDataErrorInfo.GetPropertyError(string propertyName, ErrorInfo info)
        {
            switch (propertyName)
            {
                case "TargetTemperature":
                    if (TargetTemperature < 25 || TargetTemperature > 70)
                        SetErrorInfo(info,
                            "目标温度必须在25℃至70℃之间",
                            ErrorType.Critical);
                    break;
                case "HoldTime":
                    if (HoldTime < 10 || HoldTime > 120)
                        SetErrorInfo(info,
                            "保持时间必须在10分钟至120分钟之间",
                            ErrorType.Critical);
                    break;
            }
        }
        #endregion

        #region Constructors
        public  CTemperatureSequenceKeyPoint()
        {

        }


        public CTemperatureSequenceKeyPoint(double Temperature, int Hold)
        {
            _StartTime = DateTime.Now;
            _TargetTemperature = Temperature;
            _HoldTime = (double)Hold;

        }
        #endregion

        /// <summary>
        /// 设置或返回启动时间
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                _StartTime = value;
                RaisePropertyChangedEvent("StartTime");
            }
        }


        /// <summary>
        /// 设置或返回目标温度，单位：℃
        /// </summary>
        public double TargetTemperature
        {
            get
            {
                return _TargetTemperature;
            }
            set
            {
                _TargetTemperature = value;
                RaisePropertyChangedEvent("TargetTemperature");
            }
        }

        /// <summary>
        /// 设置或返回目标温度保持时间，单位：分钟
        /// </summary>
        public double HoldTime
        {
            get
            {
                return _HoldTime;
            }
            set
            {
                _HoldTime = value;
                RaisePropertyChangedEvent("HoldTime");
            }
        }
        
        /// <summary>
        /// 重构ToString()方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder ss = new StringBuilder();
            ss.Append("目标温度=");
            ss.Append(TargetTemperature.ToString());
            ss.Append("℃,保持时间=");
            ss.Append(HoldTime.ToString());
            ss.Append("分钟");
            return ss.ToString();
        }

    }
}
