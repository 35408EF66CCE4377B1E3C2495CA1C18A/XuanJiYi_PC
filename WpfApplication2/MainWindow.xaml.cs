using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using Tai_Shi_Xuan_Ji_Yi.Windows;

namespace Tai_Shi_Xuan_Ji_Yi
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        CControlCentral control_central;
        #endregion

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                /* 加载配置文件 */
                CPublicVariables.Configuration = new CConfiguration();

                /* 初始化控制板实例，该实例包含了串口通讯的协议 */
                control_central = new CControlCentral();
                control_central.ControllerCommStatusChanged += Control_central_ControllerCommStatusChanged;

                /* 初始化左侧导航栏里的按钮 */
                CPublicVariables.CureBandList = new ObservableCollection<CCureBandClass>();
                for (int i = 0; i < 8; i++)
                {
                    CPublicVariables.CureBandList.Add(new CCureBandClass(i, ref control_central));

                }
                leftNavBar.ItemCollection = CPublicVariables.CureBandList;

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 通讯状态改变引发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_central_ControllerCommStatusChanged(object sender, CommProgressReportArg e)
        {
            switch(e.Progress)
            {
                case CommProgressReportArg.ENUM_COMM_EVENT_TYPE.PortOpening:
                    txt_CommErrInfo.Text = e.Info;
                    break;

                case CommProgressReportArg.ENUM_COMM_EVENT_TYPE.Err:
                    if (brd_CommErr.Visibility == Visibility.Hidden)
                        brd_CommErr.Visibility = Visibility.Visible;
                    txt_CommErrInfo.Text = e.Info;
                    break;

                case CommProgressReportArg.ENUM_COMM_EVENT_TYPE.Finish:
                    if (brd_CommErr.Visibility == Visibility.Visible)
                        brd_CommErr.Visibility = Visibility.Hidden;

                    break;
            }
        }

        private void btn_TemperSequenceEdit_Click(object sender, RoutedEventArgs e)
        {
            TemperatureSequenceEditor f = new TemperatureSequenceEditor(TemperatureSequenceEditor.ENUM_Window_Mode.Edit);
            f.ShowDialog();
        }

        /// <summary>
        /// 密码输入栏中按下回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_UnlockPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btn_Unlock_Click(this, new RoutedEventArgs());
        }


        /// <summary>
        /// 解锁屏幕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Unlock_Click(object sender, RoutedEventArgs e)
        {
            if(txt_UnlockPassword.Password == CPublicVariables.Configuration.LoginPassword)
            {
                brd_LoginPanel.Visibility = System.Windows.Visibility.Hidden;
                txt_UnlockPassword.Password = "";

            }
            else
            {
                txt_UnlockPassword.Password = "";
                txt_UnlockPassword.Focus();
            }
        }

        /// <summary>
        /// 锁定屏幕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_Locking(object sender, RoutedEventArgs e)
        {
            /* 显示解锁界面 */
            brd_LoginPanel.Visibility = System.Windows.Visibility.Visible;
            txt_UnlockPassword.Focus();
        }

        private void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            Setting f_setting = new Setting();
            f_setting.ShowDialog();
        }

        private void btn_History_Click(object sender, RoutedEventArgs e)
        {
            HistoryShow f_history = new HistoryShow();
            f_history.ShowDialog();
        }
    }
}
