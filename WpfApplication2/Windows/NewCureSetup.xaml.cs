using DevExpress.Xpf.Core;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

namespace Tai_Shi_Xuan_Ji_Yi.Windows
{
    /// <summary>
    /// Interaction logic for PatientNameInput.xaml
    /// </summary>
    public partial class NewCureSetup : DXWindow
    {
        public NewCureSetup()
        {
            InitializeComponent();

            Loaded += NewCureSetup_Loaded;
            Unloaded += NewCureSetup_Unloaded;
            
        }
        
        private void NewCureSetup_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<GenericMessage<string>>(this, "DBError", msg =>
            {
                string m = msg.Content;
                // 因为从数据库加载SequenceNames和获取新CureSN在单独的线程中进行，以防止UI锁死
                // 因此需要用BeginInvoke弹出MessageBox
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(m, "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }));
            });


            Messenger.Default.Register<NotificationMessage<string>>(this, notify => 
            {
                if(notify.Notification == "ApplyError")
                {
                    MessageBox.Show(notify.Content.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (notify.Notification == "ApplySucceed")
                {
                    this.DialogResult = true;
                }
            });
        }

        private void NewCureSetup_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<GenericMessage<string>>(this, "DBError");
            Messenger.Default.Unregister<NotificationMessage<string>>(this);
        }
    }
}
