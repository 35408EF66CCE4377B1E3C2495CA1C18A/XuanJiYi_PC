using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2.ChartControl
{
    /// <summary>
    /// Panel.xaml 的交互逻辑
    /// </summary>
    public partial class Panel : UserControl
    {
        public Panel()
        {
            InitializeComponent();
        }



        public List<Area2DSeries> Series
        {
            get { return (List<Area2DSeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(List<Area2DSeries>), typeof(Panel),
            new FrameworkPropertyMetadata(new List<Area2DSeries>(), new PropertyChangedCallback(OnSeriesChanged)));

        private static void OnSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panel owner = d as Panel;

            owner.grid_Series.Children.Clear();
            foreach (Area2DSeries item in owner.Series)
                owner.grid_Series.Children.Add(item);
        }


        #region DP Border Color

        public Brush BorderColor
        {
            get { return (Brush)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Brush), typeof(Panel), 
            new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnBorderChanged)));

        private static void OnBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panel owner = d as Panel;
            owner.DrawBorder();
        }
        #endregion

        #region DP Grid Color


        public Brush GridColor
        {
            get { return (Brush)GetValue(GridColorProperty); }
            set { SetValue(GridColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColorProperty =
            DependencyProperty.Register("GridColor", typeof(Brush), typeof(Panel),
            new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnGridColorChanged)));

        private static void OnGridColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panel owner = d as Panel;
            owner.DrawGrid();
        }

        #endregion

        #region DP VerticalGridSize


        public double VerticalGridSize
        {
            get { return (double)GetValue(VerticalGridSizeProperty); }
            set { SetValue(VerticalGridSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VeritcalGridSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalGridSizeProperty =
            DependencyProperty.Register("VerticalGridSize", typeof(double), typeof(Panel), 
            new PropertyMetadata(100.0, OnVerticalGridSizeChanged));

        private static void OnVerticalGridSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panel owner = d as Panel;
            owner.DrawGrid();
        }
        #endregion

        #region DP HorizontalGridSize


        public double HorizontalGridSize
        {
            get { return (double)GetValue(HorizontalGridSizeProperty); }
            set { SetValue(HorizontalGridSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalGridSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalGridSizeProperty =
            DependencyProperty.Register("HorizontalGridSize", typeof(double), typeof(Panel), 
            new PropertyMetadata(100.0, OnHorizontalGridSizeChanged));

        private static void OnHorizontalGridSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panel owner = d as Panel;
            owner.DrawGrid();
        }

        
        #endregion


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            canPanel.Children.Clear();
            base.OnRenderSizeChanged(sizeInfo);

            DrawBorder();
            DrawGrid();
        }

        #region Methods
        void DrawBorder()
        {
            Rectangle rect = new Rectangle();
            rect.Width = ActualWidth;
            rect.Height = ActualHeight;
            rect.Stroke = new SolidColorBrush(Colors.Black);

            canPanel.Children.Clear();
            canPanel.Children.Add(rect);
            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);
        }

        void DrawGrid()
        {
            //double panel_width = ActualWidth;
            //if (panel_width <= 0)
            //    return;

            //Path path_vertical_grid = new Path();
            //path_vertical_grid.Stroke = this.GridColor;
            //path_vertical_grid.StrokeThickness = 1;

            //StreamGeometry sg = new StreamGeometry();

            //Point v_start = new Point(0, 0);
            //Point v_end = new Point(0, ActualHeight);
            //int vertical_index = 0;
            //while (true)
            //{
            //    v_start.X = v_end.X = this.VerticalGridSize * vertical_index++;
            //    if (v_start.X >= panel_width)
            //        break;

            //    using (StreamGeometryContext sg_ctx = sg.Open())
            //    {
            //        sg_ctx.BeginFigure(v_start, true, false);
            //        sg_ctx.LineTo(v_end, true, false);
            //    }
            //}
            //sg.Freeze();
            //path_vertical_grid.Data = sg;

            //canPanel.Children.Add(path_vertical_grid);


            RectangleGeometry rectangle = new RectangleGeometry();
            rectangle.Rect = new Rect(0, 0, this.VerticalGridSize, this.HorizontalGridSize);

            GeometryDrawing gDrawing = new GeometryDrawing();
            gDrawing.Geometry = rectangle;
            gDrawing.Brush = null; // 方形填充色
            
            Pen stroke = new Pen();
            stroke.Thickness = 0.5;
            stroke.Brush = this.GridColor;

            gDrawing.Pen = stroke;

            DrawingBrush drawingBrush = new DrawingBrush();
            drawingBrush.Drawing = gDrawing;
            drawingBrush.Viewport = new Rect(0, 0, 0.25, 0.25);            drawingBrush.TileMode = TileMode.Tile;

            canPanel.Background = drawingBrush;
        }
        #endregion

    }
}
