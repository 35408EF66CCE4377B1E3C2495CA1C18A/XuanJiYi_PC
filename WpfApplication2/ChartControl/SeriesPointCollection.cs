using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tai_Shi_Xuan_Ji_Yi.ChartControl
{
    public class SeriesPointCollection : ObservableCollection<SeriesPoint>
    {
        #region Variables
        SeriesPoint ptMaxByX = null, ptMaxByY = null;
        #endregion

        #region Constructors
        //public SeriesPointCollection()
        //{
        //    points = new List<SeriesPoint>();
        //}
        #endregion

        #region 
        public SeriesPoint MaxByX
        {
            get
            {
                return ptMaxByX;
            }
        }

        public SeriesPoint MaxByY
        {
            get
            {
                return ptMaxByY;
            }
        }
        #endregion

        #region Methods
        public double MaxPointValue()
        {
            return this.Max<SeriesPoint>(t => t.Value);
        }


        private static int SortCompare(SeriesPoint Arg1, SeriesPoint Arg2)
        {
            if (Arg1.Value < Arg2.Value)
                return -1;
            else if (Arg1.Value > Arg2.Value)
                return 1;
            else
                return 0;
        }
        #endregion

        #region Events
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SeriesPoint pt = e.NewItems[0] as SeriesPoint;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if(ptMaxByX == null)
                {
                    ptMaxByX = pt;
                }
                else
                {
                    if(ptMaxByX.DateTimeValue != null)
                    {
                        if (ptMaxByX.DateTimeValue < pt.DateTimeValue)
                            ptMaxByX = pt;
                    }
                    else
                    {
                        if (ptMaxByX.Argument < pt.Argument)
                            ptMaxByX = pt;
                    }
                }

                if (ptMaxByY == null)
                    ptMaxByY = pt;
                else
                {
                    if (ptMaxByY.Value < pt.Value)
                        ptMaxByY = pt;
                }
            }
            else
            {
                ptMaxByX = this.OrderByDescending(t => t.Argument).First();
                ptMaxByY = this.OrderByDescending(t => t.Value).First();
            }

            base.OnCollectionChanged(e);
        }
        #endregion
    }
}
