using System;
using System.Windows;
using Tai_Shi_Xuan_Ji_Yi.ChartControl;

namespace Tai_Shi_Xuan_Ji_Yi
{
    /// <summary>
    /// ChartControlTest.xaml 的交互逻辑
    /// </summary>
    public partial class ChartControlTest : Window
    {
        SeriesPointCollection points;

        public ChartControlTest()
        {
            InitializeComponent();

            points = new SeriesPointCollection();

            AreaSeries2D series = new AreaSeries2D();
            series.Points = points;
            chart.Series.Add(series);

            points.Add(new SeriesPoint() { Argument = 0, Value = 10 });
            points.Add(new SeriesPoint() { Argument = 1, Value = 30 });
            points.Add(new SeriesPoint() { Argument = 2, Value = 23 });
            points.Add(new SeriesPoint() { Argument = 3, Value = 15 });
        }

        private void btn_AddPoint_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 1000; i++)
            {
                int n = r.Next(100);
                points.Add(new SeriesPoint() { Argument = 0, Value = (double)n });
            }
            txt_PointsCount.Text = points.Count.ToString();
        }
    }
}
