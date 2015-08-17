using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tai_Shi_Xuan_Ji_Yi.ChartControl
{
    /// <summary>
    /// PanelLayout.xaml 的交互逻辑
    /// </summary>
    public partial class PanelLayout : UserControl
    {
        ObservableCollection<Series> _series;

        public PanelLayout()
        {
            InitializeComponent();

            _series = new ObservableCollection<Series>();
            _series.CollectionChanged += _series_CollectionChanged;

            this.SizeChanged += PanelLayout_SizeChanged;
        }

        void PanelLayout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Series s in _series)
            {
                s.PanelViewSize = this.RenderSize;
                s.PointsInterval = 20;
            }
        }

        #region DP Series

        public ObservableCollection<Series> Series
        {
            set
            {
                _series = value;
            }
            get
            {
                return _series;
            }
        }

        void _series_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            AreaSeries2D s = e.NewItems[0] as AreaSeries2D;
            s.CurveUpdated += s_CurveUpdated;
        }

        void s_CurveUpdated(object sender, System.EventArgs e)
        {
            //AreaSeries2D s = sender as AreaSeries2D;

            //DrawingBrush drawingBrush = new DrawingBrush();
            //drawingBrush.Drawing = s.Draw;
            //Rect rect = s.Bounds;
            //if(rect.Size.Width > canCurve.ActualWidth)
            //{
            //    drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;
            //    drawingBrush.Viewbox =
            //        new Rect(
            //            rect.Size.Width - canCurve.ActualWidth,
            //            0,
            //            canCurve.ActualWidth,
            //            canCurve.ActualHeight);
            //}

            //drawingBrush.Stretch = Stretch.None;
            //drawingBrush.TileMode = TileMode.None;

            //drawingBrush.AlignmentX = AlignmentX.Right;
            
            //canCurve.Background = drawingBrush;
        }

        
        #endregion
    }
}
