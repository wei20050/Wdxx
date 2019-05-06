using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Forms;

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
        public MainWindow()
        {
            Common.Administrator();
            Common.IsStart();
            Administrator();
            InitializeComponent();
        }
        
        /// <summary>
        /// 是否开机自启
        /// </summary>
        public static bool IsAutoStart
        {
            get => Ini.Rini<bool>(nameof(IsAutoStart));
            set => Ini.Wini(nameof(IsAutoStart), value);
        }

        
        /// <summary>
        /// 管理员身份运行程序
        /// 在界面初始化之前调用此方法程序将以管理员权限运行
        /// ClickOnce支持WPF应用程序 WinFrom不支持
        /// </summary>
        public static void Administrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            if (wp.IsInRole(WindowsBuiltInRole.Administrator)) return;
            var exePath = Assembly.GetEntryAssembly().CodeBase;
            Process.Start(new ProcessStartInfo(exePath)
            {
                UseShellExecute = true,
                Verb = "runas"
            });
            Environment.Exit(0);
        }

        #region 系统托盘

        private NotifyIcon _notifyIcon;

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
                        _notifyIcon.Visible = false;
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
            _notifyIcon.Visible = false;
        }

        #endregion

        #endregion
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //系统托盘
            _notifyIcon = WpfNotifyIcon.SetSystemTray("预约下拉 运行中 ...", ShowWin_Click, GetList());
            //开启作业调度
            TimeJob.JobStat();
            DpYy.Text = DateTime.Now.ToString("yyyy/MM/dd");
            OnAutoStart();
            _notifyIcon.Visible = true;
            Hide();
        }

        private void BtnPull_Click(object sender, RoutedEventArgs e)
        {
            var time = Convert.ToDateTime(DpYy.Text).ToString("yyyy-MM-dd");
            var orderInfo = Common.PullOrderService(time);
            TxtContent.Text =   $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 预约下拉 {time} 数据 接口返回: {orderInfo}";
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
    }
}
