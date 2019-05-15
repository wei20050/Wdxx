using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Order
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        private readonly DispatcherTimer _timer = new DispatcherTimer();
        public MainWindow()
        {
            Common.Administrator();
            Common.IsStart();
            InitializeComponent();
            Hide();
            //系统托盘
            _notifyIcon = WpfNotifyIcon.SetSystemTray("预约下拉 运行中 ...", ShowWin_Click, GetList());
            _notifyIcon.Visible = true;
            if (string.IsNullOrEmpty(Common.ServiceUri))
            {
                Common.ServiceUri = "http://localhost/queue/QueueService.svc";
            }
            if (Xlsj == 0)
            {
                Xlsj = 3;
            }
            TxtFwqdz.Text = Common.ServiceUri;
            TxtSj.Text = Xlsj.ToString();
            //开启定时器
            _timer.Tick += TimerCall;
            _timer.Interval = TimeSpan.FromSeconds(600);
            _timer.Start();
            OnAutoStart();
        }

        private bool _isPull;

        private void TimerCall(object sender, EventArgs e)
        {
            if (DateTime.Now.Hour == Xlsj)
            {
                if (_isPull) return;
                var ret = Common.PullOrderService(string.Empty);
                if (ret == string.Empty)
                {
                    _isPull = true;
                }
            }
            else
            {
                _isPull = false;
            }
        }

        /// <summary>
        /// 是否开机自启
        /// </summary>
        public static bool IsAutoStart
        {
            get
            {
                return Ini.Rini<bool>("IsAutoStart");
            }
            set
            {
                Ini.Wini("IsAutoStart", value);
            }
        }

        /// <summary>
        /// 下拉时间
        /// </summary>
        public static int Xlsj
        {
            get
            {
                return Ini.Rini<int>("Xlsj");
            }
            set
            {
                Ini.Wini("Xlsj", value);
            }
        }

        #region 系统托盘

        private readonly NotifyIcon _notifyIcon;

        #region 托盘右键菜单

        //托盘右键菜单集合
        private List<SystemTrayMenu> GetList()
        {
            var ls = new List<SystemTrayMenu>
            {
                new SystemTrayMenu
                {
                    Txt = "打开",
                    Click = (sender, e) =>
                    {
                        Show();
                    }
                },
                new SystemTrayMenu
                {
                    Txt = "退出",
                    Click = (sender, e) =>
                    {
                        _notifyIcon.Visible = false;
                        Environment.Exit(0);
                    }
                }
            };
            return ls;
        }

        //打开主面板
        private void ShowWin_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button != MouseButtons.Left) return;
            Show();
        }

        #endregion

        #endregion

        private void BtnPull_Click(object sender, RoutedEventArgs e)
        {
            var time = Convert.ToDateTime(DpYy.Text).ToString("yyyy-MM-dd");
            var orderInfo = Common.PullOrderService(time);
            TxtContent.Text =   string.Format("{0} 预约下拉 {1} 数据 接口返回: {2}", DateTime.Now.ToString(CultureInfo.InvariantCulture), time, orderInfo);
        }

        private void CbAutoStart_Click(object sender, RoutedEventArgs e)
        {
            IsAutoStart = CbAutoStart.IsChecked != null && (bool)CbAutoStart.IsChecked;
            OnAutoStart();
        }

        private static void OnAutoStart()
        {
            if (IsAutoStart)
            {
                Common.AutoStart();
            }
            else
            {
                Common.UnAutoStart();
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _notifyIcon.Visible = true;
            Hide();
            // 取消关闭窗体
            e.Cancel = true;
        }

        //窗体已经关闭后的操作
        private void Window_Closed(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            DpYy.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Common.ServiceUri = TxtFwqdz.Text;
            Xlsj = Convert.ToInt32(TxtSj.Text);
            System.Windows.MessageBox.Show("配置保存成功!");
        }
    }
}
