using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartControl
{
    public class SeriesPointCollection : List<SeriesPoint>
    {
        #region Variables
        //List<SeriesPoint> points;
        #endregion

        #region Constructors
        //public SeriesPointCollection()
        //{
        //    points = new List<SeriesPoint>();
        //}
        #endregion

        #region Methods
        //public void Add(SeriesPoint Point)
        //{
        //    points.Add(Point);
        //}

        //public void AddRange(SeriesPoint[] Points)
        //{
        //    points.AddRange(points);
        //}

        //public void Clear()
        //{
        //    points.Clear();
        //}

        //public void RemoveAt(int Index)
        //{
        //    points.RemoveAt(Index);
        //}

        //public void Remove(SeriesPoint Item)
        //{
        //    points.Remove(Item);
        //}

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
    }
}
