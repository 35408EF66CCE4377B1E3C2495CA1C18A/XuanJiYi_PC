using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class CDatabase : IDisposable
    {
        #region Variables
        /// <summary>
        /// 数据库连接超时时间（秒）
        /// </summary>
        const int CONN_TIMEOUT = 10;

        /// <summary>
        /// 数据库命令执行超时时间(秒)
        /// </summary>
        const int COMMAND_TIMEOUT = 10;

        MySqlConnection conn;

        string strLastError;
        #endregion

        #region Constructors

        #endregion

        #region Methods
        /// <summary>
        /// Get the connection string with app setting file
        /// </summary>
        /// <returns></returns>
        string BuildTheConnectionString()
        {
            return BuildTheConnectionString(
                CPublicVariables.Configuration.SqlServerIP, 
                CPublicVariables.Configuration.SqlServerUser, 
                CPublicVariables.Configuration.SqlServerPassword);
        }

        /// <summary>
        /// Get the connection string with specified IP / User / Password
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        string BuildTheConnectionString(string IP, string User, string Password)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.ConnectionTimeout = CONN_TIMEOUT;
            builder.Database = "xuanjiyi";
            builder.Server = IP;
            builder.UserID = User;
            builder.Password = Password;
            builder.CharacterSet = "utf8";
            return builder.ConnectionString;
        }

        /// <summary>
        /// Open the database connection
        /// </summary>
        /// <returns></returns>
        bool Open()
        {
#if DebugWithoutDB
            return false;
#else
            try
            {
                string connstring = BuildTheConnectionString();
                conn = new MySqlConnection(connstring);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                strLastError = string.Format("打开数据库时发生错误，{0}", ex.Message);
                return false;
            }
#endif
        }

        /// <summary>
        /// Open the database connection with specified IP\User\Password
        /// </summary>
        /// <param name="IP">Server IP</param>
        /// <param name="User">Database user</param>
        /// <param name="Password">database password</param>
        /// <returns></returns>
        bool Open(string IP, string User, string Password)
        {
#if DebugWithoutDB
           strLastError = "Debug mode without database!";
            return false;
#else
            try
            {
                string connstring = BuildTheConnectionString(IP, User, Password);
                conn = new MySqlConnection(connstring);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                strLastError = string.Format("打开数据库时发生错误，{0}", ex.Message);
                return false;
            }
#endif
        }

        /// <summary>
        /// Close the database connection
        /// </summary>
        /// <returns></returns>
        bool Close()
        {
            try
            {
                conn.Close();
                conn.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                strLastError = string.Format("关闭数据库时发生错误，{0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Test the database connection
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public bool TestTheConnection(string IP, string User, string Password)
        {
            if (Open(IP, User, Password))
            {
                Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get all sequence names
        /// </summary>
        /// <param name="Names"></param>
        /// <returns></returns>
        public bool GetSequenceNames(out string[] Names)
        {
            List<string> names = new List<string>();
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT DISTINCT `name` FROM `temperature_sequence` WHERE `using`='E' ORDER BY `name` ASC";
                        MySqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                names.Add(dr["name"].ToString());
                            }
                        }
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            Names = names.ToArray();
            return ret;
        }

        /// <summary>
        /// Get the temperature sequence from the database
        /// </summary>
        /// <param name="SequenceName">温度序列名称</param>
        /// <param name="SequenceInstance">获取到的温度序列实例</param>
        /// <returns></returns>
        public bool GetTemperatureSequence(string SequenceName, out CTemperatureSequence SequenceInstance)
        {
            SequenceInstance = new CTemperatureSequence();

            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT `name`, `order`, `temper`, `consume`, `add_time` FROM `temperature_sequence` WHERE `name` = '" + SequenceName + "' AND `using`='E' ORDER BY `order` ASC" ;
                        MySqlDataReader dr = cmd.ExecuteReader();
                        if(dr.HasRows)
                        {
                            while(dr.Read())
                            {
                                CTemperatureSequenceKeyPoint p = new CTemperatureSequenceKeyPoint(Convert.ToDouble(dr["temper"]), Convert.ToInt16(dr["consume"]));
                                SequenceInstance.Add(p);
                            }
                        }

                        if (SequenceInstance.Count == 0)
                            SequenceInstance = null;
                        else
                            SequenceInstance.SequenceName = SequenceName;

                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// Save the temperature sequence
        /// </summary>
        /// <param name="SequenceName"></param>
        /// <param name="SequenceInstance"></param>
        /// <returns></returns>
        public bool SaveTemperatureSequence(string SequenceName, CTemperatureSequence SequenceInstance)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        StringBuilder cmdstr = new StringBuilder();
                        int order = 0;
                        cmdstr.Append("START TRANSACTION;");
                        foreach (CTemperatureSequenceKeyPoint p in SequenceInstance)
                        {
                            cmdstr.Append(
                                string.Format(
                                    "INSERT INTO `temperature_sequence` (`name`, `order`, `temper`, `consume`) VALUE (\"{0}\", {1}, {2}, {3});",
                                    new string[] { SequenceName, order.ToString(), p.TargetTemperature.ToString(), p.HoldTime.ToString() }));
                            order++;
                        }
                        cmdstr.Append("COMMIT;");
                        cmd.Connection = conn;

                        cmd.CommandText = cmdstr.ToString();
                        cmd.ExecuteNonQuery();
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 删除指定的Sequence
        /// </summary>
        /// <param name="SequenceName"></param>
        /// <returns></returns>
        public bool DeleteTemperatureSequence(string SequenceName)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "DELETE FROM `temperature_sequence` WHERE `name`='" + SequenceName + "'";
                        cmd.ExecuteNonQuery();
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 保存系统日志
        /// </summary>
        /// <param name="MessageType"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public bool SaveLog(CPublicVariables.EnumLogMessageType MessageType, string Message)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() {CommandTimeout = COMMAND_TIMEOUT })
            {
                if (Open())
                {
                    cmd.Connection = conn;

                    try
                    {
                        MySqlParameter para;

                        para = new MySqlParameter();
                        para.ParameterName = "type";
                        para.DbType = DbType.Int16;
                        para.Value = (int)MessageType;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "message";
                        para.DbType = DbType.String;
                        para.Value = Message;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "station_name";
                        para.DbType = DbType.String;
                        para.Value = CPublicVariables.Configuration.StationName;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "station_zonecode";
                        para.DbType = DbType.String;
                        para.Value = CPublicVariables.Configuration.ZoneCode;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "username";
                        para.DbType = DbType.String;
                        para.Value = CPublicVariables.Configuration.SystemUserName;
                        cmd.Parameters.Add(para);



                        cmd.CommandText = "insert into system_log (log_date, type, message, station_name, station_zonecode, username) values (getdate(), @type, @message, @station_name, @station_zonecode, @username)";

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        strLastError = string.Format("检查诊断流水号时发生错误，{0}", ex.Message);
                        ret = false;
                    }
                }
                
                
                Close();
                return ret;
            }
        }

        /// <summary>
        /// 生成新的病人编号
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public bool GetNewPatientSN(ref string SN)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() {CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if(Open())
                    {
                        cmd.Connection = conn;


                        MySqlParameter para;

                        para = new MySqlParameter();
                        para.ParameterName = "prefix";
                        para.DbType = DbType.String;
                        para.Value = CPublicVariables.Configuration.ZoneCode;
                        para.Size = 2;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "sn_out";
                        para.DbType = DbType.String;
                        para.Size = 11;
                        para.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "rc";
                        para.DbType = DbType.Int16;
                        para.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(para);

                        cmd.CommandText = "create_patient_sn";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();

                        int i = Convert.ToInt16(cmd.Parameters["rc"].Value);
                        if (i >= 0)
                        {
                            SN = cmd.Parameters["sn_out"].Value.ToString();
                            ret = true;
                        }
                        else if (i == -1)
                        {
                            strLastError = "区域代码错误";
                            ret = false;
                        }
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch(Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 从病人编号池中删除编号
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public bool DeletePatientSNFromPool(string SN)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;


                        MySqlParameter para;
                        para = new MySqlParameter();
                        para.ParameterName = "sn";
                        para.DbType = DbType.String;
                        para.Value = SN;
                        cmd.Parameters.Add(para);

                        cmd.CommandText = "delete from patient_sn_pool where sn = @sn";
                        cmd.ExecuteNonQuery();

                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 生成新的治疗编号
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public  bool GetNewCureSN(ref string SN)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() {CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;

                        MySqlParameter para;

                        para = new MySqlParameter();
                        para.ParameterName = "prefix";
                        para.DbType = DbType.String;
                        para.Value = CPublicVariables.Configuration.ZoneCode;
                        para.Size = 2;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "type";
                        para.DbType = DbType.Int16;
                        para.Value = 8; // 生成的流水号种类，参考Mysql数据库
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "sn_out";
                        para.DbType = DbType.String;
                        para.Size = 14;
                        para.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(para);

                        cmd.CommandText = "create_cure_sn";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.ExecuteScalar();
                        
                        SN = cmd.Parameters["sn_out"].Value.ToString();
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 获取已存在的病人序列号
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public bool GetExistedPatientSNList(ref string[] SN)
        {
            bool ret = false;
            List<string> lstSN = new List<string>();
            using (MySqlCommand cmd = new MySqlCommand() {CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select [sn] + '|' + [name] + '|' + [id_card] as A from patient order by id_card";
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                            lstSN.Add(dr[0].ToString().Trim());
                        dr.Close();

                        if (lstSN.Count == 0)
                            SN = null;
                        else
                            SN = lstSN.ToArray();

                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }


        public bool CheckIDCardExists(string ID_Card, out string SN, out string Name, out bool Exists)
        {
            bool ret = false;
            Exists = false;
            SN = string.Empty;
            Name = string.Empty;

            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select [sn], [name] from patient where id_card = '" + ID_Card + "'";
                        MySqlDataReader dr = cmd.ExecuteReader();
                        if(dr.HasRows)
                        {
                            dr.Read();
                            Exists = true;
                            SN = dr["sn"].ToString();
                            Name = dr["name"].ToString();
                        }
                        else
                        {
                            Exists = false;
                        }

                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 获取治疗历史记录列表
        /// </summary>
        /// <param name="CureHistoryTable"></param>
        /// <returns></returns>
        public bool GetCureHistoryList(out DataTable CureHistoryTable)
        {
            CureHistoryTable = null;
            return GetCureHistoryList("", out CureHistoryTable);
        }

        /// <summary>
        /// 获取治疗历史记录列表
        /// </summary>
        /// <param name="CureHistoryTable"></param>
        /// <returns></returns>
        public bool GetCureHistoryList(string CureSN, out DataTable CureHistoryTable)
        {
            bool ret = false;
            CureHistoryTable = null;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = @"SELECT 
                                                  `cure_sn`,
                                                  `patient_name`,
                                                  `cure_channel` + 1  AS `cure_channel`,
                                                  `sequence_name`,
                                                  `created_time`,
                                                  `updated_time`,
                                                  `seq_snapshot` 
                                                FROM
                                                  `xuanjiyi`.`cure_history` " +
                                                  (CureSN == "" ? 
                                                  "WHERE  DATE_SUB(CURDATE(), INTERVAL 31 DAY) <= date(updated_time) " : 
                                                  string.Format("WHERE `cure_sn` = '{0}' ", CureSN)) +
                                                  "ORDER BY `cure_sn` DESC";
                        DataSet ds = new DataSet();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(ds);
                        CureHistoryTable = ds.Tables[0];
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 获取已存在的病人信息
        /// </summary>
        /// <param name="Dr"></param>
        /// <returns></returns>
        public bool GetExistedPatientInfoBySN(string SN, ref DataRow Dr)
        {
            bool ret = false;
            List<string> lstSN = new List<string>();
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;

                        MySqlParameter para = new MySqlParameter();
                        para.DbType = DbType.String;
                        para.ParameterName  = "sn";
                        para.Value = SN;
                        cmd.Parameters.Add(para);

                        cmd.CommandText = "SELECT [name], [age], case sex when 'M' then '男' when 'F' then '女' end as sex, [id_card], [address], [phone], [blood_type], [station_name], [station_zonecode] FROM [dbo].[patient] where sn = @sn";
                        MySqlDataAdapter da = new MySqlDataAdapter();
                        DataSet ds = new DataSet();
                        da.SelectCommand = cmd;
                        da.Fill(ds);

                        if(ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            Dr = ds.Tables[0].Rows[0];
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 保存新的治疗信息
        /// </summary>
        /// <param name="CureSN"></param>
        /// <param name="PatitentSN"></param>
        /// <param name="CureDuration"></param>
        /// <param name="TargetTemperature"></param>
        /// <param name="StationName"></param>
        /// <param name="StationZoneCode">机台所在的区域代码</param>
        /// <returns></returns>
        public bool InsertNewCureDetail(string CureSN, double Temperature, int Second)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;

                        MySqlParameter para = new MySqlParameter();
                        para.ParameterName = "sn";
                        para.DbType = DbType.String;
                        para.Value = CureSN;
                        cmd.Parameters.Add(para);
                        

                        para = new MySqlParameter();
                        para.ParameterName = "temperature";
                        para.DbType = DbType.Double;
                        para.Value = Temperature;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "second_order";
                        para.DbType = DbType.Int32;
                        para.Value = Second;
                        cmd.Parameters.Add(para);

                        cmd.CommandText = "create_one_detail_record";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// Save the basic info of a curing progress
        /// </summary>
        /// <param name="CureSN"></param>
        /// <param name="PatientName"></param>
        /// <param name="Sequence"></param>
        /// <returns></returns>
        public bool CreateCureHistory(string CureSN, string PatientName, int Channel, CTemperatureSequence Sequence)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;

                        MySqlParameter para = new MySqlParameter();
                        para.ParameterName = "cure_sn";
                        para.DbType = DbType.String;
                        para.Value = CureSN;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "patient_name";
                        para.DbType = DbType.String;
                        para.Value = PatientName;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "cure_channel";
                        para.DbType = DbType.Int16;
                        para.Value = Channel;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "sequence_name";
                        para.DbType = DbType.String;
                        para.Value = Sequence.SequenceName;
                        cmd.Parameters.Add(para);
            
                        para = new MySqlParameter();
                        para.ParameterName = "updated_time";
                        para.DbType = DbType.DateTime;
                        para.Value = DateTime.MinValue;
                        cmd.Parameters.Add(para);

                        para = new MySqlParameter();
                        para.ParameterName = "seq_snapshot";
                        para.DbType = DbType.Binary;
                        para.Value = CPublicMethods.SerializeObjectToBinaryArray(Sequence);
                        cmd.Parameters.Add(para);

                        cmd.CommandText =
                            @"
                                INSERT INTO `xuanjiyi`.`cure_history` (
                                  `cure_sn`,
                                  `patient_name`,
                                  `cure_channel`,
                                  `sequence_name`,
                                  `created_time`,
                                  `updated_time`,
                                  `seq_snapshot`
                                ) 
                                VALUES
                                  (
                                    @cure_sn,
                                    @patient_name,
                                    @cure_channel,
                                    @sequence_name,
                                    NOW(),
                                    @updated_time,
                                    @seq_snapshot
                                  );commit;";

                        if (cmd.ExecuteNonQuery() > 0)
                            ret = true;
                        else
                            ret = false;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 保存治疗完成时间
        /// </summary>
        /// <param name="CureSN"></param>
        /// <returns></returns>
        public bool SaveCureStopTime(string CureSN)
        {
            bool ret = false;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;

                        MySqlParameter para = new MySqlParameter();
                        para.ParameterName = "cure_sn";
                        para.DbType = DbType.String;
                        para.Value = CureSN;
                        cmd.Parameters.Add(para);

                        cmd.CommandText = "update [cure_history] set [cure_stop_date] = getdate(), [cure_duration_actual] = datediff(minute, cure_start_date, getdate()) where cure_sn = @cure_sn";
                        if (cmd.ExecuteNonQuery() > 0)
                            ret = true;
                        else
                            ret = false;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 返回特定用户的治疗历史记录
        /// </summary>
        /// <param name="PatientSN"></param>
        /// <param name="HistoryTable"></param>
        /// <returns></returns>
        public bool GetHistoryByCureSN(string CureSN, out DataTable HistoryTable)
        {
            bool ret = false;
            HistoryTable = null;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText =
                            @"SELECT 
                              `cure_sn`,
                              `second_order`,
                              `temperature`,
                              `add_time` 
                            FROM
                              `xuanjiyi`.`detail` 
                             WHERE
                              `xuanjiyi`.`detail`.`cure_sn` = '" + CureSN + "' ORDER BY `second_order`";
        
                        DataSet ds = new DataSet();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(ds);
                        HistoryTable = ds.Tables[0];
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 获取设备使用时长
        /// </summary>
        /// <param name="Duration"></param>
        /// <returns></returns>
        public bool GetRunDuration(out Int64 Duration)
        {
            bool ret = false;
            Duration = -1;

            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;

                        MySqlParameter para = new MySqlParameter();

                        para = new MySqlParameter();
                        para.ParameterName = "total_duration";
                        para.DbType = DbType.Int64;
                        para.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(para);

                        cmd.CommandText = "ReturnSystemRunTime";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();

                        Duration = Convert.ToInt32(cmd.Parameters["total_duration"].Value);

                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        /// <summary>
        /// 返回系统日志
        /// </summary>
        /// <param name="LogTable"></param>
        /// <returns></returns>
        public bool GetSystemLog(out DataTable LogTable)
        {
            bool ret = false;
            LogTable = null;
            using (MySqlCommand cmd = new MySqlCommand() { CommandTimeout = COMMAND_TIMEOUT })
            {
                try
                {
                    if (Open())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select top 5000 * from [view_log] order by [时间] DESC";
                        DataSet ds = new DataSet();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(ds);
                        LogTable = ds.Tables[0];
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                catch (Exception ex)
                {
                    strLastError = ex.Message;
                    ret = false;
                }
            }

            Close();
            return ret;
        }

        public void Dispose()
        {
            conn = null;
        }
        #endregion

        #region Properties
        public string LastError
        {
            get
            {
                return strLastError;
            }
        }
        #endregion
    }
}
