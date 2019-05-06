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
        }

        /// <summary>
        /// 服务IP
        /// </summary>
        public static string ServiceIp
        {
            get => Ini.Rini(nameof(ServiceIp));
            set => Ini.Wini(nameof(ServiceIp), value);
        }

        /// <summary>
        /// 服务端口
        /// </summary>
        public static string ServicePort
        {
            get => Ini.Rini(nameof(ServicePort));
            set => Ini.Wini(nameof(ServicePort), value);
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
            _notifyIcon = WpfNotifyIcon.SetSystemTray("读卡服务 运行中 ...", ShowWin_Click, GetList());
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
                CbIp.SelectedIndex = 0;
                ServiceIp = "127.0.0.1";
                TxtContent.Text = $@"  初次运行:{Environment.NewLine}请进行如下操作:{Environment.NewLine}1. 点击读卡器设置配置读卡器{Environment.NewLine}2. 选择IP地址点击保存配置";
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
            ServiceUrl = $"http://{ServiceIp}:{ServicePort}/CardRead";
            //不是第一次运行的时候 直接隐藏窗体 显示托盘 开启测试按钮
            BtnIdCardRead.IsEnabled = true;
            BtnSsCardRead.IsEnabled = true;
            Hide();
            _notifyIcon.Visible = true;
            try
            {
                var host = Common.CreateServiceHost(typeof(CardRead), ServiceUrl);
                host.Opened += delegate
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        TxtContent.Text =
                        $@"服务已开启 ... ... {Environment.NewLine}时间:{DateTime.Now}{
                                Environment.NewLine
                            }配置IP地址:   {ServiceIp}{Environment.NewLine}本机IPV4 IP地址:{Environment.NewLine}{bdip}";
                    }));

                };
                host.Closed += delegate
                {

                    Dispatcher.Invoke(new Action(() =>
                    {
                        TxtContent.Text = $@"服务已关闭 ... ...{Environment.NewLine}时间:{DateTime.Now}";
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
                TxtContent.Text = $@"服务开启异常,请检查服务器配置!{Environment.NewLine}{ex.Message}";
            }
        }

        private void BtnIdCardRead_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($"http://{ServiceIp}:{ServicePort}/CardRead/IdCardRead");
        }
        private void BtnSsCardRead_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($"http://{ServiceIp}:{ServicePort}/CardRead/SsCardRead");
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
