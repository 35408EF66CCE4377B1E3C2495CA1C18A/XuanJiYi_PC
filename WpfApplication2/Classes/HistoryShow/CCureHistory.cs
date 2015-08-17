using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence;

namespace Tai_Shi_Xuan_Ji_Yi.Classes.HistoryShow
{
    public class CCureHistory
    {
        string _patient_name = "";
        string _cure_sn = "";
        int _cure_channel = -1;
        DateTime _created_time;
        DateTime _updated_time;
        CTemperatureSequence _seq_snapshot;

        public CCureHistory(string CureSN, string PatientName, int CureChannel, DateTime CreatedTime, DateTime UpdatedTime, byte[] SequenceSnapshot)
        {
            _cure_sn = CureSN;
            _patient_name = PatientName;
            _cure_channel = CureChannel;
            _created_time = CreatedTime;
            _updated_time = UpdatedTime;
            _seq_snapshot = (CTemperatureSequence)CPublicMethods.DeserializeObjectFromBinaryArray(SequenceSnapshot);
        }

        #region Properties
        public string CureSN
        {
            get
            {
                return _cure_sn;
            }
        }

        public string PatientName
        {
            get

            {
                return _patient_name;
            }
        }

        public int CureChannel
        {
            get
            {
                return _cure_channel;
            }
        }

        public DateTime CreatedTime
        {
            get
            {
                return _created_time;
            }
        }

        public DateTime UpdatedTime
        {
            get
            {
                return _updated_time;
            }
        }

        public CTemperatureSequence SequenceSnapshot
        {
            get
            {
                return _seq_snapshot;
            }
        }
        #endregion
    }
}
