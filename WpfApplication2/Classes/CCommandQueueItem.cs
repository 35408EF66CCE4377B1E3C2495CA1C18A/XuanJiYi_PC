using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class CCommandQueueItem
    {
        public enum ENUM_CMD
        {
            Start,
            Stop,
            SetTemper
        }

        public CCommandQueueItem(ENUM_CMD Command, int Channel, double TargetTemperature)
        {
            this.Command = Command;
            this.Channel = Channel;
            this.Temperature = TargetTemperature;
        }

        public ENUM_CMD Command
        {
            private set;
            get;
        }

        public int Channel
        {
            private set;
            get;
        }

        public double Temperature
        {
            private set;
            get;
        }
    }
}
