using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.Realtime;
using Tai_Shi_Xuan_Ji_Yi.ViewModel;
using System.Media;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class CCureBandClass : ViewModelBase
    {
        #region Const Variables

        /// <summary>
        /// 倒计时时基（ms）
        /// </summary>
#if DEBUG
        const int COUNTDOWN_BASETIME_MS = 50;
#else
         const int COUNTDOWN_BASETIME_MS  = 1000;
#endif

        /// <summary>
        /// 用户取消治疗
        /// </summary>
        const int CURE_PROG_REPORT_CANCELED = 90;

        /// <summary>
        /// 治疗过程完成
        /// </summary>
        const int CURE_PROG_REPORT_DONE = 100;

        /// <summary>
        /// 保存数据失败
        /// </summary>
        const int CURE_PROG_REPORT_DB_ERR = 80;

        /// <summary>
        /// 治疗带超过最大使用时间
        /// </summary>
        const int CURE_PROG_REPORT_CUREBAND_EXPIRED = 70;

        /// <summary>
        /// 通知UI线程向RealtimeTemperatureCollection添加一个温度点
        /// </summary>
        const int CURE_PROG_REPORT_ADD_RTT = 1;
        #endregion

        #region Variable
        /// <summary>
        /// 治疗带状态枚举
        /// </summary>
        public enum ENUM_STATE
        {
            /// <summary>
            /// 治疗带未连接
            /// </summary>
            Disconnected,

            /// <summary>
            /// 治疗带已连接，但是未启动治疗
            /// </summary>
            Standby,

            /// <summary>
            /// 治疗带超过有效期
            /// </summary>
            Overdue,

            /// <summary>
            /// 正在治疗
            /// </summary>
            Curing,

            /// <summary>
            /// 
            /// </summary>
            Heating
        }

        /// <summary>
        /// 治疗状态
        /// </summary>
        public enum ENUM_ACTION
        {
            /// <summary>
            /// 已经停止
            /// </summary>
            Stopped,

            /// <summary>
            /// 已经启动
            /// </summary>
            Started,

            /// <summary>
            /// 已经暂停
            /// </summary>
            Paused,

            /// <summary>
            /// 治疗完成
            /// </summary>
            Finished
        }


        /// <summary>
        /// 硬件控制中心
        /// </summary>
        CControlCentral control_central;

        /// <summary>
        /// 当前通道编号
        /// </summary>
        int int_channel;

        /// <summary>
        /// 治疗开始时间
        /// </summary>
        DateTime dt_startcure;

        /// <summary>
        /// 本次治疗总时间
        /// </summary>
        int cure_time_needed;

        /// <summary>
        /// 治疗带已使用时间
        /// </summary>
        int cureband_used_time_overall;

        /// <summary>
        /// 治疗消耗的有效时间（秒）
        /// 即不包括暂停时间
        /// </summary>
        int cure_elapsed_effective;

        /// <summary>
        /// 实时温度
        /// </summary>
        double realtime_temperature;

        /// <summary>
        /// 预设温度
        /// </summary>
        double preset_temperature_from_board;

        /// <summary>
        /// 预设的温度序列
        /// </summary>
        CTemperatureSequence presetted_sequence;

        /// <summary>
        /// 本阶段剩余的秒数
        /// </summary>
        int time_remained_in_stage;

        /// <summary>
        /// 治疗编号
        /// </summary>
        string cure_sn;

        /// <summary>
        /// 患者名称
        /// </summary>
        string patient_name;

        /// <summary>
        /// 治疗处于哪个阶段
        /// 该值指向Sequence集合的某个item
        /// </summary>
        int cure_stage;

        /// <summary>
        /// 温度阶段是否发生变化
        /// </summary>
        bool bln_stage_changed;

        /// <summary>
        /// 阶段内完成的进度
        /// </summary>
        double dbl_progress_in_stage;

        /// <summary>
        /// 治疗带状态
        /// </summary>
        ENUM_STATE band_state;

        /// <summary>
        /// 执行的动作
        /// </summary>
        ENUM_ACTION band_action;

        /// <summary>
        /// 后台治疗线程
        /// </summary>
        BackgroundWorker bgw_cure;

        /// <summary>
        /// 实时温度曲线
        /// </summary>
        CRealtimeTemperatureCollection realtime_temperature_collection;

        /// <summary>
        /// 用来产生倒计时时基的对象
        /// </summary>
        Timer timer_countdown_tick;

        /// <summary>
        /// 倒计时同步信号量
        /// </summary>
        Semaphore sem_countdown_tick;

        #endregion

        #region ICommandVariables
        RelayCommand<object> command_start_resume;
        RelayCommand<object> command_pause_stop;
        RelayCommand<object> command_newcure_setup;
        #endregion

        #region Constructor

        public CCureBandClass(int Channel, ref CControlCentral Controller)
        {
            this.Channel = Channel;
            ClassInit(ref Controller);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 类初始化时执行的公共代码
        /// </summary>
        public void ClassInit(ref CControlCentral Controller)
        {
            control_central = Controller;
            control_central.ControllerCommStatusChanged += Control_central_ControllerCommStatusChanged;

            bgw_cure = new BackgroundWorker();
            bgw_cure.WorkerSupportsCancellation = true;
            bgw_cure.WorkerReportsProgress = true;
            bgw_cure.DoWork += bgw_cure_DoWork;
            bgw_cure.ProgressChanged += bgw_cure_ProgressChanged;
            bgw_cure.RunWorkerCompleted += Bgw_cure_RunWorkerCompleted;
            //bgw_cure.RunWorkerAsync();

            realtime_temperature_collection = new CRealtimeTemperatureCollection();
            presetted_sequence = new CTemperatureSequence();

            /* 初始化倒计时时基的定时器 */
            timer_countdown_tick = new Timer(OnTimerIntervalEvent, null, Timeout.Infinite, COUNTDOWN_BASETIME_MS);
            sem_countdown_tick = new Semaphore(0, 1);

            /* 初始化Command */
            command_start_resume = new RelayCommand<object>(p => StartOrResumeExecute(p));

            this.patient_name = "";
        }

        /// <summary>
        /// 开始治疗
        /// </summary>
        bool Start()
        {
            if (presetted_sequence == null || presetted_sequence.SequenceName == "")  // 如果还没有选择温度序列，警告
            {
                MessageBox.Show("未选择预设温度序列，请点击·新建治疗·按钮进行加载", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                if (control_central.BoardInfo.State.Length > 0)
                {
                    if (control_central.BoardInfo.State[Channel] == ENUM_STATE.Disconnected)
                    {
                        MessageBox.Show("当前通道未插入治疗带", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    // 治疗带使用时间达到最大值
                    else if (this.CureBandState == ENUM_STATE.Overdue)
                    {
                        MessageBox.Show("治疗带使用时间达到最大值，请更换治疗带", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    else if (this.CureAction == ENUM_ACTION.Finished)    // 上次测试完毕，需要重新生成一个CureSN才能开始治疗
                    {
                        MessageBox.Show("该治疗编号已使用，请新建治疗后重新开始", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    else
                    {
                        this.CureAction = ENUM_ACTION.Started;
                        // 启动定时线程，开始治疗
                        if (!bgw_cure.IsBusy)    // bgw_cure线程是否已经运行，如果运行，说明是Resume操作
                        {
                            this.PresetTemperatureInCurrentStage = this.PresettedSequence[0].TargetTemperature; // 初始温度设置为第一个序列温度 
                            bgw_cure.RunWorkerAsync();
                        }
                        else // 从暂停状态恢复
                        {
                            /* 启动1s时基定时器 */
                            timer_countdown_tick.Change(100, COUNTDOWN_BASETIME_MS);
                        }

                        return true;
                    }
                }
                else
                {
                    // 如果还没有收到控制板发回的状态，则不能启动治疗
                    // 如果运行到这里，说明串口通讯出现故障
                    MessageBox.Show("未收到控制板发回的状态报告，请稍候...", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        /// <summary>
        /// 暂停治疗
        /// </summary>
        void Pause()
        {
            timer_countdown_tick.Change(Timeout.Infinite, COUNTDOWN_BASETIME_MS);
        }

        /// <summary>
        /// 复位所有变量
        /// </summary>
        void Reset()
        {
            try
            {
                sem_countdown_tick.Release();
            }
            catch
            {

            }
            bgw_cure.CancelAsync();
        }

        /// <summary>
        /// 清除最后一次治疗的全部内容
        /// </summary>
        void ClearLastCureContent()
        {
            this.RealTimeTemperatureCollection = new CRealtimeTemperatureCollection();
            this.PresettedSequence = null;
            this.StartCure = DateTime.MinValue;
            this.PatientName = "";
            this.CureSN = "";
            this.CureElapsedEffective = 0;
            this.TimeRemainedInStage = 0;
            this.CureProgressInStage = 0;
            this.RealtimeTemperature = 0;
            this.CureAction = ENUM_ACTION.Stopped;

        }

        /// <summary>
        /// 播放治疗完成的语音提示
        /// </summary>
        void PlayCompleteVoice()
        {
            SoundPlayer player = new SoundPlayer(string.Format(".\\voice\\{0}{1}.wav", CPublicVariables.Configuration.CompleteVoiceFilePrefix, this.Channel + 1));
            player.PlaySync();
        }
        #endregion

        #region Properties

        public int Channel
        {
            set
            {
                int_channel = value;
                RaisePropertyChanged("Channel");
            }
            get
            {
                return int_channel;
            }
        }

        /// <summary>
        /// 开始治疗时间
        /// </summary>
        public DateTime StartCure
        {
            get
            {
                return dt_startcure;
            }
            private set
            {
                dt_startcure = value;
                RaisePropertyChanged("StartCure");
            }
        }

        /// <summary>
        /// 治疗进度（%）
        /// </summary>
        public double CureProgressOverall
        {
            get
            {
                if (cure_time_needed > 0)
                {
                    double percent = (double)this.CureElapsedEffective / (double)this.CureTimeNeeded * 100;
                    return percent;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// 返回温度阶段内的完成进度（%)
        /// </summary>
        public double CureProgressInStage
        {
            private set
            {
                dbl_progress_in_stage = value;
                RaisePropertyChanged("CureProgressInStage");
            }
            get
            {
                return dbl_progress_in_stage;
            }
        }

        /// <summary>
        /// 治疗花费的总时间（TimeSpan）
        /// 包括暂停时间
        /// </summary>
        public TimeSpan CureElapsedOveralll
        {
            get
            {
                return DateTime.Now - this.StartCure;
            }
        }

        /// <summary>
        /// 治疗花费的有效时间（秒）
        /// 不包含暂停时间
        /// </summary>
        public int CureElapsedEffective
        {
            private set
            {
                cure_elapsed_effective = value;
                RaisePropertyChanged("CureElapsedEffective");
                RaisePropertyChanged("CureProgressOverall");
                RaisePropertyChanged("TimeRemained");
            }
            get
            {
                return cure_elapsed_effective;
            }
        }

        /// <summary>
        /// 剩余的治疗时间（秒）
        /// </summary>
        public int TimeRemained
        {
            get
            {
                return this.CureTimeNeeded - this.CureElapsedEffective;
            }
        }

        /// <summary>
        /// 返回本阶段剩余的秒数
        /// </summary>
        public int TimeRemainedInStage
        {
            private set
            {
                time_remained_in_stage = value;
                RaisePropertyChanged("TimeRemainedInStage");
            }
            get
            {
                return time_remained_in_stage;
            }
        }

        /// <summary>
        /// 本次治疗需要的总时间（秒），包含所有阶段
        /// </summary>
        public int CureTimeNeeded
        {
            private set
            {
                cure_time_needed = value;
                RaisePropertyChanged("CureTimeNeeded");
            }
            get
            {
                return cure_time_needed;
            }
        }

        /// <summary>
        /// 治疗带使用总时间（小时）
        /// </summary>
        public int CureBandServiceTime
        {
            private set
            {
                cureband_used_time_overall = value;
                RaisePropertyChanged("CureBandServiceTime");
            }
            get
            {
                return cureband_used_time_overall;
            }
        }

        /// <summary>
        /// 设置或返回实时温度
        /// </summary>
        public double RealtimeTemperature
        {
            private set
            {
                realtime_temperature = value;
                RaisePropertyChanged("RealtimeTemperature");
            }
            get
            {
                return realtime_temperature;
            }
        }

        /// <summary>
        /// 返回当前阶段的预设温度
        /// </summary>
        public double PresetTemperatureInCurrentStage
        {
            private set
            {
                preset_temperature_from_board = value;
                RaisePropertyChanged("PresetTemperatureInCurrentStage");
            }
            get
            {
                return preset_temperature_from_board;
            }
        }

        /// <summary>
        /// 治疗带状态
        /// </summary>
        public ENUM_STATE CureBandState
        {
            private set
            {
                band_state = value;
                RaisePropertyChanged("CureBandState");
            }
            get
            {
                return band_state;
            }
        }

        /// <summary>
        /// 治疗动作
        /// </summary>
        public ENUM_ACTION CureAction
        {
            set
            {
                band_action = value;
                RaisePropertyChanged("CureAction");
            }
            get
            {
                return band_action;
            }
        }

        /// <summary>
        /// 实时温度曲线
        /// </summary>
        public CRealtimeTemperatureCollection RealTimeTemperatureCollection
        {
            private set
            {
                realtime_temperature_collection = value;
                RaisePropertyChanged("RealTimeTemperatureCollection");
            }
            get
            {
                return realtime_temperature_collection;
            }
        }

        /// <summary>
        /// 设置或返回预设的温度序列
        /// </summary>
        public CTemperatureSequence PresettedSequence
        {
            set
            {
                presetted_sequence = value;
                RaisePropertyChanged("PresettedSequence");
            }
            get
            {
                return presetted_sequence;
            }
        }

        /// <summary>
        /// 设置或返回治疗编号
        /// </summary>
        public string CureSN
        {
            set
            {
                cure_sn = value;
                RaisePropertyChanged("CureSN");
            }
            get
            {
                return cure_sn;
            }
        }

        /// <summary>
        /// 设置或返回患者姓名
        /// </summary>
        public string PatientName
        {
            get
            {
                return patient_name;
            }
            set
            {
                patient_name = value;
                RaisePropertyChanged("PatientName");
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 产生倒计时时基，供治疗线程倒计时
        /// </summary>
        /// <param name="state"></param>
        private void OnTimerIntervalEvent(object state)
        {
            try
            {
                sem_countdown_tick.Release();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 开始治疗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgw_cure_DoWork(object sender, DoWorkEventArgs e)
        {
            /*=========================================== 判断治疗带是否过期 ===============================================*/
            /* 
            * 如果治疗带过期，并且治疗还未启动，则退出 
            * 如果正在治疗中，则继续治疗，但是避免下一次启动治疗
            */
            if (this.CureBandState == ENUM_STATE.Overdue && this.CureElapsedEffective == 0)
            {
                bgw_cure.ReportProgress(CURE_PROG_REPORT_CUREBAND_EXPIRED);
                return;
            }
            /*=========================================================================================================*/

            CDatabase db = new CDatabase();

            /* 
            * 启动控制板 
            */
            control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.SetTemper, this.Channel, this.PresetTemperatureInCurrentStage));
            Thread.Sleep(1000);
            control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.Start, this.Channel, 0));
            Thread.Sleep(2000);
            /*
            * 这里必须将cure_stage赋值为-1，第一次进入this.PresettedSequence.WhichStageBelongTo()函数时会产生一次StageChanged，
            * 这样可以触发一次SetTemperature事件；否则，第一个stage的温度没有机会设置到主控板
            */
            cure_stage = -1;

            /* 获取治疗总时间 */
            this.CureTimeNeeded = (int)this.PresettedSequence.Sum(t => t.HoldTime) * 60;
            this.StartCure = DateTime.Now;

            /* 启动1s时基定时器 */
            timer_countdown_tick.Change(100, COUNTDOWN_BASETIME_MS);

            while (true)
            {
                /* 等待倒计时时基中断 */
                sem_countdown_tick.WaitOne();

                #region Obsolete segment
                /*
                +++++++++++++++++++++++++++++++++++
                +  获取数据的部分被移至通讯成功事件
                +++++++++++++++++++++++++++++++++++
                */
                /* 获取治疗带数据 */
                //if (control_central.BoardInfo.State.Length > 0)
                //{
                /* 获取实时温度 */
                //this.RealtimeTemperature = control_central.BoardInfo.Temperature[this.Channel];

                /* 获取状态 */
                //this.CureBandState = control_central.BoardInfo.State[this.Channel];

                /* 获取治疗带使用时间 */
                //this.CureBandServiceTime = control_central.BoardInfo.CureBandServiceTime[this.Channel];   
                //}
                #endregion

                #region Check user's interrupt
                /*============== 检查用户是否请求停止治疗过程 ==============*/
                if (bgw_cure.CancellationPending)
                {
                    bgw_cure.ReportProgress(CURE_PROG_REPORT_CANCELED);
                    return;
                }
                /*=================================================*/
                #endregion  

                /*============================================= 判断治疗带是否在加热 ============================================*/
                /* 
                * 检查治疗带是否已启动
                * 因为控制板有可能因为I2C通讯问题重启，此时所有通道会重置到停止状态，这种情况下需要返回的治疗带状态是停止，
                * 此时则需要重新启动一下
                */
                if (this.CureBandState == ENUM_STATE.Standby)
                {

                    control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.SetTemper, this.Channel, this.PresetTemperatureInCurrentStage));
                    control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.Start, this.Channel, 0));

                    Thread.Sleep(2000); // 等待启动

                    continue;
                }
                /*=========================================================================================================*/

                #region Check user's interrupt
                /*============== 检查用户是否请求停止治疗过程 ==============*/
                if (bgw_cure.CancellationPending)
                {
                    bgw_cure.ReportProgress(CURE_PROG_REPORT_CANCELED);
                    return;
                }
                /*=================================================*/
                #endregion

                /* 如果状态是正在加热，则等待，同时发送设置温度的命令，以确保目标温度写入控制板 */
                if (this.CureBandState == ENUM_STATE.Heating)
                {
                    control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.SetTemper, this.Channel, this.PresetTemperatureInCurrentStage));
                    Thread.Sleep(2000);
                    continue;
                }

                /*================================================ 具体的业务逻辑 =============================================*/
                /* 检查控制板返回的治疗带状态是否正常 
                ** 如果不在Curing状态，说明出现故障，则暂停计时，等待用户处理硬件故障 
                */
                if (this.CureBandState == ENUM_STATE.Curing)
                {

                    /* 检查当前治疗属于哪个阶段 */
                    this.PresettedSequence.WhichStageBelongTo(this.CureElapsedEffective, cure_stage, out cure_stage, out bln_stage_changed, out time_remained_in_stage, out dbl_progress_in_stage);
                    this.CureProgressInStage = dbl_progress_in_stage;
                    this.TimeRemainedInStage = time_remained_in_stage;

                    // 如果温度阶段发生变化，则读取预设温度，并将预设温度写入控制板
                    if (bln_stage_changed)
                    {
                        /* 预设温度 */
                        this.PresetTemperatureInCurrentStage = this.PresettedSequence[cure_stage].TargetTemperature;

                        /* 向控制板设置加热温度 */
                        control_central.SetControllerWorks(
                            new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.SetTemper, this.Channel, this.PresetTemperatureInCurrentStage));
                    }

                    /* 添加实时温度 */
                    bgw_cure.ReportProgress(CURE_PROG_REPORT_ADD_RTT);

                    /* 每一秒获取的温度均保存在数据库中 */
                    if (db.InsertNewCureDetail(this.CureSN, this.RealtimeTemperature, this.CureElapsedEffective) == false)
                    {
                        // 如果保存数据失败，退出治疗过程
                        bgw_cure.ReportProgress(CURE_PROG_REPORT_DB_ERR, db.LastError);
                        return;
                    }

                    /*
                    * 检查治疗是否完成 
                    */
                    if (this.CureElapsedEffective >= this.CureTimeNeeded)     // 治疗完成
                    {
                        bgw_cure.ReportProgress(CURE_PROG_REPORT_DONE);
                        return;
                    }

                    /* 增加治疗时间 */
                    this.CureElapsedEffective++;
                }
                /*=========================================================================================================*/

                #region Check user's interrupt
                /*============== 检查用户是否请求停止治疗过程 ==============*/
                if (bgw_cure.CancellationPending)
                {
                    bgw_cure.ReportProgress(CURE_PROG_REPORT_CANCELED);
                    return;
                }
                /*=================================================*/
                #endregion
            }
        }

        /// <summary>
        /// 治疗进度和状态报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgw_cure_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case CURE_PROG_REPORT_CANCELED: // 用户终止治疗
                    this.CureAction = ENUM_ACTION.Stopped;
                    /* 清空上一次的治疗信息 */
                    ClearLastCureContent();
                    break;

                case CURE_PROG_REPORT_DONE:

                    this.CureAction = ENUM_ACTION.Finished;
                    /* 这里播放音频文件 */
                    PlayCompleteVoice();
                    break;

                case CURE_PROG_REPORT_DB_ERR:
                    MessageBox.Show("保存温度数据时发生错误，治疗终止\r\n错误：" + e.UserState.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case CURE_PROG_REPORT_ADD_RTT:
                    /* 实时温度画在曲线上 */
                    this.RealTimeTemperatureCollection.Add(new CRealtimeTemperaturePoint(
                            this.CureElapsedEffective,
                            this.RealtimeTemperature));
                    //RaisePropertyChanged("RealTimeTemperatureCollection");
                    break;

                case CURE_PROG_REPORT_CUREBAND_EXPIRED:
                    /* 关闭控制板加热 */
                    control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.Stop, this.Channel, 0));
                    this.RealtimeTemperature = 0;
                    this.CureAction = ENUM_ACTION.Stopped;
                    break;
            }
        }

        private void Bgw_cure_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /* 关闭控制板加热 */
            control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.Stop, this.Channel, 0));
        }

        private void Control_central_ControllerCommStatusChanged(object sender, CommProgressReportArg e)
        {
            switch (e.Progress)
            {

                case CommProgressReportArg.ENUM_COMM_EVENT_TYPE.Finish:
                    /* 获取治疗带使用时间 */
                    this.CureBandServiceTime = control_central.BoardInfo.CureBandServiceTime[this.Channel];


                    /* 获取治疗带状态 */
                    // 如果使用时间达到最大值，则标记为过期
                    if (this.CureBandServiceTime >= CPublicVariables.Configuration.MaxCureBandServieTime && !bgw_cure.IsBusy)
                        this.CureBandState = ENUM_STATE.Overdue;
                    else
                        this.CureBandState = control_central.BoardInfo.State[this.Channel];


                    /* 显示实时温度 */
                    if (this.CureBandState == ENUM_STATE.Standby || this.CureBandState == ENUM_STATE.Curing || this.CureBandState == ENUM_STATE.Heating)
                        this.RealtimeTemperature = control_central.BoardInfo.Temperature[this.Channel];
                    else
                        this.realtime_temperature = 0.0;
                    break;

                case CommProgressReportArg.ENUM_COMM_EVENT_TYPE.PortOpened:
                    /* 
                    * 产生PortOpened事件有两种情况：
                    * 1、程序启动
                    * 2、通讯中断后重新恢复连接
                    * 如果出现第二种情况，可能是控制板断电或者其它情况造成的重启，此时需要根据PC端记录的状态恢复每个通道的工作状态，
                    * 因此这里向每个通道重新写入控制信息
                    */
                    if (this.CureAction == ENUM_ACTION.Started || this.CureAction == ENUM_ACTION.Paused)
                    {
                        control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.Start, this.Channel, 0));
                        control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.SetTemper, this.Channel, this.PresetTemperatureInCurrentStage));
                    }
                    else
                    {
                        control_central.SetControllerWorks(new CCommandQueueItem(CCommandQueueItem.ENUM_CMD.Stop, this.Channel, 0));
                    }
                    break;
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// 启动治疗或恢复治疗命令
        /// </summary>
        public RelayCommand<object> StartOrResume
        {
            get
            {
                if (command_start_resume == null)
                    command_start_resume = new RelayCommand<object>(p => StartOrResumeExecute(p));
                return command_start_resume;
            }
        }

        /// <summary>
        /// 暂停治疗或停止治疗命令
        /// </summary>
        public RelayCommand<object> PauseOrStop
        {
            get
            {
                if (command_pause_stop == null)
                    command_pause_stop = new RelayCommand<object>(p => PauseOrStopExecute(p));
                return command_pause_stop;
            }
        }

        public RelayCommand<object> NewCureSetup
        {
            get
            {
                if (command_newcure_setup == null)
                    command_newcure_setup = new RelayCommand<object>(p => NewCureSetupExecute(p));
                return command_newcure_setup;
            }
        }

        /// <summary>
        /// 启动或恢复按钮按下的命令
        /// </summary>
        /// <param name="param"></param>
        void StartOrResumeExecute(object param)
        {
            this.Start();
        }

        /// <summary>
        /// 暂停或停止按钮按下的命令
        /// </summary>
        /// <param name="param"></param>
        void PauseOrStopExecute(object param)
        {
            if (this.CureAction == ENUM_ACTION.Started)
            {
                this.Pause();
                this.CureAction = ENUM_ACTION.Paused;
            }
            else if (this.CureAction == ENUM_ACTION.Paused)
            {
                if (MessageBox.Show("终止治疗将清空现有治疗数据，是否继续终止治疗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    this.Pause();
                    this.Reset();
                }
            }
        }

        /// <summary>
        /// 新建治疗的Command
        /// </summary>
        /// <param name="param"></param>
        void NewCureSetupExecute(object param)
        {
            ViewModelNewCureSetup vm = ((ViewModelLocator)Application.Current.Resources["Locator"]).NewCureSetup;
            vm.LoadParameters();
            Messenger.Default.Send<ViewModelNewCureSetup>(vm, "NewCureSetup");

            if (vm.Result)
            {
                /*
                * 清空上次的治疗信息
                */
                this.ClearLastCureContent();

                this.CureSN = vm.CureSN;
                this.PatientName = vm.PatientName;
                this.PresettedSequence = vm.Sequence;

                // Save the basic info of the cure progress
                // Which contains Patient name, sequence binary array, Cure SN, etc.
                using (CDatabase db = new CDatabase())
                {
                    if (db.CreateCureHistory(CureSN, PatientName, Channel, PresettedSequence) == false)
                    {
                        MessageBox.Show("创建新的治疗记录时发生错误\r\n错误：" + db.LastError, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion
    }
}
