using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tai_Shi_Xuan_Ji_Yi.Controls
{
    /// <summary>
    /// RunPauseStopIcon.xaml 的交互逻辑
    /// </summary>
    public partial class RunPauseStopIcon : UserControl
	{
        public enum ENUM_State
        {
            Run,
            Pause,
            Stop
        }

		public RunPauseStopIcon()
		{
			this.InitializeComponent();
		}



        /// <summary>
        /// 设置或返回状态
        /// </summary>
        public ENUM_State State
        {
            get { return (ENUM_State)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(ENUM_State), typeof(RunPauseStopIcon), new PropertyMetadata(ENUM_State.Stop, OnStateChanged));

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RunPauseStopIcon owner = d as RunPauseStopIcon;
            ENUM_State state = (ENUM_State)e.NewValue;

            switch(state)
            {
                case ENUM_State.Run:
                    owner.icon_run.Visibility = Visibility.Visible;
                    owner.icon_pause.Visibility = Visibility.Hidden;
                    owner.icon_stop.Visibility = Visibility.Hidden;
                    break;

                case ENUM_State.Pause:
                    owner.icon_run.Visibility = Visibility.Hidden;
                    owner.icon_pause.Visibility = Visibility.Visible;
                    owner.icon_stop.Visibility = Visibility.Hidden;
                    break;

                case ENUM_State.Stop:
                    owner.icon_run.Visibility = Visibility.Hidden;
                    owner.icon_pause.Visibility = Visibility.Hidden;
                    owner.icon_stop.Visibility = Visibility.Visible;
                    break;
            }
        }




        public Brush IconBackground
        {
            get { return (Brush)GetValue(IconBackgroundProperty); }
            set { SetValue(IconBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBackgroundProperty =
            DependencyProperty.Register("IconBackground", typeof(Brush), typeof(RunPauseStopIcon), new PropertyMetadata(new SolidColorBrush(Colors.White), OnIconBackgroundChanged));

        private static void OnIconBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RunPauseStopIcon owner = d as RunPauseStopIcon;
            owner.UserControl.Foreground = (Brush)e.NewValue;
        }
    }
}