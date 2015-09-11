using DevExpress.Xpf.Core;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tai_Shi_Xuan_Ji_Yi.Classes;
using Tai_Shi_Xuan_Ji_Yi.Classes.StepAreaAndLineChart.PresetSequence;
using Tai_Shi_Xuan_Ji_Yi.Controls;
using Tai_Shi_Xuan_Ji_Yi.Converters;

namespace Tai_Shi_Xuan_Ji_Yi.Windows
{
    /// <summary>
    /// TemperatureSequenceEditor.xaml 的交互逻辑
    /// </summary>
    public partial class TemperatureSequenceEditor : DXWindow
    {
        #region Variables
        /// <summary>
        /// 窗口打开模式
        /// </summary>
        public enum ENUM_Window_Mode
        {
            /// <summary>
            /// 编辑模式
            /// </summary>
            Edit,

            /// <summary>
            /// 加载模式
            /// </summary>
            Load
        }

        ViewModel_TemperatureSequenceEditor _vm;
        BackgroundWorker bgw_Init;
        string[] seq_names;
        #endregion

        #region Constructions
        public TemperatureSequenceEditor(ENUM_Window_Mode Mode)
        {
            InitializeComponent();

            _vm = new ViewModel_TemperatureSequenceEditor();

            /* 加载Sequence Names */
            bgw_Init = new BackgroundWorker();
            bgw_Init.WorkerReportsProgress = true;
            bgw_Init.DoWork += bgw_Init_DoWork;
            bgw_Init.ProgressChanged += bgw_Init_ProgressChanged;
            bgw_Init.RunWorkerAsync();

            /* 根据模式设置界面 */
            if (Mode == ENUM_Window_Mode.Load)
            {
                btn_New.Visibility = Visibility.Hidden;
                btn_Save.Visibility = System.Windows.Visibility.Hidden;
                btn_Delete.Visibility = System.Windows.Visibility.Hidden;
                btn_Load.Visibility = System.Windows.Visibility.Visible;
                grid_SequenceEdit.IsEnabled = false;
            }
            else
            {
                btn_New.Visibility = Visibility.Visible;
                btn_Save.Visibility = System.Windows.Visibility.Visible;
                btn_Delete.Visibility = System.Windows.Visibility.Visible;
                btn_Load.Visibility = System.Windows.Visibility.Hidden;
                grid_SequenceEdit.IsEnabled = true;
            }

        }
        #endregion

        #region Properties
        /// <summary>
        /// 返回选定的Sequence名称
        /// </summary>
        public string SelectedSequenceName
        {
            get
            {
                return _vm.SequenceName;
            }
        }

        /// <summary>
        /// 返回选定的Sequence
        /// </summary>
        public CTemperatureSequence SelectedSequece
        {
            get
            {
                return _vm.Sequence;
            }
        }
        #endregion

        #region Events

        void bgw_Init_DoWork(object sender, DoWorkEventArgs e)
        {
            /* 如果参数中传入了字符串，说明加载完Sequence Names以后切换到字符串指定的Sequence */
            string seq_name_to_display;
            if (e.Argument != null)
                seq_name_to_display = e.Argument.ToString();
            else
                seq_name_to_display = "";

            /* 清空列表，启动Busy进度条 */
            bgw_Init.ReportProgress(1);

            /* 加载Sequence Names */
            using (CDatabase db = new CDatabase())
            {
                if (db.GetSequenceNames(out seq_names))
                {
                    bgw_Init.ReportProgress(100, seq_name_to_display);
                }
                else
                {
                    bgw_Init.ReportProgress(0, db.LastError);
                }
            }
        }

        void bgw_Init_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch(e.ProgressPercentage)
            {
                    /* 加载Sequence Names发生错误 */
                case 0:
                    MessageBox.Show("加载温度预设名称时发生错误\r\n" + e.UserState.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                /* 启动Busy进度条 */
                case 1:
                   // pb_Init.IsIndeterminate = true;
                    cbo_SequenceNames.ItemsSource = null;
                    break;

                    /* Sequence Names 显示在ComboBox里 */
                case 100:
                    cbo_SequenceNames.ItemsSource = seq_names;
                    string seq_name_to_display = e.UserState.ToString();
                    if (seq_name_to_display != "")
                    {
                        cbo_SequenceNames.SelectedItem = seq_name_to_display;
                    }
                    else
                    {
                        _vm.Sequence = new CTemperatureSequence();
                        _vm.SequenceName = "";
                        grid_SequenceEdit.ItemsSource = _vm.Sequence;
                        chart1.PresettedSequence = _vm.Sequence;

                        Binding b = new Binding("TotalTime");
                        b.Source = _vm.Sequence;
                        b.Converter = new CConverterPresetTotalTimeToComment();
                        txt_TotalCureTime.SetBinding(TextBlock.TextProperty, b);

                        b = new Binding("TotalTime");
                        b.Source = _vm.Sequence;
                        b.Converter = new CConverterPresetTotalTimeToColor();
                        txt_TotalCureTime.SetBinding(TextBlock.ForegroundProperty, b);

                    }
                    break;
            }

            /* 停止Busy进度条 */
            //pb_Init.IsIndeterminate = false;
        }
        
        private void view2_InitNewRow(object sender, DevExpress.Xpf.Grid.InitNewRowEventArgs e)
        {
            // Set the defalut value to the cell
            grid_SequenceEdit.SetCellValue(e.RowHandle, colHoldTime, 10); // default time: 10 minutes
            grid_SequenceEdit.SetCellValue(e.RowHandle, colTargetTemperature, 40); // default temperature: 40 degree
        }

