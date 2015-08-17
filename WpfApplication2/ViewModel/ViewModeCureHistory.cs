
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using Tai_Shi_Xuan_Ji_Yi.Classes.HistoryShow;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.Realtime;

namespace Tai_Shi_Xuan_Ji_Yi.ViewModel
{
    public class ViewModeCureHistory : ViewModelBase
    {
        ObservableCollection<CCureHistory> _history_collection = null;
        CRealtimeTemperatureCollection _realtime_temper_collection;

        public ViewModeCureHistory()
        {
        }

        public void LoadHistorySummary()
        {
            LoadHistorySummary("");
        }

        public void LoadHistorySummary(string CureSN)
        {
            this.RealtimeTemperCollection = null;
            _realtime_temper_collection = null;
            RaisePropertyChanged("RealtimeTemperCollection");


            _history_collection = new ObservableCollection<CCureHistory>();

            new Thread(() =>
            {
                DataTable dt = null;
                // 加载History列表
                using (CDatabase db = new CDatabase())
                {
                    if (!db.GetCureHistoryList(CureSN, out dt))
                    {
                        // 如果错误，通知View弹出错误对话框
                        Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>(db.LastError, "获取历史列表时发生错误"), "DBError");
                    }
                    else
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            _history_collection.Add(new CCureHistory(
                                dr["cure_sn"].ToString(),
                                dr["patient_name"].ToString(),
                                Convert.ToInt16(dr["cure_channel"]),
                                Convert.ToDateTime(dr["created_time"]),
                                Convert.ToDateTime(dr["updated_time"]),
                                (byte[])dr["seq_snapshot"]));
                        }
                        
                            this.HistoryCollection = _history_collection;
                            RaisePropertyChanged("HistoryCollection");
                    }
                }
            }).Start();
        }

        #region Properties
        public ObservableCollection<CCureHistory> HistoryCollection
        {
            private set;
            get;
        }

        /// <summary>
        /// 返回实时温度历史列表
        /// </summary>
        public CRealtimeTemperatureCollection RealtimeTemperCollection
        {
            private set;
            get;
        }

        /// <summary>
        /// 返回加载实时温度历史列表的进度
        /// </summary>
        public int LoadRTCollectionProgress
        {
            private set;
            get;
        }

        /// <summary>
        /// 返回实时温度历史列表的最大条目数
        /// </summary>
        public int MaxRTCollectionItems
        {
            private set;
            get;
        }
        #endregion

        #region ICommands

        public RelayCommand<CCureHistory> CureItemSelected
        {
            get
            {
                return new RelayCommand<CCureHistory>(history => 
                {
                    if (history == null)
                        return;

                    // 清空上次查询的结果
                    RealtimeTemperCollection = null;
                    _realtime_temper_collection = new CRealtimeTemperatureCollection();
                    RaisePropertyChanged("RealtimeTemperCollection");

                    string cure_sn = history.CureSN;
                    DataTable dt = null;
                    new Thread(() =>
                    {
                        using (CDatabase db = new CDatabase())
                        {
                            // 获取实时温度历史列表
                            if (!db.GetHistoryByCureSN(cure_sn, out dt))
                            {
                                // 如果错误，通知View弹出错误对话框
                                Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>( db.LastError, "获取历史温度曲线时发生错误"), "DBError");
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    LoadRTCollectionProgress = 0;
                                    MaxRTCollectionItems = dt.Rows.Count;
                                    RaisePropertyChanged("LoadRTCollectionProgress");
                                    RaisePropertyChanged("MaxRTCollectionItems");
                                }));


                                // 根据历史列表生成CRealtimeTemperatureCollection对象
                                foreach (DataRow dr in dt.Rows)
                                {
                                    _realtime_temper_collection.Add(new CRealtimeTemperaturePoint(Convert.ToInt16(dr["second_order"]), Convert.ToDouble(dr["temperature"])));

                                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        LoadRTCollectionProgress++;
                                        RaisePropertyChanged("LoadRTCollectionProgress");
                                    }));

                                }

                                RealtimeTemperCollection = _realtime_temper_collection;
                                RaisePropertyChanged("RealtimeTemperCollection");

                                LoadRTCollectionProgress = 0;
                                //RaisePropertyChanged("LoadRTCollectionProgress");
                            }
                        }
                    }).Start();
                    
                });
            }
        }

        public RelayCommand<string> SearchCureSN
        {
            get
            {
                return new RelayCommand<string>(sn => 
                {
                    LoadHistorySummary(sn);
                });
            }
        }
        #endregion 

    }
}
