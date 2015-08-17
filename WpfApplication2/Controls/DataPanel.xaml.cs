using System.Windows;
using System.Windows.Controls;

namespace Tai_Shi_Xuan_Ji_Yi
{
	/// <summary>
	/// DataPanel.xaml 的交互逻辑
	/// </summary>
	public partial class DataPanel : UserControl
	{
		public DataPanel()
		{
			this.InitializeComponent();
		}



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(DataPanel), new PropertyMetadata("title", OnTitleChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPanel owner = d as DataPanel;
            owner.txt_Title.Text = e.NewValue.ToString();
        }




        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(DataPanel), new PropertyMetadata("value", OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPanel owner = d as DataPanel;
            owner.txt_Value.Text = e.NewValue.ToString();
        }


        
	}
}