        private void view2_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            // Update the start time according to the setted values
            _vm.Sequence.UpdateStartTime();
            chart1.PresettedSequence = null;
            chart1.PresettedSequence = _vm.Sequence;
        }

        private void GridDragDropManager_Dropped(object sender, DevExpress.Xpf.Grid.DragDrop.GridDroppedEventArgs e)
        {
            chart1.PresettedSequence = null;
            chart1.PresettedSequence = _vm.Sequence;
        }

        private void cbo_SequenceNames_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox cbo = sender as ComboBox;
            if (cbo.SelectedItem != null)
            {
                _vm.LoadSequence(cbo.SelectedItem.ToString());
                grid_SequenceEdit.ItemsSource = _vm.Sequence;
                chart1.PresettedSequence = _vm.Sequence;

                Binding b = new Binding("TotalTime");
                b.Source = _vm.Sequence;
                b.Converter = new CConverterPresetTotalTimeToComment();
                txt_TotalCureTime.SetBinding(TextBlock.TextProperty, b);

                b = new Binding("TotalTime");
                b.Source = _vm.Sequence;
                b.Converter = new CConverterPresetTotalTimeToColor();
                txt_TotalCureTime.SetBinding(TextBlock.ForegroundProperty, b);
            }
        }

        
        /// <summary>
        /// 保存温度序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            /* 如果Sequence为空，即还没有编辑Sequence，则提示并且不保存 */
            if(_vm.Sequence.Count == 0)
            {
                MessageBox.Show("当前温度序列为空", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            /* 检查总时间是否超过60分钟 */
            if(_vm.Sequence.TotalTime > CPublicVariables.Configuration.MaxCureTimeAllowed)
            {
                MessageBox.Show("总治疗时间不能超过60分钟", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SequenceNameInput f = new SequenceNameInput();
            bool? r = f.ShowDialog();
            if(r.HasValue && r.Value == true)
            {
                /* 保存Sequence到数据库 */
                using(CDatabase db = new CDatabase())
                {
                    /* 检查名称是否已经存在 */
                    CTemperatureSequence seq;
                    r = db.GetTemperatureSequence(f.SequenceName, out seq);
                    if(r == false)  // 查询时发生错误
                    {
                        MessageBox.Show("查询温度序列时发生错误\r\n" + db.LastError, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if(seq != null && seq.Count > 0)   // 如果当前Sequence name已存在，询问用户是否覆盖
                        {
                            // 如果不覆盖，则退出保存过程
                            if(MessageBox.Show(string.Format("{0}已存在，是否覆盖？", f.SequenceName), "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                            {
                                return;
                            }
                                // 如果覆盖，则先删除旧的Sequence，再保存
                            else
                            {
                                /* 删除Sequence */
                                r = db.DeleteTemperatureSequence(f.SequenceName);
                                if(r == false)
                                {
                                    MessageBox.Show("删除温度序列时发生错误\r\n" + db.LastError, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }
                        }

                        /* 保存Sequence */
                        r = db.SaveTemperatureSequence(f.SequenceName, _vm.Sequence);
                        if (r == false)  // 如果发生错误
                        {
                            MessageBox.Show("保存温度序列时发生错误\r\n" + db.LastError, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            /* 重新加载Sequence Names列表 */
                            bgw_Init.RunWorkerAsync(f.SequenceName);
                        }
                    }

                   
                }
            }
            else
            {
                // Do Nothing
            }
        }
        
        private void btn_New_Click(object sender, RoutedEventArgs e)
        {
            if(_vm.Sequence.Count > 0)
            {
                if(MessageBox.Show("是否清除当前正在编辑的温度序列？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    cbo_SequenceNames.SelectedIndex = -1;
                    _vm.Sequence = new CTemperatureSequence();
                    _vm.SequenceName = "";
                    grid_SequenceEdit.ItemsSource = _vm.Sequence;
                    chart1.PresettedSequence = _vm.Sequence;

                    Binding b = new Binding("TotalTime");
                    b.Source = _vm.Sequence;
                    b.Converter = new CConverterPresetTotalTimeToComment();
                    txt_TotalCureTime.SetBinding(TextBlock.TextProperty, b);

                    b = new Binding("TotalTime");
                    b.Source = _vm.Sequence;
                    b.Converter = new CConverterPresetTotalTimeToColor();
                    txt_TotalCureTime.SetBinding(TextBlock.ForegroundProperty, b);
                }
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if(_vm.Sequence.Count > 0)
            {
                if (MessageBox.Show(string.Format("是否删除温度序列{0}？", _vm.SequenceName), "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using(CDatabase db = new CDatabase())
                    {
                        bool r = db.DeleteTemperatureSequence(_vm.SequenceName);
                        if(r == false)  // 删除失败
                        {
                            MessageBox.Show("删除温度序列时发生错误\r\n" + db.LastError, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else            // 删除成功
                        {
                            MessageBox.Show("删除成功", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                            bgw_Init.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        
 
        private void btn_Load_Click(object sender, RoutedEventArgs e)
        {
            /* 如果Sequence为空，即还没有编辑Sequence，则提示并且不保存 */
            if (_vm.Sequence.Count == 0)
            {
                MessageBox.Show("请先选择一个温度序列", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                this.DialogResult = true;   
            }
        }

        #endregion

    }
}
