
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Windows;
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
            _realtime_temper_collection = new CRealtimeTemperatureCollection();
            _history_collection = new ObservableCollection<CCureHistory>();
        }

        public void LoadHistorySummary()
        {
            LoadHistorySummary("", 0);
        }

        public void LoadHistorySummary(int Limit)
        {
            LoadHistorySummary("", Limit);
        }

        public void LoadHistorySummary(string CureSN, int Limit)
        {
            int item_cnt = 0;

            _realtime_temper_collection.Clear();
            _history_collection.Clear();

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
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                _history_collection.Add(
                                    new CCureHistory(
                                        dr["cure_sn"].ToString(),
                                        dr["patient_name"].ToString(),
                                        Convert.ToInt16(dr["cure_channel"]),
                                        Convert.ToDateTime(dr["created_time"]),
                                        Convert.ToDateTime(dr["updated_time"]),
                                        (byte[])dr["seq_snapshot"]));
                            }));

                            item_cnt++;
                            if(Limit > 0)
                            {
                                if (item_cnt > Limit)
                                    break;
                            }
                        }
                    }
                }
            }).Start();
        }

        #region Properties
        public ObservableCollection<CCureHistory> HistoryCollection
        {
            get
            {
                return _history_collection;
            }
        }

        /// <summary>
        /// 返回实时温度历史列表
        /// </summary>
        public CRealtimeTemperatureCollection RealtimeTemperCollection
        {
            get
            {
                return _realtime_temper_collection;
            }
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
                    _realtime_temper_collection.Clear();

                    string cure_sn = history.CureSN;
                    DataTable dt = null;
                    new Thread(() =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => 
                        {
                            // 设置鼠标光标到等待模式
                            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.AppStarting;
                        }));

                        using (CDatabase db = new CDatabase())
                        {
                            // 获取实时温度历史列表
                            if (!db.GetHistoryByCureSN(cure_sn, out dt))
                            {
                                // 如果错误，通知View弹出错误对话框
                                Messenger.Default.Send<NotificationMessage<string>>(new NotificationMessage<string>(db.LastError, "获取历史温度曲线时发生错误"), "DBError");
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

                                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        _realtime_temper_collection.Add(new CRealtimeTemperaturePoint(Convert.ToInt16(dr["second_order"]), Convert.ToDouble(dr["temperature"])));

                                        LoadRTCollectionProgress++;
                                        RaisePropertyChanged("LoadRTCollectionProgress");

                                    }));
                                }

                                LoadRTCollectionProgress = 0;
                                RaisePropertyChanged("LoadRTCollectionProgress");

                                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    // Set the cursor to the default style
                                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                                }));

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
                    LoadHistorySummary(sn, 0);
                });
            }
        }
        #endregion 

    }
}
