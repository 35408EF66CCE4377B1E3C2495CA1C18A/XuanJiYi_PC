using System.ComponentModel;
using System.Windows;
using System;
using System.Windows.Media;

namespace Tai_Shi_Xuan_Ji_Yi.ChartControl
{
    public abstract class Series
    {
        public abstract event EventHandler CurveUpdated;
        
        /// <summary>
        /// 数据点
        /// </summary>
        public abstract SeriesPointCollection Points
        {
            set;
            get;
        }

        /// <summary>
        /// 图标显示区域的尺寸
        /// </summary>
        public abstract Size PanelViewSize { set; }

        /// <summary>
        /// 曲线绘制窗口的尺寸（StreamGenometry尺寸）
        /// </summary>
        public abstract Rect Bounds
        {
            get;
        }

        /// <summary>
        /// 曲线视图中显示的数据点数
        /// </summary>
        public abstract double PointsInterval { set; }

        public abstract Drawing Draw { get; }
    }
}
