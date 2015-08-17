using DevExpress.Xpf.Core;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace Tai_Shi_Xuan_Ji_Yi
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : DXWindow
    {
        public Setting()
        {
            InitializeComponent();

            this.Loaded += Setting_Loaded;
            this.Unloaded += Setting_Unloaded;
        }
        
        private void Setting_Loaded(object sender, RoutedEventArgs e)
        {
            // 如果更改密码是发生错误，弹出错误提示窗口
            Messenger.Default.Register<GenericMessage<string>>(this, "ChangePasswordFailed", msg => {
                var message = msg.Content;
                MessageBox.Show(message.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                ClearAndSetPasswordBox();
            });

            // 密码更改成功后弹出提示窗口
            Messenger.Default.Register<GenericMessage<string>>(this, "ChangePasswordSucceed", msg => {
                var message = msg.Content;
                MessageBox.Show(message.ToString(), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void Setting_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<GenericMessage<string>>(this, "ChangePasswordFailed");
            Messenger.Default.Unregister<GenericMessage<string>>(this, "ChangePasswordSucceed");
        }

        /// <summary>
        /// 清楚密码框的内容并将焦点设置到第一个密码框
        /// </summary>
        void ClearAndSetPasswordBox()
        {
            passwordBox.Password = "";
            passwordBox1.Password = "";
            passwordBox.Focus();
        }
    }
}
