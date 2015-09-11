using System;
using System.IO.Ports;
using System.Text;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    /// <summary>
    /// 控制电路板类
    /// </summary>
    public class CControlBoard
    {
        #region Variables
        
        public enum Enum_ONOFF
        {
            ON, OFF
        }

        const int LOADER_COUNT = 90; // 负载总数
        const int TEMPERATURE_SENSOR_COUNT = 4; // 温度传感器总数

        SerialPort port; // 串口类
        string strLastError; // 最后一个错误信息

        /// <summary>
        /// 通讯错误的次数
        /// </summary>
        int intCommErrCnt;
        
        #endregion

        #region Constructors
        public CControlBoard()
        {
            port = new SerialPort("COM1", 115200, Parity.None, 8, StopBits.One);
        }

        #endregion

        #region Properties

        public string PortName
        {
            get
            {
                return port.PortName;
            }
        }

        public  string LastError
        {
            get
            {
                return strLastError;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
#if !DEBUG
            port.PortName = CPublicVariables.Configuration.ControllerPort;
            port.ReadTimeout = 3000;
            if (port.IsOpen)
                port.Close();
            try
            {
                port.Open();
                return true;
            }
            catch(Exception ex)
            {
                strLastError = ex.Message;
                return false;
            }
#else
            port.PortName = CPublicVariables.Configuration.ControllerPort;
            port.ReadTimeout = 3000;
            return true;
#endif
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                port.Close();
                return true;
            }
            catch (Exception ex)
            {
                strLastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 查询控制板数据
        /// </summary>
        /// <returns></returns>
        
        #endregion

        #region Public Methods
        public bool QueryStatus(ref CControlBoardInfo BoardInfo)
        {
#if DEBUG
             StringBuilder boardquery = new StringBuilder();
            Random r = new Random(DateTime.Now.Second + DateTime.Now.Millisecond);

            for (int i = 0; i < 8; i++)
            {
                // 随机产生治疗状态

                //double tmp = r.NextDouble();
                //if (tmp < 0.25)
                //    boardquery.Append("0");     // Standby
                //else if (tmp < 0.5)
                //    boardquery.Append("1");     // Curing
                //else if (tmp < 0.75)
                //    boardquery.Append("2");     // Heating
                //else
                //    boardquery.Append("e");     // Disconnected

                boardquery.Append("1");
                boardquery.Append(",");

                // 随机产生治疗带使用时间
                boardquery.Append(r.Next(1000).ToString());
                boardquery.Append(",");

                // 随机产生治疗带温度
                boardquery.Append(((40 + r.NextDouble() * 10) * 100).ToString("F0"));

                if (i < 7)
                    boardquery.Append(";");
            }
            
            BoardInfo.AnalyzeQuaryString(boardquery.ToString());

            return true;
#else
            bool blnRet = false;
            try
            {
                port.WriteLine("Q?\r");
                string ret = port.ReadLine();
                ret = ret.Replace("\r", "").Replace("\n", "").Trim();
                BoardInfo.AnalyzeQuaryString(ret);
                blnRet = true;
                intCommErrCnt = 0;
            }
            catch (Exception ex)
            {
                intCommErrCnt++;
                if (intCommErrCnt >= 10)
                {
                    strLastError = ex.Message;
                    blnRet = false;
                }
                else
                {
                    blnRet = true;
                }
            }
            return blnRet;
#endif


        }

        /// <summary>
        /// 检查串口号是否在系统设置里更改
        /// </summary>
        /// <returns></returns>
        public  bool CheckPortChanged()
        {
            if (port.PortName == CPublicVariables.Configuration.ControllerPort)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 开始治疗
        /// </summary>
        /// <param name="Channel">启动的通道1~8</param>
        /// <returns></returns>
        public bool StartCuring(int Channel)
        {
            string cmd = "S" + Channel.ToString() + "\r";
            try
            {
                port.WriteLine(cmd);
                return true;
            }
            catch (Exception ex)
            {
                strLastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 停止治疗
        /// </summary>
        /// <param name="Channel">通道1~8</param>
        /// <returns></returns>
        public bool StopCuring(int Channel)
        {
            string cmd = "E" + Channel.ToString() + "\r";
            try
            {
                port.WriteLine(cmd);
                return true;
            }
            catch (Exception ex)
            {
                strLastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public bool SetTemperature(int Channel, double Temperature)
        {
            string cmd = "T" + Channel.ToString() + (Temperature * 100).ToString("F0") + "\r";
            try
            {
                port.WriteLine(cmd);
                return true;
            }
            catch (Exception ex)
            {
                strLastError = ex.Message;
                return false;
            }
        }
        #endregion  
    }
}
