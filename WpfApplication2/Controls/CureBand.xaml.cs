using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using Tai_Shi_Xuan_Ji_Yi.ViewModel;
using Tai_Shi_Xuan_Ji_Yi.Windows;

namespace Tai_Shi_Xuan_Ji_Yi
{
	/// <summary>
	/// CureBand.xaml 的交互逻辑
	/// </summary>
	public partial class CureBand : UserControl
	{
        public event RoutedEventHandler StartCureClicked;
        public event RoutedEventHandler StopCureClicked;

		public CureBand()
		{
			this.InitializeComponent();

            Loaded += CureBand_Loaded;
		}

        private void CureBand_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<ViewModelNewCureSetup>(this, "NewCureSetup", vm =>
            {
                NewCureSetup f = new NewCureSetup();
                f.ShowDialog();
            });
        }
    }
}