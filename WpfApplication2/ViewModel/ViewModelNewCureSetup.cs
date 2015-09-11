using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence;

namespace Tai_Shi_Xuan_Ji_Yi.ViewModel
{
    public class ViewModelNewCureSetup : ViewModelBase
    {
        string strPatientName;
        /// <summary>
        /// Set or get the patient name
        /// </summary>
        public string PatientName
        {
            set
            {
                strPatientName = value;
                RaisePropertyChanged("PatientName");
            }
            get
            {
                return strPatientName;
            }
        }
        public string CureSN { get; private set; }

        public string[] SequenceNames { get; private set; }

        string strSelectedSeqName;
        public string SelectedSeqName
        {
            get
            {
                return strSelectedSeqName;
            }
            private set
            {
                strSelectedSeqName = value;
                RaisePropertyChanged("SelectedSeqName");
            }
        }

        CTemperatureSequence seq;
        public CTemperatureSequence Sequence
        {
            private set
            {
                seq = value;
                RaisePropertyChanged("Sequence");
            }
            get
            {
                return seq;
            }

        }

        public bool Result { get; set; }

        public ViewModelNewCureSetup()
        {
            //LoadParameters();
        }
        
        /// <summary>
        /// 启动一个加载SequenceNames和生成CureSN的线程
        /// </summary>
        /// <param name="param"></param>
        public void LoadParameters()
        {
            string[] seq_names = null;
            string cure_sn = string.Empty;

            new Thread(() =>
            {
                using (CDatabase db = new CDatabase())
                {
                    // 从数据库获取温度预设曲线名称列表
                    if (!db.GetSequenceNames(out seq_names))
                    {
                        // 如果错误，通知View弹出错误对话框
                        Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(db.LastError), "DBError");
                        return;
                    }
                    else
                    {
                        this.SequenceNames = seq_names;
                        RaisePropertyChanged("SequenceNames");
                    }

                    if (!db.GetNewCureSN(ref cure_sn))
                    {
                        // 如果错误，通知View弹出错误对话框
                        Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(db.LastError), "DBError");
                    }
                    else
                    {
                        this.CureSN = cure_sn;
                        RaisePropertyChanged("CureSN");
                        return;
                    }
                }
            }).Start();

            this.SelectedSeqName = null;
            this.Sequence = null;
            this.PatientName = "";
            this.Result = false;
        }


        /// <summary>
        /// Load sequence with name
        /// </summary>
        /// <param name="SeqName"></param>
        /// <returns></returns>
        public void LoadSequence(string SequenceName)
        {
            using (CDatabase db = new CDatabase())
            {
                CTemperatureSequence _Sequence;
                if (db.GetTemperatureSequence(SequenceName, out _Sequence))
                {
                    this.Sequence = _Sequence;
                    if (_Sequence == null)
                        this.SelectedSeqName = "";
                    else
                        this.SelectedSeqName = _Sequence.SequenceName;
                }
                else
                {

                }
            }
        }

        public RelayCommand<string> SequenceSelectionChanged
        {
            get
            {
                return new RelayCommand<string>(new Action<string>(LoadSequence));
            }
        }

        public RelayCommand ApplySetting
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (PatientName == "")
                    {
                        Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>("患者姓名不能为空", "ApplyError"));
                    }
                    else if (Sequence == null)
                    {
                        Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>("请选择预设温度曲线", "ApplyError"));
                    }
                    else
                    {
                        Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>("成功", "ApplySucceed"));
                        Result = true;
                    }
                });
            }
        }
    }
}
