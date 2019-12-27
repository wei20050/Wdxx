using CardReading.Core;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CardReading.ServiceHost
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// 服务端口
        /// </summary>
        public static string ServicePort
        {
            get { return Ini.Rini("ServicePort"); }
            set { Ini.Wini("ServicePort", value); }
        }

        /// <summary>
        /// 是否开机自启
        /// </summary>
        public static bool IsAutoStart
        {
            get { return Ini.Rini("IsAutoStart") == "true"; }

            set { Ini.Wini("IsAutoStart", value ? "true" : "false"); }
        }

        /// <summary>
        /// 服务地址
        /// </summary>
        private static string _serviceUrl;

        public FormMain()
        {
            Common.Administrator();
            Common.IsStart();
            if (string.IsNullOrEmpty(Settings.CardReaderType))
            {
                //默认给华旭金卡读卡器
                Settings.CardReaderType = "CardReading.IdCardReaderCommon.CardReadingCommon";
            }
            InitializeComponent();
            //图标显示在托盘区
            notifyIcon1.Visible = true;
            checkBoxEx1.Checked = IsAutoStart;
            OnAutoStart();
            //ip组
            var ips = Common.GetLocalIp();
            //本地ip
            var bdip = string.Empty;
            foreach (var ip in ips)
            {
                bdip += Environment.NewLine + ip;
            }
            if (string.IsNullOrEmpty(ServicePort))
            {
                //第一次运行显示窗体
                Show();
                richTextBox1.Text = string.Format("  初次运行:{0}请进行如下操作:{0}1. 点击读卡器设置配置读卡器{0}2. 配置端口号 3. 点击保存配置{0}", Environment.NewLine);
                button1.Enabled = false;
                return;
            }
            if (string.IsNullOrEmpty(ServicePort))
            {
                ServicePort = "9876";
            }
            textBox1.Text = ServicePort;
            _serviceUrl = string.Format("http://127.0.0.1:{0}", ServicePort);
            //不是第一次运行的时候 开启测试按钮
            button1.Enabled = true;
            try
            {
                var hh = new CoreClientHost(typeof(CardRead), Convert.ToInt32(ServicePort));
                richTextBox1.Text = string.Format("服务已开启 ... ... {0}时间:{1}{0} 本机IPV4 IP地址:{2}", Environment.NewLine, DateTime.Now, bdip);
                hh.Open();
            }
            catch (Exception ex)
            {
                richTextBox1.Text = @"服务开启异常,请检查服务器配置!" + Environment.NewLine + ex.Message;
            }
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
        //身份证读卡测试
        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(_serviceUrl + "/IdCardRead");
        }
        //读卡器设置
        private void button2_Click(object sender, EventArgs e)
        {
            new CardReadSetting().ShowDialog();
        }
        //保存
        private void button3_Click(object sender, EventArgs e)
        {
            ServicePort = textBox1.Text;
            Common.Restart();
        }

        private void 关闭程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Environment.Exit(0);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
            }
        }

        private void checkBoxEx1_CheckedChanged(object sender, EventArgs e)
        {
            IsAutoStart = checkBoxEx1.Checked;
            OnAutoStart();
        }
    }
}
