using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class CControlCentral : IDisposable
    {
        #region Variables
       
        public const int STAT_COMM_QUERY_FINISHED = 100;
        public const int STAT_COMM_QUERY_ERR = 99;
        public const int STAT_COMM_PORT_INEXITSTENT = 98;
        public const int STAT_COMM_PORT_OPENING = 0;
        public const int STAT_COMM_PORT_OPENED = 1;
        /// <summary>
        /// 授权码错误
        /// </summary>
        public const int STAT_COMM_LIC_ERR = 80;

        public const int STAAT_COMM_LIC_OK = 81;

        /// <summary>
        /// 通讯状态发生改变
        /// </summary>
        public event EventHandler<CommProgressReportArg> ControllerCommStatusChanged;

        string strLastError;

        /// <summary>
        /// 控制电路板实例
        /// </summary>
        CControlBoard mControlBoard;

        /// <summary>
        /// 采集板子的状态
        /// </summary>
        CControlBoardInfo mBoardInfo;

        

        /// <summary>
        /// 后台通讯线程，负责和控制器进行通讯
        /// </summary>
        BackgroundWorker bgw_RS232;

        /// <summary>
        /// 串口命令发送列队
        /// </summary>
        Queue<CCommandQueueItem> queSendingCommands;


        #endregion

        #region Constructors
        public CControlCentral()
        {
            mControlBoard = new CControlBoard();
            mBoardInfo = new CControlBoardInfo();

            queSendingCommands = new Queue<CCommandQueueItem>();
            bgw_RS232 = new BackgroundWorker();
            bgw_RS232.WorkerReportsProgress = true;
            bgw_RS232.WorkerSupportsCancellation = true;
            bgw_RS232.DoWork += bgw_DoWork;
            bgw_RS232.ProgressChanged += bgw_ProgressChanged;
            bgw_RS232.RunWorkerAsync();


        }
        #endregion

        #region Properties
        /// <summary>
        /// 返回控制板信息
        /// </summary>
        public CControlBoardInfo BoardInfo
        {
            get
            {
                return mBoardInfo;
            }
        }

        /// <summary>
        /// 设置或返回肿瘤舱运行状态
        /// </summary>
        public bool IsRunning
        {
            private set;
            get;
        }

        public string LastError
        {
            get
            {
                return strLastError;
            }
        }
        #endregion

        #region Events
        //void controlboard_QueriedDataCompleted(object sender, bool e)
        //{
        //    if (ControllerCommStatusChanged != null)
        //        ControllerCommStatusChanged(this, e);

        //    if(e) // 如果返回的数据都是有效的
        //    {
        //        mTemperatureController.SetTemperature(mBoardInfo.AverageTemperature); // 给温度控制类设置当前平均温度
        //    }
        //}

        /// <summary>
        /// 当通讯线程完成一次通讯后，向UI汇报
        /// 可以用来更新UI上的通讯指示灯状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CommProgressReportArg.ENUM_COMM_EVENT_TYPE type = CommProgressReportArg.ENUM_COMM_EVENT_TYPE.Err;
            string msg = string.Empty;
            if (e.ProgressPercentage == STAT_COMM_QUERY_ERR)
            {
                type = CommProgressReportArg.ENUM_COMM_EVENT_TYPE.Err;
                msg = "查询控制板信息失败，" + mControlBoard.LastError;
            }
            else if(e.ProgressPercentage == STAT_COMM_PORT_INEXITSTENT)
            {
                type = CommProgressReportArg.ENUM_COMM_EVENT_TYPE.Err;
                msg = LastError;
            }
            else if (e.ProgressPercentage == STAT_COMM_QUERY_FINISHED)
                type = CommProgressReportArg.ENUM_COMM_EVENT_TYPE.Finish;
            else if (e.ProgressPercentage == STAT_COMM_PORT_OPENING)
            {
                type = CommProgressReportArg.ENUM_COMM_EVENT_TYPE.PortOpening;
                msg = string.Format("正在打开串口{0}...", mControlBoard.PortName);
            }
            else if (e.ProgressPercentage == STAT_COMM_PORT_OPENED)
                type = CommProgressReportArg.ENUM_COMM_EVENT_TYPE.PortOpened;

            if (ControllerCommStatusChanged != null)
            {
                ControllerCommStatusChanged(this, new CommProgressReportArg(type, msg));
            }
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            //int cdkey_daysleft = -1;
            //DateTime cdkey_expired = DateTime.MinValue;

            while (true)
            {
                bgw_RS232.ReportProgress(STAT_COMM_PORT_OPENING);
                if (!mControlBoard.Open()) // 如果打开串口失败
                {
                    strLastError = mControlBoard.LastError;

                    mBoardInfo.RS232Connected = false;
                    Thread.Sleep(1000);

                    /* 检查是否请求退出通讯过程 */
                    if (bgw_RS232.CancellationPending)
                    {
                        mControlBoard.Close();
                        return;
                    }

                    bgw_RS232.ReportProgress(STAT_COMM_PORT_INEXITSTENT);
                    Thread.Sleep(1000);
                }
                else
                {
                    bgw_RS232.ReportProgress(STAT_COMM_PORT_OPENED);

                    mBoardInfo.RS232Connected = true; // 成功连接到控制箱


                    while (true)
                    {

                        Thread.Sleep(300);

                        /* 查询控制器各项参数 */
                        if (mControlBoard.QueryStatus(ref mBoardInfo)) // 查询状态成功
                        {
                            bgw_RS232.ReportProgress(STAT_COMM_QUERY_FINISHED);
                        }
                        else    // 查询状态失败
                        {
                            queSendingCommands.Clear();
                            strLastError = mControlBoard.LastError;
                            bgw_RS232.ReportProgress(STAT_COMM_QUERY_ERR);
                            mControlBoard.Close();
                            Thread.Sleep(1000);
                            break;  // 如果查询状态失败，重新连接串口
                        }


                        /******************** Send control commands in quene *****************/
                        CCommandQueueItem cmd = null;
                        lock (queSendingCommands)
                        {
                            if (queSendingCommands.Count > 0)
                                cmd = queSendingCommands.Dequeue();
                        }
                        if (cmd != null)
                        {
                            Thread.Sleep(100);

                            switch (cmd.Command)
                            {
                                case CCommandQueueItem.ENUM_CMD.Start:
                                    mControlBoard.StartCuring(cmd.Channel);
                                    break;

                                case CCommandQueueItem.ENUM_CMD.Stop:
                                    mControlBoard.StopCuring(cmd.Channel);
                                    break;

                                case CCommandQueueItem.ENUM_CMD.SetTemper:
                                    mControlBoard.SetTemperature(cmd.Channel, cmd.Temperature);
                                    break;
                            }

#if DEBUG
                            Trace.WriteLine(string.Format("Command Send >>> {0}", cmd.ToString()));
                            Trace.WriteLine(string.Format("Commands remained {0}", queSendingCommands.Count.ToString()));
#endif
                        }


                        /* 检查是否请求退出通讯过程 */
                        if (bgw_RS232.CancellationPending)
                        {
                            mControlBoard.Close();
                            return;
                        }
                        /******************************************************************/


                        /* 检查cdkey是否正确 */

                        //int cdkey_avaliable = CStaticMethods.CheckLicenseAvaliable(CPublicVariables.Configuration.MachineCode, CStaticMethods.GetActiveCode(), CPublicVariables.Configuration.CDKEY, out cdkey_expired, out cdkey_daysleft);
                        //if (cdkey_avaliable != 0)    // CDKEY验证错误
                        //{
                        //    bgw_RS232.ReportProgress(STAT_COMM_LIC_ERR, cdkey_avaliable);
                        //    Thread.Sleep(1000);

                        //    /* 控制器恢复到初始状态 */
                        //    if (this.BoardInfo.ZeroPower == true)
                        //        this.SetControllerWorks(CMD_ZERO_PWR_OFF);  // 关闭零点电源

                        //    if (this.BoardInfo.PreheaterStatus == true)
                        //        this.SetControllerWorks(CMD_PREHEATER_OFF); // 关闭预加热

                        //    if (this.BoardInfo.HeaterLevel != CControlBoardInfo.Enum_HeaterLevel.LOW)   // 加热器调到最低档
                        //        this.SetControllerWorks(CMD_HEATER_LEVEL_LOW);

                        //    if (this.BoardInfo.FanOpened != 0)              // 关闭降温风扇
                        //        this.SetControllerWorks(CMD_COOL_FAN_OFF);

                        //}
                        //else
                        //{

                        //    bgw_RS232.ReportProgress(STAAT_COMM_LIC_OK);

                        //    /* 当前温度写入温度控制类，进行温度控制 */
                        //    mTemperatureTunner.SetTemperature(mBoardInfo.MovingAverageTemperatureInCabin);
                        //}


                        if (mControlBoard.CheckPortChanged())   /* 检查串口号是否在系统设置里做了更改 */
                        {
                            mControlBoard.Close();
                            break; // 如果串口号更改了，则重新打开控制器
                        }

                        /* 检查是否请求退出通讯过程 */
                        if (bgw_RS232.CancellationPending)
                        {
                            mControlBoard.Close();
                            return;
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 向控制器发送控制命令
        /// </summary>
        /// <param name="Work"></param>
        public void SetControllerWorks(CCommandQueueItem Command)
        {
            lock(queSendingCommands)
            {
                if (queSendingCommands.Count <= 10)
                {
                    queSendingCommands.Enqueue(Command);
#if DEBUG
                    Trace.WriteLine(string.Format("Command Item Added >>> {0}", Command.ToString()));
                    Trace.WriteLine(string.Format("Commands remained {0}", queSendingCommands.Count.ToString()));
#endif
                }
            }
        }
        #endregion

        public void Dispose()
        {
            bgw_RS232.CancelAsync();
        }

    }

    /// <summary>
    /// 向UI汇报通讯进度
    /// </summary>
    public class CommProgressReportArg : EventArgs
    { 
        public enum ENUM_COMM_EVENT_TYPE
        {
            Err,
            Finish,
            PortOpening,
            PortOpened
        }
        public CommProgressReportArg(ENUM_COMM_EVENT_TYPE Progress, string Info)
        {
            this.Progress = Progress;
            this.Info = Info;
        }

        public CommProgressReportArg(ENUM_COMM_EVENT_TYPE Progress)
        {
            this.Progress = Progress;
            this.Info = "";
        }

        public ENUM_COMM_EVENT_TYPE Progress
        {
            private set;
            get;
        }

        public string Info
        {
            private set;
            get;
        }
    }
}
