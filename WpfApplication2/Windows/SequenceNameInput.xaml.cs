using System.Windows;

namespace Tai_Shi_Xuan_Ji_Yi
{
	/// <summary>
	/// SequenceNameInput.xaml 的交互逻辑
	/// </summary>
	public partial class SequenceNameInput : Window
    {
		public SequenceNameInput()
		{
			this.InitializeComponent();
			
			// 在此点之下插入创建对象所需的代码。
		}

        private void btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Name.Text != "")
                this.DialogResult = true;
            else
                MessageBox.Show("请输入温度序列名称", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string SequenceName
        {
            get
            {
                return txt_Name.Text;
            }
            set
            {
                txt_Name.Text = value;
            }
        }
        
	}
}