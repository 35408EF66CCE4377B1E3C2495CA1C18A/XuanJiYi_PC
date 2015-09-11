using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class CConfiguration : INotifyPropertyChanged, ICloneable
    {
        #region Variables
        public event PropertyChangedEventHandler PropertyChanged;
        string strControllerPort;
        string strSqlServerIP;
        string strSqlServerUser;
        string strSqlServerPassword;
        string[] strBloodType;
        string strStationName;
        string strSystemUserName;
        string strSystemIP;
        double dblTargetTemperature;
        string strZoneCode;
        int intTempTuneSilence;
        double dblTemperatureTuneDeadArea;
        double dblTemperatrueTuneThreshold;
        string strCDKEY;
        string strMachineCode;
        string strActiveCode;
        int intCDKEY_Daysleft;
        DateTime dtCDKEY_Expried;
        long intRunningHours = 0;
        int intLastCureDuration;
        string strAssemblyVersion;
        int intHVGLevel;
        string strLoginPassword;
        int intMaxCureBandServieTime;
        string strCompleteVoiceFilePrefix;
        int intMaxCureTimeAllowed;
        #endregion

        #region Constructors
        public CConfiguration()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            strStationName = Environment.MachineName;
            strSystemUserName = Environment.UserName;
            strSystemIP = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();

            this.LoginPassword = cfg.AppSettings.Settings["Login"].Value;

            this.ControllerPort = cfg.AppSettings.Settings["ControlBoardPort"].Value;

            this.SqlServerIP = cfg.AppSettings.Settings["SqlServerIP"].Value;
            this.SqlServerUser = cfg.AppSettings.Settings["SqlServerUser"].Value;
            this.SqlServerPassword = cfg.AppSettings.Settings["SqlServerPassword"].Value;

            this.BloodType = cfg.AppSettings.Settings["BloodType"].Value.Split(';');

            this.TargetTemperature = Convert.ToDouble(cfg.AppSettings.Settings["TargetTemperature"].Value);

            this.ZoneCode = cfg.AppSettings.Settings["ZoneCode"].Value;

            this.TempTuneSilence = Convert.ToInt16(cfg.AppSettings.Settings["TempTuneSilence"].Value);
            this.TemperatureTuneDeadArea = Convert.ToDouble(cfg.AppSettings.Settings["TemperatureTuneDeadArea"].Value);
            this.TemperatureTuneThreshold = Convert.ToDouble(cfg.AppSettings.Settings["TemperatureTuneThreshold"].Value);

            //strMachineCode = CStaticMethods.GetMachineSN();
            //strActiveCode = CStaticMethods.GetActiveCode();

            this.CDKEY = cfg.AppSettings.Settings["CDKEY"].Value;
           
            this.LastCureDuration = Convert.ToInt32(cfg.AppSettings.Settings["LastCureDuration"].Value);
            this.MaxCureTimeAllowed = Convert.ToInt32(cfg.AppSettings.Settings["MaxCureTimeAllowed"].Value);

            /* 返回程序版本号 */
            Version ApplicationVersion = new Version(Application.ProductVersion);
            this.AssemblyVersion = ApplicationVersion.ToString();

            this.MaxCureBandServieTime = Convert.ToInt32(cfg.AppSettings.Settings["MaxCureBandServieTime"].Value);
            this.CompleteVoiceFilePrefix = cfg.AppSettings.Settings["CompleteVoiceFilePrefix"].Value;
        }

        public CConfiguration(CConfiguration Cloned)
        {
            strStationName = Cloned.StationName;
            strSystemUserName = Cloned.SystemUserName;
            strSystemIP = Cloned.SystemIP;


            this.ControllerPort = Cloned.ControllerPort;

            this.SqlServerIP = Cloned.SqlServerIP;
            this.SqlServerUser = Cloned.SqlServerUser;
            this.SqlServerPassword = Cloned.SqlServerPassword;

            this.BloodType = Cloned.BloodType;

            this.TargetTemperature = Cloned.TargetTemperature;

            this.ZoneCode = Cloned.ZoneCode;

            this.TempTuneSilence = Cloned.TempTuneSilence;
            this.TemperatureTuneDeadArea = Cloned.TemperatureTuneDeadArea;
            this.TemperatureTuneThreshold = Cloned.TemperatureTuneThreshold;

            strMachineCode = Cloned.MachineCode;
            strActiveCode = Cloned.ActiveCode;

            strCDKEY = Cloned.CDKEY;

            this.CDKEY_Expired = Cloned.CDKEY_Expired;
            this.CDKEY_DaysLeft = Cloned.CDKEY_DaysLeft;

            this.LastCureDuration = Cloned.LastCureDuration;
            this.MaxCureTimeAllowed = Cloned.MaxCureTimeAllowed;

            /* 获取设备使用时长 */
            Int64 duration;
            using (CDatabase db = new CDatabase())
            {
                db.GetRunDuration(out duration);
                this.RunningHours = duration;
            }

            this.HVGLevel = Cloned.HVGLevel;

            /* 返回程序版本号 */
            this.AssemblyVersion = Cloned.AssemblyVersion;

            this.MaxCureBandServieTime = Cloned.MaxCureBandServieTime;
            this.CompleteVoiceFilePrefix = Cloned.CompleteVoiceFilePrefix;
        }
        #endregion

        #region Properties
        public string LoginPassword
        {
            set
            {
                strLoginPassword = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LoginPassword"));
            }
            get
            {
                return strLoginPassword;
            }
        }

        /// <summary>
        /// 设置或返回设备名称
        /// </summary>
        public string StationName
        {
            set
            {
                strStationName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("StationName"));
            }
            get
            {
                return strStationName;
            }
        }

        /// <summary>
        /// 设置或返回登陆系统的用户名称
        /// </summary>
        public string SystemUserName
        {
            set
            {
                strSystemUserName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SystemUserName"));
            }
            get
            {
                return strSystemUserName;
            }
        }

        /// <summary>
        /// 设置或返回系统IP地址
        /// </summary>
        public string SystemIP
        {
            set
            {
                strSystemIP = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SystemIP"));
            }
            get
            {
                return strSystemIP;
            }
        }

        /// <summary>
        /// 设置或返回控制器串口号
        /// </summary>
        public string ControllerPort
        {
            set
            {
                strControllerPort = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ControllerPort"));
            }
            get
            {
                return strControllerPort;
            }
        }

        /// <summary>
        /// 设置或获取数据库地址
        /// </summary>
        public string SqlServerIP
        {
            set
            {
                strSqlServerIP = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SqlServerIP"));
            }
            get
            {
                return strSqlServerIP;
            }
        }

        /// <summary>
        /// 设置或获取数据库用户名
        /// </summary>
        public string SqlServerUser
        {
            set
            {
                strSqlServerUser = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SqlServerUser"));
            }
            get
            {
                return strSqlServerUser;
            }
        }


        /// <summary>
        /// 设置或获取数据库密码
        /// </summary>
        public string SqlServerPassword
        {
            set
            {
                strSqlServerPassword = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SqlServerPassword"));
            }
            get
            {
                return strSqlServerPassword;
            }
        }

        /// <summary>
        /// 设置或获取用户血型
        /// </summary>
        public string[] BloodType
        {
            set
            {
                strBloodType = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BloodType"));
            }
            get
            {
                return strBloodType;
            }
        }

        
        /// <summary>
        /// 设置或获取视频录制路径
        /// </summary>
        public double TargetTemperature
        {
            set
            {
                dblTargetTemperature = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TargetTemperature"));
            }
            get
            {
                return dblTargetTemperature;
            }
        }

        /// <summary>
        /// 获取肿瘤舱区域代码
        /// </summary>
        public string ZoneCode
        {
            set
            {
                strZoneCode = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ZoneCode"));
            }
            get
            {
                return strZoneCode;
            }
        }

        /// <summary>
        /// 设置或获取温度调节器的静默时间，该参数用来防止风扇频繁启动
        /// 单位：秒
        /// </summary>
        public int TempTuneSilence
        {
            set
            {
                intTempTuneSilence = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TempTuneSilence"));
            }
            get
            {
                return intTempTuneSilence;
            }
        }

        /// <summary>
        /// 设置或获取温度调节器调节死区
        /// 单位：秒
        /// </summary>
        public double TemperatureTuneDeadArea
        {
            set
            {
                dblTemperatureTuneDeadArea = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TemperatureTuneDeadArea"));
            }
            get
            {
                return dblTemperatureTuneDeadArea;
            }
        }

        /// <summary>
        /// 设置或获取温度调节器调节阈值
        /// 单位：秒
        /// </summary>
        public double TemperatureTuneThreshold
        {
            set
            {
                dblTemperatrueTuneThreshold = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TemperatureTuneThreshold"));
            }
            get
            {
                return dblTemperatrueTuneThreshold;
            }
        }

        /// 设置或获取注册码
        /// 单位：秒
        /// </summary>
        public string CDKEY
        {
            set
            {
                strCDKEY = value;

                /* 获取当前序列号的有效期和有效状态 */
                //CStaticMethods.CheckLicenseAvaliable(this.MachineCode, this.ActiveCode, strCDKEY, out dtCDKEY_Expried, out intCDKEY_Daysleft);
                this.CDKEY_Expired = dtCDKEY_Expried;
                this.CDKEY_DaysLeft = intCDKEY_Daysleft;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CDKEY"));
                }
            }
            get
            {
                return strCDKEY;
            }
        }

        /// <summary>
        /// 返回机器识别号
        /// </summary>
        public string MachineCode
        {
            get
            {
                return strMachineCode;
            }
        }

        /// <summary>
        /// 返回激活码
        /// </summary>
        public string ActiveCode
        {
            get
            {
                return strActiveCode;
            }
        }

        /// <summary>
        /// 设置或返回CDKEY剩余天数
        /// </summary>
        public int CDKEY_DaysLeft
        {
            private set
            {
                intCDKEY_Daysleft = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CDKEY_DaysLeft"));
                }
            }
            get
            {
                return intCDKEY_Daysleft;

            }
        }

        /// <summary>
        /// 设置或返回CDKEY过期日期
        /// </summary>
        public DateTime CDKEY_Expired
        {
            private set
            {
                dtCDKEY_Expried = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CDKEY_Expired"));
                }
            }
            get
            {
                return dtCDKEY_Expried;

            } 
        }

        /// <summary>
        /// 返回设备工作时长
        /// </summary>
        public Int64 RunningHours
        {
            set
            {
                intRunningHours = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("RunningHours"));
                }
            }
            get
            {
                return intRunningHours;
            }
        }

        /// <summary>
        /// 最后一次设置的治疗时长，单位（分钟）
        /// </summary>
        public int LastCureDuration
        {
            set
            {
                intLastCureDuration = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LastCureDuration"));
                }
            }
            get
            {
                return intLastCureDuration;
            }
        }

        /// <summary>
        /// Get or return the max curing time allowed
        /// The typically value is 60 minutes
        /// </summary>
        public int MaxCureTimeAllowed
        {
            set
            {
                intMaxCureTimeAllowed = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxCureTimeAllowed"));
            }
            get
            {
                return intMaxCureTimeAllowed;
            }
        }

        /// <summary>
        /// 设置或返回高压发生器电压档位
        /// </summary>
        public int HVGLevel
        {
            set
            {
                intHVGLevel = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HVGLevel"));
                }
            }
            get
            {
                return intHVGLevel;
            }
        }

        /// <summary>
        /// 治疗带使用时间最大值（分钟）
        /// </summary>
        public int MaxCureBandServieTime
        {
            set
            {
                intMaxCureBandServieTime = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxCureBandServieTime"));
                }
            }
            get
            {
                return intMaxCureBandServieTime;
            }
        }

        /// <summary>
        /// 设置或返回治疗完成音频文件的前缀
        /// </summary>
        public string CompleteVoiceFilePrefix
        {
            set
            {
                strCompleteVoiceFilePrefix = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CompleteVoiceFilePrefix"));
                }
            }
            get
            {
                return strCompleteVoiceFilePrefix;
            }
        }

        /// <summary>
        /// 返回程序版本
        /// </summary>
        public  string AssemblyVersion
        {
            private set
            {
                strAssemblyVersion = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AssemblyVersion"));
                }
            }
            get
            {
                return strAssemblyVersion;
            }
        }

        
        #endregion

        #region Public Methods
        public void Save()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            cfg.AppSettings.Settings["Login"].Value = this.LoginPassword;

            cfg.AppSettings.Settings["ControlBoardPort"].Value = this.ControllerPort;

            cfg.AppSettings.Settings["SqlServerIP"].Value = this.SqlServerIP;
            cfg.AppSettings.Settings["SqlServerUser"].Value = this.SqlServerUser;
            cfg.AppSettings.Settings["SqlServerPassword"].Value = this.SqlServerPassword;


            cfg.AppSettings.Settings["TargetTemperature"].Value = this.TargetTemperature.ToString();

            cfg.AppSettings.Settings["TempTuneSilence"].Value = this.TempTuneSilence.ToString();
            cfg.AppSettings.Settings["TemperatureTuneDeadArea"].Value = this.TemperatureTuneDeadArea.ToString();
            cfg.AppSettings.Settings["TemperatureTuneThreshold"].Value = this.TemperatureTuneThreshold.ToString();

            cfg.AppSettings.Settings["CDKEY"].Value = this.CDKEY;

            cfg.AppSettings.Settings["LastCureDuration"].Value = this.LastCureDuration.ToString();
            cfg.AppSettings.Settings["MaxCureTimeAllowed"].Value = this.MaxCureTimeAllowed.ToString();
            /* 高压发生器电压档位 */
            cfg.AppSettings.Settings["HVGLevel"].Value = this.HVGLevel.ToString();

            cfg.AppSettings.Settings["MaxCureBandServieTime"].Value = this.MaxCureBandServieTime.ToString();
            cfg.AppSettings.Settings["CompleteVoiceFilePrefix"].Value = this.CompleteVoiceFilePrefix;

            cfg.Save(ConfigurationSaveMode.Modified);
        }

        public object Clone()
        {
            //CConfiguration cfg = new CConfiguration();
            //cfg.ControllerPort = this.ControllerPort;
            //cfg.SqlServerIP = this.SqlServerIP;
            //cfg.SqlServerUser = this.SqlServerUser;
            //cfg.SqlServerPassword = this.SqlServerPassword;
            //cfg.BloodType = this.BloodType;
            //cfg.VideoDevice = this.VideoDevice;
            //cfg.VideoClipSavePath = this.VideoClipSavePath;
            //cfg.TargetTemperature = this.TargetTemperature;
            //cfg.ZoneCode = this.ZoneCode;
            //cfg.TempTuneSilence = this.TempTuneSilence;
            //cfg.TemperatureTuneDeadArea = this.TemperatureTuneDeadArea;
            //cfg.TemperatureTuneThreshold = this.TemperatureTuneThreshold;
            //cfg.CDKEY = this.CDKEY;
            //cfg.CDKEY_DaysLeft = this.CDKEY_DaysLeft;
            //cfg.CDKEY_Expired = this.CDKEY_Expired;
            return new CConfiguration(this);
        }

        public override bool Equals(object obj)
        {
            CConfiguration Config = obj as CConfiguration;
            return
                (this.LoginPassword == Config.LoginPassword) &&
                (this.ControllerPort == Config.ControllerPort) &&
                (this.SqlServerIP == Config.SqlServerIP) &&
                (this.SqlServerUser == Config.SqlServerUser) &&
                (this.SqlServerPassword == Config.SqlServerPassword) &&
                (this.BloodType == Config.BloodType) &&
                (this.TargetTemperature == Config.TargetTemperature) &&
                (this.ZoneCode == Config.ZoneCode) &&
                (this.TempTuneSilence == Config.TempTuneSilence) &&
                (this.TemperatureTuneDeadArea == Config.TemperatureTuneDeadArea) &&
                (this.TemperatureTuneThreshold == Config.TemperatureTuneThreshold) &&
                (this.CDKEY == Config.CDKEY) &&
                (this.MachineCode == Config.MachineCode) &&
                (this.ActiveCode == Config.ActiveCode) &&
                (this.CDKEY_DaysLeft == Config.CDKEY_DaysLeft) &&
                (this.CDKEY_Expired == Config.CDKEY_Expired) &&
                (this.LastCureDuration == Config.LastCureDuration) &&
                (this.MaxCureTimeAllowed == Config.MaxCureTimeAllowed) &&
                (this.HVGLevel == Config.HVGLevel) &&
                (this.MaxCureBandServieTime == Config.MaxCureBandServieTime) &&
                (this.CompleteVoiceFilePrefix == Config.CompleteVoiceFilePrefix);

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 设置新值
        /// </summary>
        /// <param name="NewConfig"></param>
        public void SetValue(CConfiguration NewConfig)
        {
            this.LoginPassword = NewConfig.LoginPassword;

            this.ControllerPort = NewConfig.ControllerPort;

            this.SqlServerIP = NewConfig.SqlServerIP;
            this.SqlServerUser = NewConfig.SqlServerUser;
            this.SqlServerPassword = NewConfig.SqlServerPassword;

            this.TargetTemperature = NewConfig.TargetTemperature;

            this.TempTuneSilence = NewConfig.TempTuneSilence;
            this.TemperatureTuneDeadArea = NewConfig.TemperatureTuneDeadArea;
            this.TemperatureTuneThreshold = NewConfig.TemperatureTuneThreshold;

            strCDKEY = NewConfig.CDKEY;

            this.CDKEY_Expired = NewConfig.CDKEY_Expired;
            this.CDKEY_DaysLeft = NewConfig.CDKEY_DaysLeft;

            this.LastCureDuration = NewConfig.LastCureDuration;
            this.MaxCureTimeAllowed = NewConfig.MaxCureTimeAllowed;

            this.HVGLevel = NewConfig.HVGLevel;

            this.MaxCureBandServieTime = NewConfig.MaxCureBandServieTime;
            this.CompleteVoiceFilePrefix = NewConfig.CompleteVoiceFilePrefix;

        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 将BloodType列表转换为字符串以保存到app.config文件中
        /// </summary>
        /// <param name="BloodType"></param>
        /// <returns></returns>
        string ConvertBloodTypeToString(string[] BloodType)
        {
            StringBuilder bloodtype_string = new StringBuilder();
            for(int i = 0; i < BloodType.Length; i++)
            {
                bloodtype_string.Append(BloodType[i]);
                if (i < bloodtype_string.Length - 1)
                    bloodtype_string.Append(";");
            }

            return bloodtype_string.ToString();
        }

        #endregion

    }
}
