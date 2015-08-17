using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.Realtime;

namespace Tai_Shi_Xuan_Ji_Yi.Controls
{
    /// <summary>
    /// StepAreaAndLineChart.xaml 的交互逻辑
    /// </summary>
    public partial class StepAreaAndLineChart : UserControl
    {
        
        public StepAreaAndLineChart()
        {
            InitializeComponent();
            
            //for (int i = 0; i < 10; i++)
            //{
            //    this.RealtimeTemperatureCollection.Add(new CRealtimeTemperaturePoint(i, 40 + i)); 
            //}
        }


        /// <summary>
        /// 设置或返回预设的温度序列列表
        /// </summary>
        public CTemperatureSequence PresettedSequence
        {
            get { return (CTemperatureSequence)GetValue(PresettedSequenceProperty); }
            set { SetValue(PresettedSequenceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PresettedSequence.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresettedSequenceProperty =
            DependencyProperty.Register("PresettedSequence", typeof(CTemperatureSequence), typeof(StepAreaAndLineChart), new PropertyMetadata(OnPresettedSequenceChanged));


        private static void OnPresettedSequenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StepAreaAndLineChart owner = d as StepAreaAndLineChart;

            /*
            * 重新生成一个CTemperatureSequence对象，这个对象和传入的对象区别是：
            * 重新生成的CTemperatureSequence对象会在尾部多加一个Point，这个Point用来
            * 正确产生Step Series的最后一个横阶梯 
            */

            CTemperatureSequence seq;

            if (e.NewValue == null)
                seq = new CTemperatureSequence();
            else
            {
                seq = ((CTemperatureSequence)e.NewValue).Clone() as CTemperatureSequence;
                if (seq.Count > 0)
                {
                    /* 这里在最后加入一个Point用来修正Step Chart的View */
                    CTemperatureSequenceKeyPoint point = seq[seq.Count - 1];
                    seq.Add(new CTemperatureSequenceKeyPoint() {HoldTime = 0, TargetTemperature = point.TargetTemperature });
                }
            }
            owner._SeriesSequence.DataSource = seq;

        }


        /// <summary>
        /// 设置或返回实时温度列表
        /// </summary>
        public CRealtimeTemperatureCollection RealtimeTemperatureCollection
        {
            get { return (CRealtimeTemperatureCollection)GetValue(RealtimeTemperatureCollectionProperty); }
            set { SetValue(RealtimeTemperatureCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RealtimeTemperatureCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RealtimeTemperatureCollectionProperty =
            DependencyProperty.Register("RealtimeTemperatureCollection", typeof(CRealtimeTemperatureCollection), typeof(StepAreaAndLineChart), new PropertyMetadata());


        /// <summary>
        /// 设置或返回实时温度曲线的可见性
        /// </summary>
        public bool RealtimeTemperatureVisible
        {
            get { return (bool)GetValue(RealtimeTemperatureVisibleProperty); }
            set { SetValue(RealtimeTemperatureVisibleProperty, value); }
        }
   
        // Using a DependencyProperty as the backing store for RealtimeTemperatureVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RealtimeTemperatureVisibleProperty =
            DependencyProperty.Register("RealtimeTemperatureVisible", typeof(bool), typeof(StepAreaAndLineChart), new PropertyMetadata(true, OnRealtimeTemperatureVisibleChanged));

        private static void OnRealtimeTemperatureVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StepAreaAndLineChart _owner = d as StepAreaAndLineChart;
            _owner._SeriesRealtime.Visible = (bool)e.NewValue;
        }
    }
}
