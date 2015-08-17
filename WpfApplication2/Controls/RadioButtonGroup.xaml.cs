using System.Windows.Controls;
using System.Collections.ObjectModel;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using System.Windows;

namespace Tai_Shi_Xuan_Ji_Yi.Controls
{
    /// <summary>
    /// RadioButtonGroup.xaml 的交互逻辑
    /// </summary>
    public partial class RadioButtonGroup : UserControl
    {
        public RadioButtonGroup()
        {
            InitializeComponent();
            stackPanel.Children.Clear();
        }

        public ObservableCollection<CCureBandClass> ItemCollection
        {
            get { return (ObservableCollection<CCureBandClass>)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(ObservableCollection<CCureBandClass>), typeof(RadioButtonGroup), new PropertyMetadata(new ObservableCollection<CCureBandClass>(), OnItemCollectionChanged));

        private static void OnItemCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadioButtonGroup owner = d as RadioButtonGroup;
            ObservableCollection<CCureBandClass> items = e.NewValue as ObservableCollection<CCureBandClass>;
            for (int i = 0; i < items.Count; i++)
            {
                RadioButton button = new RadioButton();
                button.Style = App.Current.Resources["RadioButtonStyle1"] as Style;
                button.DataContext = items[i];
                owner.AddItem(button);
            }

            /* 选中第一个按钮 */
            ((RadioButton)owner.stackPanel.Children[0]).IsChecked = true;
        }


        public CCureBandClass SelectedItem
        {
            get { return (CCureBandClass)GetValue(SelectedItemProperty); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(CCureBandClass), typeof(RadioButtonGroup),
            new PropertyMetadata(null));

        internal void AddItem(RadioButton Item)
        {
            stackPanel.Children.Add(Item);
            Item.Checked += Item_Checked;
        }

        void Item_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            SetValue(SelectedItemProperty, btn.DataContext as CCureBandClass);
        }

    }
}
