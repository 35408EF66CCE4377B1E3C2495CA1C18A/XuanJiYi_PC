using DevExpress.Xpf.Core;
using System.Windows;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using Tai_Shi_Xuan_Ji_Yi.Controls;

namespace Tai_Shi_Xuan_Ji_Yi
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            CPublicVariables.Configuration = new CConfiguration();

            base.OnStartup(e);

            DXSplashScreen.Show<SplashScreenView1>();
        }
    }
}
