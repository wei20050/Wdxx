using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using CardReading.Core;

namespace ServiceCardRead
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
            InitializeComponent();
            //默认隐藏窗体
            Hide();
            //系统托盘
            _notifyIcon = WpfNotifyIcon.SetSystemTray("读卡服务 运行中 ...", ShowWin_Click, GetList());
            _notifyIcon.Visible = true;
            CbAutoStart.IsChecked = IsAutoStart;
            OnAutoStart();
            var bdip = "127.0.0.1";
            var ips = Common.GetLocalIp();
            CbIp.Items.Add(bdip);
            if (ips != null && ips.Count > 0)
            {
                foreach (var ip in ips)
                {
                    bdip += Environment.NewLine + ip;
                    CbIp.Items.Add(ip);
                }
            }
            if (string.IsNullOrEmpty(ServiceIp))
            {
                //第一次运行显示窗体
                Show();
                CbIp.SelectedIndex = 0;
                ServiceIp = "127.0.0.1";
                TxtContent.Text = string.Format("  初次运行:{0}请进行如下操作:{0}1. 点击读卡器设置配置读卡器{0}2. 选择IP地址点击保存配置{0}", Environment.NewLine);
                BtnIdCardRead.IsEnabled = false;
                BtnSsCardRead.IsEnabled = false;
                return;
            }
            if (string.IsNullOrEmpty(ServicePort))
            {
                ServicePort = "9876";
            }
            CbIp.Text = ServiceIp;
            TxtPort.Text = ServicePort;
            ServiceUrl = string.Format("http://{0}:{1}/CardRead", ServiceIp, ServicePort);
            //不是第一次运行的时候 直接隐藏窗体 显示托盘 开启测试按钮
            BtnIdCardRead.IsEnabled = true;
            BtnSsCardRead.IsEnabled = true;
            try
            {
                var host = Common.CreateServiceHost(typeof(CardRead), ServiceUrl);
                host.Opened += delegate
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        TxtContent.Text = string.Format("服务已开启 ... ... {0}时间:{1}{0}配置IP地址: {2}{0}本机IPV4 IP地址:{0}{3}", Environment.NewLine, DateTime.Now, ServiceIp, bdip);
                    }));

                };
                host.Closed += delegate
                {

                    Dispatcher.Invoke(new Action(() =>
                    {
                        TxtContent.Text = string.Format("服务已关闭 ... ...{0}时间:{1}", Environment.NewLine, DateTime.Now);
                    }));
                };
                Action open = () =>
                {
                    host.Open();
                };
                //这里异步开启本地服务
                open.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                TxtContent.Text = "服务开启异常,请检查服务器配置!" + Environment.NewLine + ex.Message;
            }
        }

        /// <summary>
        /// 服务IP
        /// </summary>
        public static string ServiceIp
        {
            get
            {
                return Ini.Rini("ServiceIp");
            }
            set
            {
                Ini.Wini("ServiceIp", value);
            }
        }

        /// <summary>
        /// 服务端口
        /// </summary>
        public static string ServicePort
        {
            get
            {
                return Ini.Rini("ServicePort");
            }
            set
            {
                Ini.Wini("ServicePort", value);
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
        /// 服务地址
        /// </summary>
        public static string ServiceUrl;


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

        private void BtnIdCardRead_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format("http://{0}:{1}/CardRead/IdCardRead", ServiceIp, ServicePort));
        }
        private void BtnSsCardRead_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(string.Format("http://{0}:{1}/CardRead/SsCardRead", ServiceIp, ServicePort));
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            ServiceIp = CbIp.Text;
            ServicePort = TxtPort.Text;
            Common.Restart();
        }

        private void CbAutoStart_Click(object sender, RoutedEventArgs e)
        {
            IsAutoStart = CbAutoStart.IsChecked != null && (bool)CbAutoStart.IsChecked;
            OnAutoStart();
        }
        /// <summary>
        /// 开机自启
        /// </summary>
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
        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            var form = new CardReaderConfigForm();
            form.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            // 取消关闭窗体
            e.Cancel = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
        }
    }
}
