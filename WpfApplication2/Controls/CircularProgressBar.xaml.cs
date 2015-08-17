using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Expression.Shapes;
using System.Drawing;

namespace ZhongLiuCang
{
	/// <summary>
	/// CircularProgressBar.xaml 的交互逻辑
	/// </summary>
	public partial class CircularProgressBar : UserControl
    {
        #region Variables
        DoubleAnimation animation;
        AnimationClock animationclock;
        #endregion

        public CircularProgressBar()
		{
			this.InitializeComponent();
        }

        #region Dependency Properties
        #region Max

        /// <summary>
        /// 设置或获取进度最大值
        /// </summary>
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(CircularProgressBar), new FrameworkPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxChanged)));

        private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularProgressBar own = d as CircularProgressBar;
            double value = (double)e.NewValue;

            if (value < own.Min)
            {
                own.Max = own.Min;
            }
            else
            {
                own.DrawAngle();
            }

            if (value < own.Progress)
                own.Progress = value;
        }
        #endregion

        #region DP - Min

        /// <summary>
        /// 设置或获取最小值
        /// </summary>
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(CircularProgressBar), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnMinChanged)));

        private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularProgressBar own = d as CircularProgressBar;
            double value = (double)e.NewValue;

            if (value >= own.Max)
            {
                own.Min = own.Max;
            }
            else
            {
                own.DrawAngle();
            }

            if (value > own.Progress)
                own.Progress = value;
        }
        #endregion

        #region DP - Progress

        /// <summary>
        /// 设置或获取进度值
        /// </summary>
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(CircularProgressBar), new FrameworkPropertyMetadata(50.0, new PropertyChangedCallback(OnProgressChanged)));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularProgressBar own = d as CircularProgressBar;
            double value = (double)e.NewValue;
            if(value <= own.Min)
            {
                own.Progress = own.Min;
            }
            else if (value > own.Max)
            {
                own.Progress = own.Max;
            }
                
            own.DrawAngle();
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// 绘制进度条的圆弧
        /// </summary>
        void DrawAngle()
        {
            double angle = 0.1;
            double percent = 0;

            if (this.Min == this.Max)
            {
                angle = 0.1;
                percent = 0;
            }
            else
            {
                angle = (this.Progress - this.Min) / (this.Max - this.Min) * 360; // 计算圆弧绘制的弧度
                if (angle <= 0)
                    angle = 0.1;

                percent = (this.Progress - this.Min) / (this.Max - this.Min) * 100;  // 计算百分比 
                if (percent <= 0)
                    percent = 0;
            }

            // 绘制圆弧用到的动画
            QuarticEase ease = new QuarticEase() { EasingMode = EasingMode.EaseOut };
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animation.To = angle;
            animation.EasingFunction = ease;
            animationclock = animation.CreateClock();
            animationclock.Completed += animationclock_Completed;

            // 绘制圆弧
            this.ProgressIndicator.ApplyAnimationClock(Arc.EndAngleProperty, animationclock, HandoffBehavior.SnapshotAndReplace);
            this.txt_ProgressValue.Text = string.Format("{0}%", percent.ToString("F1"));
        }

        void animationclock_Completed(object sender, EventArgs e)
        {
            this.ProgressIndicator.EndAngle = this.ProgressIndicator.EndAngle;
            animationclock.Controller.Stop();
        }
        #endregion
    }
}