using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tai_Shi_Xuan_Ji_Yi.Controls
{
    /// <summary>
    /// FlatProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class FlatProgressBar : UserControl
    {
        public FlatProgressBar()
        {
            InitializeComponent();
        }

        #region Methods
        protected void DrawBar()
        {
            double width = 0;
            try
            {
                width = brd.ActualWidth;
                bar.Width = width * (this.Value / (this.Max - this.Min));
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            try
            {
                if (sizeInfo != null && sizeInfo.WidthChanged)
                {
                    DrawBar();
                }
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            base.OnRenderSizeChanged(sizeInfo);
        }

        #endregion

        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(FlatProgressBar), new PropertyMetadata(0.0, OnMinChanged));

        private static void OnMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlatProgressBar owner = d as FlatProgressBar;
            if ((double)e.NewValue > owner.Max)
                owner.Min = (double)e.OldValue;
            else
                owner.DrawBar();
        }




        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(FlatProgressBar), new PropertyMetadata(100.0, OnMaxChanged));

        private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlatProgressBar owner = d as FlatProgressBar;
            if ((double)e.NewValue < owner.Min)
                owner.Max = (double)e.OldValue;
            else
                owner.DrawBar();
        }

        
        


        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(FlatProgressBar), new PropertyMetadata(0.0, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlatProgressBar owner = d as FlatProgressBar;
            owner.DrawBar();
        }

        
    }
}
