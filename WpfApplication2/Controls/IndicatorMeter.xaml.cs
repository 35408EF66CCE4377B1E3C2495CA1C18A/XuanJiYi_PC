using Microsoft.Expression.Shapes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ZhongLiuCang
{
	/// <summary>
	/// IndicatorMeter.xaml 的交互逻辑
	/// </summary>
	public partial class IndicatorMeter : UserControl
    {

        #region Variables
        DoubleAnimation animation;
        AnimationClock animationclock;
        #endregion

        #region Constructors
        public IndicatorMeter()
		{
			this.InitializeComponent();

            /* 给IndicatorBarPattern属性分配一个新的List，否则所有的IndicatorMeter实例将共享同一个List
             * 参考 http://msdn.microsoft.com/zh-cn/library/aa970563%28v=vs.110%29.aspx 
             */
            SetValue(IndicatorBarPatternProperty, new List<Color_Value>());

            Loaded += delegate
            {
                DrawScale(PrimaryScaleRaduisInner, PrimaryScaleRadiusOuter, PrimaryScaleCount, MinorScaleRadiusInner, MinorScaleRadiusOuter, MinorScaleCount, PrimaryScaleStartAngle, PrimaryScaleEndAngle, PrimaryScaleWitdh, PrimaryScaleBackground);
            };
            
        }
        #endregion

        #region Properties
        /********************************* Biu Biu Biu ***********************************/

        #region DP Min
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnMinChanged)));

        private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;

            /* 检查 Min 是否超过了 Max，如果超过，会导致绘制IndicatorBar时出错 */
            if (owner.Min > owner.Max)
            {
                owner.Min = owner.Max;
                return;
            }

            if (owner.Min > owner.IndicatorValue)
            {
                owner.IndicatorValue = owner.Min;
            }

            if (owner.Min > owner.RangeBarMin)
            {
                owner.RangeBarMin = owner.Min;
            }

            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);

            // 更改Indicator Bar颜色
            owner.ChangeIndicatorBarBackground();
        }
        #endregion

        #region DP Max
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnMaxChanged)));

        private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;

            if (owner.Max <= owner.Min)
            {
                owner.Max = owner.Min;
                return;
            }

            if (owner.Max < owner.IndicatorValue)
                owner.IndicatorValue = owner.Max;

            if (owner.Max < owner.RangeBarMax)
                owner.RangeBarMax = owner.Max;

            

            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);

            // 更改Indicator Bar颜色
            owner.ChangeIndicatorBarBackground();
        }
        #endregion

        #region DP IndicatorValue
        public double IndicatorValue
        {
            get { return (double)GetValue(IndicatorValueProperty); }
            set { SetValue(IndicatorValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorValueProperty =
            DependencyProperty.Register("IndicatorValue", typeof(double), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnIndicatorValueChanged)));

        private static void OnIndicatorValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double indicator_bar_angle;

            IndicatorMeter owner = d as IndicatorMeter;
            double value = (double)e.NewValue;

            // 显示指示值文本
            if (double.IsNaN(value))
            {
                owner.txt_Value.Text = "N/A";
                indicator_bar_angle = 0;    // 如果输入的值是double.NaN，则指示条指向最小值
            }
            else
            {

                if (value > owner.Max)
                {
                    owner.IndicatorValue = owner.Max;
                    return;
                }

                if (value < owner.Min)
                {
                    owner.IndicatorValue = owner.Min;
                    return;
                }


                owner.txt_Value.Text = value.ToString();
                indicator_bar_angle = 280 * (owner.IndicatorValue - owner.Min) / (owner.Max - owner.Min); // 根据指示值计算指示条的角度
                /* 如果 Min == Max，则会出现 Max - Min = 0 的情况，此时indicator_bar_angle == double.NaN */
                if (double.IsNaN(indicator_bar_angle))
                {
                    indicator_bar_angle = 0;
                }
            }


            // 绘制Indicator Bar
            owner.BeginIndicatorValueChangedAnimation(indicator_bar_angle);

            // 更改Indicator Bar颜色
            owner.ChangeIndicatorBarBackground();
        }
        #endregion

        #region DP IndicatorValueFontSize


        public double IndicatorValueFontSize
        {
            get { return (double)GetValue(IndicatorValueFontSizeProperty); }
            set { SetValue(IndicatorValueFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorValueFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorValueFontSizeProperty =
            DependencyProperty.Register("IndicatorValueFontSize", typeof(double), typeof(IndicatorMeter), new PropertyMetadata(12.0));

        
        #endregion

        #region DP UnitString
        public string UnitString
        {
            get { return (string)GetValue(UnitStringProperty); }
            set { SetValue(UnitStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitStringProperty =
            DependencyProperty.Register("UnitString", typeof(string), typeof(IndicatorMeter),
            new PropertyMetadata("℃"));

        #endregion

        #region DP UnitStringFontSize
        /// <summary>
        /// 设置或返回单位文本的字体大小
        /// </summary>
        public double UnitFontSize
        {
            get { return (double)GetValue(UnitFontSizeProperty); }
            set { SetValue(UnitFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitFontSizeProperty =
            DependencyProperty.Register("UnitFontSize", typeof(double), typeof(IndicatorMeter), new PropertyMetadata(12.0));

        #endregion

        #region DP RangeBarVisible


        public Visibility RangeBarVisible
        {
            get { return (Visibility)GetValue(RangeBarVisibleProperty); }
            set { SetValue(RangeBarVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeBarVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeBarVisibleProperty =
            DependencyProperty.Register("RangeBarVisible", typeof(Visibility), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnRangeBarVisibleChanged)));

        private static void OnRangeBarVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            owner.RangeBar.Visibility = (Visibility)e.NewValue;
        }
        #endregion

        #region DP RangeBarMin
        public double RangeBarMin
        {
            get { return (double)GetValue(RangeBarMinProperty); }
            set { SetValue(RangeBarMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeBarMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeBarMinProperty =
            DependencyProperty.Register("RangeBarMin", typeof(double), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnRangeBarMinChanged)));

        private static void OnRangeBarMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            double min = (double)e.NewValue;

            if(min < owner.Min)
            {
                owner.RangeBarMin = owner.Min;
                return;
            }

            if(min > owner.Max)
            {
                owner.RangeBarMin = owner.RangeBarMax;
                return;
            }

            double div = (min - owner.Min) / (owner.Max - owner.Min);
            double angle = 280 * div;

            owner.RangeBar.StartAngle = angle;

            owner.ChangeIndicatorBarBackground();
        }

        #endregion

        #region DP RangeBarMax
        public double RangeBarMax
        {
            get { return (double)GetValue(RangeBarMaxProperty); }
            set { SetValue(RangeBarMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeBarMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeBarMaxProperty =
            DependencyProperty.Register("RangeBarMax", typeof(double), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnRangeBarMaxChanged)));

        private static void OnRangeBarMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            double max = (double)e.NewValue;

            if(max > owner.Max)
            {
                owner.RangeBarMax = owner.Max;
                return;
            }

            if (max < owner.Min)
            {
                owner.RangeBarMax = owner.RangeBarMin;
                return;
            }

            double div = (max - owner.Min) / (owner.Max - owner.Min);
            double angle = 280 * div;

            owner.RangeBar.EndAngle = angle;

            owner.ChangeIndicatorBarBackground();
        }

        #endregion

        #region DP RangeBarBackground

        /// <summary>
        /// 设置或返回RangeBar背景颜色
        /// </summary>
        public SolidColorBrush RangeBarBackground
        {
            get { return (SolidColorBrush)GetValue(RangeBarBackgroundProperty); }
            set { SetValue(RangeBarBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeBarBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeBarBackgroundProperty =
            DependencyProperty.Register("RangeBarBackground", typeof(SolidColorBrush), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnRangeBarBackgroundChanged)));

        private static void OnRangeBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            owner.RangeBar.Stroke = e.NewValue as SolidColorBrush;
        }

        
        #endregion

        #region DP IndicatorBarBackground
        /// <summary>
        /// 默认的Indicator Bar背景颜色
        /// </summary>
        public SolidColorBrush IndicatorBarBackground
        {
            get { return (SolidColorBrush)GetValue(IndicatorBarBackgroundProperty); }
            set { SetValue(IndicatorBarBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorBarBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorBarBackgroundProperty =
            DependencyProperty.Register("IndicatorBarBackground", typeof(SolidColorBrush), typeof(IndicatorMeter), 
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnIndicatorBarBackgroundChanged)));

        private static void OnIndicatorBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            owner.ChangeIndicatorBarBackground();
        }
        #endregion

        #region DP IndicatorBar Background Pattern

        /// <summary>
        /// Indicator Bar背景颜色模板
        /// Indicator Bar的背景颜色会根据指示值的不同而变色
        /// </summary>
        public List<Color_Value> IndicatorBarPattern
        {
            get { return (List<Color_Value>)GetValue(IndicatorBarPatternProperty); }
            set { SetValue(IndicatorBarPatternProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorBarPattern.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorBarPatternProperty =
            DependencyProperty.Register("IndicatorBarPattern", typeof(List<Color_Value>), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(new List<Color_Value>(), new PropertyChangedCallback(OnIndicatorBarPatternChanged)));



        private static void OnIndicatorBarPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            owner.IndicatorBarPattern.Sort(SortCompare);
            owner.ChangeIndicatorBarBackground();
        }

        /// <summary>
        /// 对List<IndicatorBarColorPattern>进行排序的函数
        /// </summary>
        /// <param name="Arg1"></param>
        /// <param name="Arg2"></param>
        /// <returns></returns>
        private static int SortCompare(Color_Value Arg1, Color_Value Arg2)
        {
            if (Arg1.Value < Arg2.Value)
                return -1;
            else if (Arg1.Value > Arg2.Value)
                return 1;
            else
                return 0;
        }

        
        #endregion

        #region DP Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(IndicatorMeter), 
            new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            owner.txt_Title.Text = e.NewValue.ToString();
        }

        #endregion

        #region DP Title Visibility


        public Visibility TitleVisibility
        {
            get { return (Visibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleVisibilityProperty =
            DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(IndicatorMeter), 
            new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnTitleVisibilityChanged)));

        private static void OnTitleVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            owner.txt_Title.Visibility = (Visibility)e.NewValue;
        }

        
        #endregion

        /********************************* Biu Biu Biu ***********************************/
        #endregion

        #region Methods
        /// <summary>
        /// 播放指示条动画
        /// </summary>
        /// <param name="Angle"></param>
        void BeginIndicatorValueChangedAnimation(double Angle)
        {
            if (Angle < 5)  // 如果EndAngle小于5度，则直接显示角度值，QuinticEase会造成过冲，EndAngle会变成负值，造成闪动
            {
                /* 如果上一个动画还在执行中，则先停止动画 
                 * 否则执行中的动画可能再次更改EndAngle
                 */
                if (animationclock != null && animationclock.CurrentState != ClockState.Stopped)
                    animationclock.Controller.Stop();

                IndicatorBar.EndAngle = Angle;
            }
            else
            {
                animation = new DoubleAnimation();
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                animation.To = Angle;

                // QuinticEase ease = new QuinticEase();
                ElasticEase ease = new ElasticEase() { Oscillations = 2, Springiness = 5 };
                ease.EasingMode = EasingMode.EaseOut;
                animation.EasingFunction = ease;

                animationclock = animation.CreateClock();
                animationclock.Completed += animationclock_Completed;

                IndicatorBar.ApplyAnimationClock(Arc.EndAngleProperty, animationclock, HandoffBehavior.SnapshotAndReplace);
                animationclock.Controller.Begin();
            }
        }

        void animationclock_Completed(object sender, EventArgs e)
        {
            /* 为什么动画完成后需要再次对EndAngle赋值？
             * 动画对目标属性的更改仅限在对象的绘制过程有效，并不会真正改变目标属性值。
             * 在本例中动画更改了Arc对象的EndAngle属性，当动画作用于Arc对象时，动画可以保持EndAngle属性最终的值，而当调用了
             * 动画的Stop()方法后，EndAngle属性将会恢复其初值。
             * 因此，当从对象释放动画时，需要对目标属性进行一次赋值，使其保持最终的值
             * */
            IndicatorBar.EndAngle = IndicatorBar.EndAngle;
            animationclock.Controller.Stop();
        }

        /// <summary>
        /// 根据当前的指示值和颜色模板更改Indicator Bar背景色
        /// </summary>
        void ChangeIndicatorBarBackground()
        {
            Color_Value previous_pattern = new Color_Value();
            Color_Value next_pattern = new Color_Value();

            /* 如果没有设置颜色模式列表，则用默认的Indicator Bar背景色绘制 */
            if(this.IndicatorBarPattern.Count == 0)
            {
                this.IndicatorBar.Stroke = this.IndicatorBarBackground;
                return;
            }

            /* 利用颜色模式列表绘制Indicator Bar背景 */
            for (int i = 0; i < this.IndicatorBarPattern.Count; i++)
            {
                if (this.IndicatorValue < this.IndicatorBarPattern[i].Value)
                {
                    /* 根据指示值获取Indicator Bar将要在哪两个颜色之间变色 */
                    if (i == 0)
                    {
                        previous_pattern.Background = this.IndicatorBarBackground;
                        previous_pattern.Value = this.Min;
                    }
                    else
                    {
                        previous_pattern = this.IndicatorBarPattern[i - 1];

                    }

                    next_pattern = this.IndicatorBarPattern[i];

                    /* 根据指示值和颜色模式列表计算Indicator Bar的背景色RGB分量 */
                    byte color_r =(byte)(
                        (this.IndicatorValue - previous_pattern.Value) / (next_pattern.Value - previous_pattern.Value) *
                        (next_pattern.Background.Color.R - previous_pattern.Background.Color.R) + previous_pattern.Background.Color.R);

                    byte color_g = (byte)(
                        (this.IndicatorValue - previous_pattern.Value) / (next_pattern.Value - previous_pattern.Value) *
                        (next_pattern.Background.Color.G - previous_pattern.Background.Color.G) + previous_pattern.Background.Color.G);

                    byte color_b = (byte)(
                        (this.IndicatorValue - previous_pattern.Value) / (next_pattern.Value - previous_pattern.Value) *
                        (next_pattern.Background.Color.B - previous_pattern.Background.Color.B) + previous_pattern.Background.Color.B);

                    this.IndicatorBar.Stroke = new SolidColorBrush(Color.FromRgb(color_r, color_g, color_b));

                    return;
                }
            }

            /* 如果遍历完颜色模式列表还没有发现匹配的范围，则说明指示值超过模式列表的最大值，则用模式列表的最后一项绘制Indicator Bar背景 */
            this.IndicatorBar.Stroke = this.IndicatorBarPattern[this.IndicatorBarPattern.Count - 1].Background;
        }
        #endregion

        #region DrawScale

        /// <summary>
        /// 主刻度外圈的半径
        /// </summary>
        public double PrimaryScaleRadiusOuter
        {
            get { return (double)GetValue(PrimaryScaleRadiusOuterProperty); }
            set { SetValue(PrimaryScaleRadiusOuterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleRadiusOuter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleRadiusOuterProperty =
            DependencyProperty.Register("PrimaryScaleRadiusOuter", typeof(double), typeof(IndicatorMeter), 
            new FrameworkPropertyMetadata(55.0, new PropertyChangedCallback(OnPrimaryScaleRadiusOuterChanged)));

        private static void OnPrimaryScaleRadiusOuterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            double val = (double)e.NewValue;

            /* 如果外圈半径小于内圈半径，则令外圈半径等于内圈半径 */
            if (val <= owner.PrimaryScaleRaduisInner)
            {
                owner.PrimaryScaleRadiusOuter = owner.PrimaryScaleRaduisInner;
                return;
            }
            else
            {
                /* 属性更改后需要立刻重绘刻度 */
                owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
            }
        }

        /// <summary>
        /// 主刻度内圈的半径
        /// </summary>
        public double PrimaryScaleRaduisInner
        {
            get { return (double)GetValue(PrimaryScaleRaduisInnerProperty); }
            set { SetValue(PrimaryScaleRaduisInnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleRaduis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleRaduisInnerProperty =
            DependencyProperty.Register("PrimaryScaleRaduisInner", typeof(double), typeof(IndicatorMeter), 
            new FrameworkPropertyMetadata(45.0, new PropertyChangedCallback(OnPrimaryScaleRaduisInnerChanged)));

        private static void OnPrimaryScaleRaduisInnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            double val = (double)e.NewValue;

            /* 如果内圈半径大于外圈半径，则令内圈半径等于外圈半径 */
            if (val >= owner.PrimaryScaleRadiusOuter)
            {
                owner.PrimaryScaleRaduisInner = owner.PrimaryScaleRadiusOuter;
                return;
            }
            else
            {
                /* 属性更改后需要立刻重绘刻度 */
                owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
            }
        }

        /// <summary>
        /// 设置主刻度的起始角度
        /// 三点方向为0度，角度按逆时针方向增加
        /// </summary>
        public double PrimaryScaleStartAngle
        {
            get { return (double)GetValue(PrimaryScaleStartAngleProperty); }
            set { SetValue(PrimaryScaleStartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleStartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleStartAngleProperty =
            DependencyProperty.Register("PrimaryScaleStartAngle", typeof(double), typeof(IndicatorMeter), new FrameworkPropertyMetadata(130.0, new PropertyChangedCallback(OnPrimaryScaleStartAngleChanged)));

        private static void OnPrimaryScaleStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }


        /// <summary>
        /// 设置主刻度的终止角度
        /// 三点方向为0度，角度按逆时针方向增加
        /// </summary>
        public double PrimaryScaleEndAngle
        {
            get { return (double)GetValue(PrimaryScaleEndAngleProperty); }
            set { SetValue(PrimaryScaleEndAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrimaryScaleEndAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleEndAngleProperty =
            DependencyProperty.Register("PrimaryScaleEndAngle", typeof(double), typeof(IndicatorMeter), new FrameworkPropertyMetadata(410.0, new PropertyChangedCallback(OnPrimaryScaleEndAngleChanged)));

        private static void OnPrimaryScaleEndAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }

        /// <summary>
        /// 主刻度线的宽度
        /// </summary>
        public double PrimaryScaleWitdh
        {
            get { return (double)GetValue(PrimaryScaleWitdhProperty); }
            set { SetValue(PrimaryScaleWitdhProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleWitdh.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleWitdhProperty =
            DependencyProperty.Register("PrimaryScaleWitdh", typeof(double), typeof(IndicatorMeter), new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(OnPrimaryScaleWidthChanged)));

        private static void OnPrimaryScaleWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }


        /// <summary>
        /// 主刻度线数量（刻度格数）
        /// </summary>
        public int PrimaryScaleCount
        {
            get { return (int)GetValue(PrimaryScaleCountProperty); }
            set { SetValue(PrimaryScaleCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleCountProperty =
            DependencyProperty.Register("PrimaryScaleCount", typeof(int), typeof(IndicatorMeter), new FrameworkPropertyMetadata(5, new PropertyChangedCallback(OnPrimaryScaleCountChanged)));

        private static void OnPrimaryScaleCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }

        /// <summary>
        /// 主刻度线颜色
        /// </summary>
        public Brush PrimaryScaleBackground
        {
            get { return (Brush)GetValue(PrimaryScaleBackgroundProperty); }
            set { SetValue(PrimaryScaleBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleBackgroundProperty =
            DependencyProperty.Register("PrimaryScaleBackground", typeof(Brush), typeof(IndicatorMeter), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnPrimaryScaleBackgroundChanged)));

        private static void OnPrimaryScaleBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }

        /// <summary>
        /// 主刻度标签是否可见
        /// </summary>
        public Visibility PrimaryScaleLabelVisibility
        {
            get { return (Visibility)GetValue(PrimaryScaleLabelVisibilityProperty); }
            set { SetValue(PrimaryScaleLabelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrimaryScaleLabelVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleLabelVisibilityProperty =
            DependencyProperty.Register("PrimaryScaleLabelVisibility", typeof(Visibility), typeof(IndicatorMeter), 
            new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnPrimaryScaleLabelVisibility)));

        private static void OnPrimaryScaleLabelVisibility(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }

        /// <summary>
        /// 主刻度标签所在圆的半径
        /// </summary>
        public double PrimaryScaleLabelRadius
        {
            get { return (double)GetValue(PrimaryScaleLabelRadiusProperty); }
            set { SetValue(PrimaryScaleLabelRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrimaryScaleLabelRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryScaleLabelRadiusProperty =
            DependencyProperty.Register("PrimaryScaleLabelRadius", typeof(double), typeof(IndicatorMeter), 
            new FrameworkPropertyMetadata(63.0, new PropertyChangedCallback(OnPrimaryScaleLabelRadiusChanged)));

        private static void OnPrimaryScaleLabelRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }


        /// <summary>
        /// 次刻度外圆半径
        /// </summary>
        public double MinorScaleRadiusOuter
        {
            get { return (double)GetValue(MinorScaleRadiusOuterProperty); }
            set { SetValue(MinorScaleRadiusOuterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorScaleRadiusOuterProperty =
            DependencyProperty.Register("MinorScaleRadiusOuter", typeof(double), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(50.0, new PropertyChangedCallback(OnMinorScaleRadiusOuterChanged)));

        private static void OnMinorScaleRadiusOuterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }


        /// <summary>
        /// 次刻度内圆半径
        /// </summary>
        public double MinorScaleRadiusInner
        {
            get { return (double)GetValue(MinorScaleRadiusInnerProperty); }
            set { SetValue(MinorScaleRadiusInnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorScaleRadiusInner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorScaleRadiusInnerProperty =
            DependencyProperty.Register("MinorScaleRadiusInner", typeof(double), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(45.0, new PropertyChangedCallback(OnMinorScaleRadiusInnerChanged)));

        private static void OnMinorScaleRadiusInnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }



        public int MinorScaleCount
        {
            get { return (int)GetValue(MinorScaleCountProperty); }
            set { SetValue(MinorScaleCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorScaleCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorScaleCountProperty =
            DependencyProperty.Register("MinorScaleCount", typeof(int), typeof(IndicatorMeter),
            new FrameworkPropertyMetadata(5, new PropertyChangedCallback(OnMinorScaleCountChanged)));

        private static void OnMinorScaleCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorMeter owner = d as IndicatorMeter;
            /* 属性更改后需要立刻重绘刻度 */
            owner.DrawScale(owner.PrimaryScaleRaduisInner, owner.PrimaryScaleRadiusOuter, owner.PrimaryScaleCount, owner.MinorScaleRadiusInner, owner.MinorScaleRadiusOuter, owner.MinorScaleCount, owner.PrimaryScaleStartAngle, owner.PrimaryScaleEndAngle, owner.PrimaryScaleWitdh, owner.PrimaryScaleBackground);
        }

        


        /// <summary>
        /// 获取仪表盘的中心点坐标
        /// </summary>
        Point MeterCenter
        {
            get
            {
                return new Point(
                    canScale.ActualWidth / 2,
                    canScale.ActualHeight / 2);
            }
        }

        /// <summary>
        /// 极坐标转换为直角坐标
        /// </summary>
        /// <param name="Polar"></param>
        /// <returns></returns>
        Point ConvertPolarToDescartes(Point Polar)
        {
            return new Point(
                Polar.X * Math.Cos(Polar.Y / 180 * Math.PI),
                Polar.X * Math.Sin(Polar.Y / 180 * Math.PI));
        }

        /// <summary>
        /// 由于绘制仪表盘时需要以仪表盘中心坐标为坐标原点，
        /// 因此使用此方法将绘制点修正到中心坐标为原点的坐标系
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        Point ShiftedCoordinate(Point p)
        {
            return new Point(
                MeterCenter.X + p.X,
                MeterCenter.Y + p.Y);
        }

        /// <summary>
        /// 绘制刻度线
        /// </summary>
        /// <param name="Primary_Radius_Inner">主刻度线内圆半径</param>
        /// <param name="Primary_Radius_Outer">主刻度线外圆半径</param>
        /// <param name="Start_Angle">主刻度线起始角度</param>
        /// <param name="End_Angle">主刻度线终止角度</param>
        /// <param name="Primary_Count">主刻度线数量</param>
        /// <param name="Width">刻度线宽度</param>
        /// <param name="Background">刻度线颜色</param>
        void DrawScale(double Primary_Radius_Inner, double Primary_Radius_Outer, int Primary_Count, double Minor_Radius_Inner, double Minor_Radius_Outer, int Minor_Count, double Start_Angle, double End_Angle, double Width, Brush Background)
        {
            canScale.Children.Clear();
            canPrimaryScaleLabel.Children.Clear();

            if (Primary_Count > 0)
            {
                /* 计算每个Primary Scale标签所显示的数值 */
                double label_interval = (this.Max - this.Min) / Primary_Count;

                /* 计算每个Primary Scale之间的角度 */
                double angle_interval_primary_scale = (End_Angle - Start_Angle) / Primary_Count;

                /* 计算每个Minor Scale之间的角度 */
                double angle_interval_minor_scale = angle_interval_primary_scale / Minor_Count;

                /* 开始绘制 */
                for (int i = 0; i <= Primary_Count; i++)
                {
                    /* 当前要绘制的主刻度的角度值 */
                    double current_angle = Start_Angle + angle_interval_primary_scale * i;

                    /* 绘制主刻度 */
                    Point inner_primary = ShiftedCoordinate(ConvertPolarToDescartes(new Point(Primary_Radius_Inner, current_angle)));
                    Point outer_primary = ShiftedCoordinate(ConvertPolarToDescartes(new Point(Primary_Radius_Outer, current_angle)));
                    Line line_primary = new Line();
                    line_primary.Stroke = Background;
                    line_primary.StrokeThickness = Width;
                    line_primary.X1 = inner_primary.X;
                    line_primary.Y1 = inner_primary.Y;
                    line_primary.X2 = outer_primary.X;
                    line_primary.Y2 = outer_primary.Y;
                    line_primary.SnapsToDevicePixels = true;
                    canScale.Children.Add(line_primary);

                    /* 如果primary scale标签可见，则绘制标签 */
                    if (PrimaryScaleLabelVisibility == Visibility.Visible)
                    {
                        Point poslabel = ShiftedCoordinate(ConvertPolarToDescartes(new Point(PrimaryScaleLabelRadius, Start_Angle + angle_interval_primary_scale * i)));
                        TextBlock label = new TextBlock();
                        label.Text = (this.Min + label_interval * i).ToString("F0");
                        label.SnapsToDevicePixels = true;
                        label.Foreground = this.PrimaryScaleBackground;
                        label.FontFamily = this.FontFamily;
                        label.FontSize = this.FontSize;
                        label.FontStyle = this.FontStyle;
                        label.FontWeight = this.FontWeight;

                        /* 旋转文本 */
                        RotateTransform tr = new RotateTransform(Start_Angle + angle_interval_primary_scale * i + 90);
                        TransformGroup trg = new TransformGroup();
                        trg.Children.Add(tr);
                        label.RenderTransform = trg;
                        label.RenderTransformOrigin = new Point(0.5, 0.5);

                        /* 分析文本宽度和高度，以确定标签应该显示在哪个坐标 */
                        label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                        label.Arrange(new Rect(label.DesiredSize));
                        Canvas.SetLeft(label, poslabel.X - label.ActualWidth / 2);
                        Canvas.SetTop(label, poslabel.Y - label.ActualHeight / 2);
                        canPrimaryScaleLabel.Children.Add(label);
                    }

                    /* 绘制次刻度
                     * 由于Minor_Count表示的时次刻度将主刻度分割的数量，
                     * 所以当分割数量=1时，不对主刻度做任何分割，所以不予处理
                     * i < PrimaryScaleCount表示绘制完最后一个主刻度以后不再绘制次刻度
                     */
                    if (Minor_Count > 1 && i < PrimaryScaleCount)
                    {
                        for (int n = 1; n < Minor_Count; n++)
                        {
                            Point inner_minor = ShiftedCoordinate(ConvertPolarToDescartes(new Point(Minor_Radius_Inner, current_angle + angle_interval_minor_scale * n)));
                            Point outer_minor = ShiftedCoordinate(ConvertPolarToDescartes(new Point(Minor_Radius_Outer, current_angle + angle_interval_minor_scale * n)));
                            Line line_minor = new Line();
                            line_minor.Stroke = Background;
                            line_minor.StrokeThickness = Width;
                            line_minor.X1 = inner_minor.X;
                            line_minor.Y1 = inner_minor.Y;
                            line_minor.X2 = outer_minor.X;
                            line_minor.Y2 = outer_minor.Y;
                            line_minor.SnapsToDevicePixels = true;
                            canScale.Children.Add(line_minor);
                        }
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// IndicatorBar颜色策略
    /// </summary>
    public class Color_Value : DependencyObject
    {
        public Color_Value()
        { }

        public Color_Value(SolidColorBrush Background, double Value)
        {
            this.Background = Background;
            this.Value = Value;
        }



        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(Color_Value), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        public double  Value
        {
            get { return (double )GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double ), typeof(Color_Value ), new PropertyMetadata(0.0));
    }
}