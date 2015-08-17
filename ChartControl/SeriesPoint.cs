using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartControl
{
    public class SeriesPoint
    {
        public double Argument
        {
            set;
            get;
        }

        public DateTime DateTimeValue
        {
            set;
            get;
        }

        public object Tag
        {
            set;
            get;
        }

        public double Value
        {
            set;
            get;
        }
    }
}
