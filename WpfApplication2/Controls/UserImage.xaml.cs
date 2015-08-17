using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tai_Shi_Xuan_Ji_Yi
{
    /// <summary>
    /// UserImage.xaml 的交互逻辑
    /// </summary>
    public partial class UserImage : UserControl
    {
        #region Variables
        Storyboard sb_PopDetail;
        #endregion

        public UserImage()
        {
            this.InitializeComponent();

            sb_PopDetail = this.Resources["sb_PopDetail"] as Storyboard;
        }

        private void MouseDownOutSide(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            sb_PopDetail.Stop();
        }


        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            sb_PopDetail.Begin();
            Mouse.Capture(this);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, new MouseButtonEventHandler(MouseDownOutSide));
        }
    }
}