using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Tai_Shi_Xuan_Ji_Yi.ChartControl
{
    class AreaSeries2D : Series
    {
        #region Variables
        public override event System.EventHandler CurveUpdated;

        Size szPanelView;
        SeriesPointCollection _points;
        
        double dblPointsInterval = 0;
        double dblMaxPointValue;
        Point ptNext = new Point(0, 0);
        

        StreamGeometry gStream_Line, gStream_Area;
        GeometryDrawing gDrawing_Line, gDrawing_Area;
        DrawingGroup drawingGroup;
        
        #endregion

        #region Constructors
        public AreaSeries2D()
        {
            // 初始化保存数据点的变量
            _points = new SeriesPointCollection();
            _points.CollectionChanged += _points_CollectionChanged;

            // 初始化绘制曲线的画板
            gStream_Line = new StreamGeometry();
            gStream_Area = new StreamGeometry();

            gDrawing_Line = new GeometryDrawing();
            gDrawing_Line.Brush = null;
            gDrawing_Line.Pen = new Pen()
            {
                Thickness = 1,
                Brush = Brushes.Black
            };
            gDrawing_Line.Geometry = gStream_Line;

            /* 初始化绘制填充色的画板 */
            gDrawing_Area = new GeometryDrawing();
            gDrawing_Area.Brush = Brushes.Red;
            gDrawing_Area.Pen = new Pen()
            {
                Thickness = 0,
                Brush = null
            };
            gDrawing_Area.Geometry = gStream_Area;

            /* 生成绘画组 */
            drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(gDrawing_Line);
            drawingGroup.Children.Add(gDrawing_Area);
        }
        #endregion

        #region Properties
        /// <summary>
        /// 设置或返回数据列表
        /// </summary>
        public override SeriesPointCollection Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                _points.CollectionChanged += _points_CollectionChanged;
            }
        }

        /// <summary>
        /// 设置曲线显示区域尺寸
        /// </summary>
        public override Size PanelViewSize
        {
            set 
            {                
                szPanelView = value;
                RedrawWholeCurve();
            }
        }

        /// <summary>
        /// 返回承载曲线的 StreamGenometry 的尺寸
        /// </summary>
        public override Rect Bounds
        {
            get { return gStream_Line.Bounds; }
        }

        /// <summary>
        /// 曲线视图内显示的数据点数发生变化
        /// </summary>
        public override double PointsInterval
        {
            set
            {
                /* 如果曲线显示的数据点数没有发生变化，则不要刷新曲线 */
                double tmp = dblPointsInterval;
                if (tmp != value)
                {
                    dblPointsInterval = value;
                    RedrawWholeCurve();
                }

            }
        }

        /* 返回承载曲线图形的画板 */
        public override Drawing Draw
        {
            get
            {
                return drawingGroup;
            }
        }
        #endregion

        #region Methods
        void RaisePropertyChangedEvent(string PropertyName)
        {
            //if (PropertyChanged != null)
            //    PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        void RaiseCurveUpdatedEvent()
        {
            if (CurveUpdated != null)
                CurveUpdated(this, new EventArgs());
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
            return CanvasHeight - CanvasHeight * (PointValue / MaxValue);
        }

        /// <summary>
        /// 重绘整个曲线
        /// </summary>
        void RedrawWholeCurve()
        {
            gStream_Line.Clear();

            /* 如果显示曲线的Panel尺寸为 0， 则不绘制曲线 */
            if (szPanelView.Height == 0 || szPanelView.Width == 0)
                return;

            /* 如果两点间的间距为 0， 则不绘制曲线 */
            if (dblPointsInterval == 0)
                return;

            /* 获取数据序列中的最大 Y 值 *
             * 该值用来计算即将绘制的点在View上的 纵坐标 */
            dblMaxPointValue = _points.MaxByY.Value;

            using (StreamGeometryContext ctx = gStream_Line.Open())
            {
                using (StreamGeometryContext ctx_area = gStream_Area.Open())
                {
                    ctx_area.BeginFigure(new Point(0, szPanelView.Height), true, true);

                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (i == 0) // 如果是第一个点，则调用BegingFigure方法
                        {
                            ptNext.X = 0;
                            ptNext.Y = CalPointY(_points[i].Value, dblMaxPointValue, szPanelView.Height);
                            ctx.BeginFigure(ptNext, true, false);
                        }
                        else // 从第二个点开始，调用LintTo方法
                        {
                            ptNext.X += dblPointsInterval;
                            ptNext.Y = CalPointY(_points[i].Value, dblMaxPointValue, szPanelView.Height);
                            ctx.LineTo(ptNext, true, true);
                        }

                        ctx_area.LineTo(ptNext, true, true);
                    }

                    ctx_area.LineTo(new Point(ptNext.X, szPanelView.Height), true, true);
                }
            }

            /*  通知父类曲线已经重绘 */
            RaiseCurveUpdatedEvent();
        }
        #endregion
        
        #region Events
        /// <summary>
        /// 当曲线数据点发生变化时触发此事件
        /// 该事件用来重绘曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //SeriesPoint p = e.NewItems[0] as SeriesPoint;
            //if (p.Value > dblMaxPointValue)
            //    dblMaxPointValue = p.Value;

            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    using (StreamGeometryContext ctx = gStream.Open())
            //    {
            //        if (gStream.MayHaveCurves()) // 如果已经有曲线
            //        {
            //            ptNext.X += dblPointsInterval;
            //            ptNext.Y = CalPointY(p.Value, dblMaxPointValue, szPanelView.Height);
            //            ctx.LineTo(ptNext, true, true);
            //        }
            //        else // 如果还没有曲线
            //        {
            //            ptNext.X = 0;
            //            ptNext.Y = CalPointY(p.Value, dblMaxPointValue, szPanelView.Height);
            //            ctx.BeginFigure(ptNext, true, false);
            //        }
            //    }

            //    /* 曲线更新的通知 */
            //    RaiseCurveUpdatedEvent();
            //}
            //else
            //{
            //    /* 如果从数据点列表中删除了全部或某个数据点，则需要重绘曲线 */
            //    RedrawWholeCurve();  
            //}

            RedrawWholeCurve();
        }
        #endregion
    }
}
