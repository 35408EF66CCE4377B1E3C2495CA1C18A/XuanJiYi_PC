using DevExpress.Xpf.Core;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using Tai_Shi_Xuan_Ji_Yi.ViewModel;

namespace Tai_Shi_Xuan_Ji_Yi.Windows
{
    /// <summary>
    /// Interaction logic for HistoryShow.xaml
    /// </summary>
    public partial class HistoryShow : DXWindow
    {
        public HistoryShow()
        {
            InitializeComponent();
            ViewModelLocator locator = grid_root.DataContext as ViewModelLocator;
            locator.CureHistory.LoadHistorySummary();

            Loaded += HistoryShow_Loaded;
            Unloaded += HistoryShow_Unloaded;
        }

        private void HistoryShow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage<string>>(this, "DBError", msg =>
            {
                // 因为从数据库加载cure_history表和detail表在单独的线程中进行，以防止UI锁死
                // 因此需要用BeginInvoke弹出MessageBox
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(msg.Content, msg.Notification, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }));
            });
        }

        private void HistoryShow_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage<string>>(this, "DBError");
        }

        
    }
}
