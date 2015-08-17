using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChartControl
{
    /// <summary>
    /// Area2DControl.xaml 的交互逻辑
    /// </summary>
    public partial class Area2DSeries : UserControl
    {
        public Area2DSeries()
        {
            InitializeComponent();

            SetValue(PointsProperty, new SeriesPointCollection());
        }

        #region DP Points


        public SeriesPointCollection Points
        {
            get { return (SeriesPointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Points.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(SeriesPointCollection), typeof(Area2DSeries), 
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSeriesPointCollectionChanged)));

        private static void OnSeriesPointCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Area2DSeries owner = d as Area2DSeries;
            owner.DrawSeries();
        }

        
        #endregion

        #region Private Properties

        #endregion

        void DrawSeries()
        {
            /* 清除以前的曲线 */
            canSeries.Children.Clear();

            /* 如果没有序列点，则退出 */
            if(this.Points.Count == 0)
                return;

            /* 获取Canvas的高度 */
            double can_height = canSeries.ActualHeight;
            if(can_height <= 0)
                return;

            /* 计算两个数据点之间的间隔 */
            double point_interval = this.ActualWidth / this.Points.Count;
            if(point_interval <= 0)
                return;

            /* 获取所有点里Y值最大的点 */
            double max_value = this.Points.MaxPointValue();


            /* 绘制数据点 */
            // Create a path to draw a geometry with.  
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;

            // Create a StreamGeometry to use to specify myPath.  
            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;

            // Open a StreamGeometryContext that can be used to describe this StreamGeometry   
            // object's contents.  
            using (StreamGeometryContext ctx = geometry.Open())
            {

                for (int i = 0; i < this.Points.Count; i++)
                {
                    /* 计算绘制该点的 Y 坐标 */
                    double y = CalPointY(this.Points[i].Value, max_value, can_height);

                    /* 计算绘制该点的 X 坐标 */
                    double x = point_interval * i;

                    if (i == 0)     // 如果是第一个点
                    {
                        // Begin the triangle at the point specified. Notice that the shape is set to   
                        // be closed so only two lines need to be specified below to make the triangle.  
                        ctx.BeginFigure(new Point(x, y), true /* is filled */, true /* is closed */);
                    }
                    else if (i == this.Points.Count - 1) // 如果是最后一个点
                    {
                        // Draw a line to the next specified point.  
                        ctx.LineTo(new Point(x, y), true /* is stroked */, false /* is smooth join */);
                    }
                    else
                    {
                        // Draw another line to the next specified point.  
                        ctx.LineTo(new Point(x, y), true /* is stroked */, false /* is smooth join */);
                    }
                }
            }

            // Freeze the geometry (make it unmodifiable)  
            // for additional performance benefits.  
            geometry.Freeze();

            // Specify the shape (triangle) of the Path using the StreamGeometry.  
            myPath.Data = geometry;

            canSeries.Children.Add(myPath);
        }

        /// <summary>
        /// 根据Y值计算该点在Canvas上绘制的Y坐标
        /// </summary>
        /// <param name="PointValue"></param>
        /// <param name="MaxValue"></param>
        /// <param name="CanvasHeight"></param>
        /// <returns></returns>
        double CalPointY(double PointValue, double MaxValue, double CanvasHeight)
        {
            return CanvasHeight -  CanvasHeight * (PointValue / MaxValue);
        }
    }
}
