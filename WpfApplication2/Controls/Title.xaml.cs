using System.Windows;
using System.Windows.Controls;

namespace Tai_Shi_Xuan_Ji_Yi
{
	/// <summary>
	/// Title.xaml 的交互逻辑
	/// </summary>
	public partial class Title : UserControl
	{
        
        public static readonly RoutedEvent LockingEvent =
            EventManager.RegisterRoutedEvent("Locking", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Title));

        public event RoutedEventHandler Locking
        {
            add { AddHandler(LockingEvent, value); }
            remove { RemoveHandler(LockingEvent, value); }
        }


		public Title()
		{
			this.InitializeComponent();
		}

        private void btn_Lock_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(LockingEvent, this));
        }
	}
}