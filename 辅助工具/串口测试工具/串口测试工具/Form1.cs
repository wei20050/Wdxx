using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace 串口测试工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //串口操作对象
        private readonly SerialPortEx _comDevice = new SerialPortEx();
        
        private static Parity GetParity(int parity)
        {
            switch (parity)
            {
                case 0:
                    return Parity.Even;
                case 1:
                    return Parity.Mark;
                case 2:
                    return Parity.None;
                case 3:
                    return Parity.Odd;
                case 4:
                    return Parity.Space;
                default:
                    return Parity.None;
            }
        }
        private static StopBits GetStopBits(int stopBits)
        {
            switch (stopBits)
            {
                case 0:
                    return StopBits.One;
                case 1:
                    return StopBits.OnePointFive;
                case 2:
                    return StopBits.Two;
                default:
                    return StopBits.One;
            }
        }
        //打开端口
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalVar.ConfigStr = string.Format("{0},{1},{2},{3},{4}", comboBox2.SelectedIndex, comboBox3.SelectedIndex, comboBox4.SelectedIndex, comboBox5.SelectedIndex, comboBox6.SelectedIndex);
                GlobalVar.Com = comboBox1.Text;
                _comDevice.PortName = comboBox1.Text;
                _comDevice.BaudRate = Convert.ToInt32(comboBox2.Text);
                _comDevice.Parity = GetParity(comboBox3.SelectedIndex);
                _comDevice.DataBits = Convert.ToInt32(comboBox4.Text);
                _comDevice.StopBits = GetStopBits(comboBox5.SelectedIndex);
                _comDevice.RtsEnable = true;
                _comDevice.Open();
                comboBox1.Enabled = comboBox2.Enabled = comboBox3.Enabled =
                    comboBox4.Enabled = comboBox5.Enabled = comboBox6.Enabled = false;
                richTextBox1.Text += @"打开串口成功!" + Environment.NewLine;
                timer1.Enabled = false;
            }
            catch (Exception exception)
            {
                richTextBox1.Text += @"打开串口失败!  " + exception + Environment.NewLine;
            }
        }
        //关闭端口
        private void button2_Click(object sender, EventArgs e)
        {
            _comDevice.Close();
            comboBox1.Enabled = comboBox2.Enabled = comboBox3.Enabled =
                comboBox4.Enabled = comboBox5.Enabled = comboBox6.Enabled = true;
            richTextBox1.Text += @"关闭串口成功!" + Environment.NewLine;
            timer1.Enabled = true;
        }
        //发送文本
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalVar.StrText = textBox1.Text;
                _comDevice.Write(textBox1.Text);
                richTextBox1.Text += @"发送文本成功:  文本:" + textBox1.Text + Environment.NewLine;
            }
            catch (Exception exception)
            {
                richTextBox1.Text += @"发送文本失败:  文本:" + textBox1.Text + @" 错误:" + exception + Environment.NewLine;
            }
        }
        //发送字节
        private void button4_Click(object sender, EventArgs e)
        {
            GlobalVar.ByteText = textBox2.Text;
            Fszj(textBox2.Text.Trim());
        }
        //发送字节
        private void Fszj(string zj)
        {
            try
            {
                var strs = zj.Split(' ');
                var bs = new byte[strs.Length];
                for (var i = 0; i < strs.Length; i++)
                {
                    bs[i] = (byte)Convert.ToInt32(strs[i]);
                }
                _comDevice.Write(bs, 0, bs.Length);
                richTextBox1.Text += @"发送字节数组数据成功:  字节:" + zj + Environment.NewLine;
            }
            catch (Exception exception)
            {
                richTextBox1.Text += @"发送字节数组数据失败:  字节:" + zj + @" 错误:" + exception + Environment.NewLine;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _comDevice.DataReceivedEx += Com_DataReceived;
            ShuaXinJm();
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Tick += (o, args) =>
            {
                Invoke(new Action(() =>
                {
                    var coms = SerialPort.GetPortNames().ToList();
                    comboBox1.DataSource = coms;
                    if (coms.Contains(GlobalVar.Com))
                    {
                        comboBox1.Text = GlobalVar.Com;
                    }
                }));
            };
            if (GlobalVar.Zdy == null)
            {
                GlobalVar.Zdy = new StringCollection();
            }
            else
            {
                ShuaXinZdy();
            }
        }

        private void Com_DataReceived(byte[] b)
        {
            var tmp = b.Aggregate(string.Empty, (current, d) => current + (d + " "));
            var ret = Encoding.Default.GetString(b);
            Invoke(new Action(() =>
            {
                richTextBox1.Text += @"收到数据=> " +
                                     Environment.NewLine +
                                     @"    字节:" +
                                     tmp +
                                     Environment.NewLine +
                                     @"    文本:" + ret +
                                     Environment.NewLine + Environment.NewLine;
            }));
        }
        //生成C#代码
        private void button5_Click(object sender, EventArgs e)
        {
            var str = Properties.Resources.SerialPortHelp;
            if (string.IsNullOrEmpty(str)) return;
            str = str.Replace("[duankou]", comboBox1.Text);
            str = str.Replace("[botelv]", comboBox2.Text);
            str = str.Replace("[xiaoyan]", "Parity." + GetParity(comboBox3.SelectedIndex));
            str = str.Replace("[shuju]", comboBox4.Text);
            str = str.Replace("[tingzhi]", "StopBits." + GetStopBits(comboBox5.SelectedIndex));
            str = str.Replace("[wenben]", textBox1.Text);
            str = str.Replace("[zijie]", textBox2.Text);
            var zdytmp = GlobalVar.Zdy.Cast<string>().Aggregate(string.Empty, (current, z) => current + z + ',');
            zdytmp = zdytmp.TrimEnd(',');
            str = str.Replace("[zdy]", zdytmp);
            File.WriteAllText("SerialPortHelp.cs",str);
            MessageBox.Show(@"代码生成成功！", @"生成代码");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _comDevice.Close();
        }
        //打开串口监控软件
        private void button6_Click(object sender, EventArgs e)
        {
            if (!File.Exists(@"AccessPort.exe"))
            {
                var accessPort = Properties.Resources.AccessPort;
                var fsObj = new FileStream(@"AccessPort.exe", FileMode.CreateNew);
                fsObj.Write(accessPort, 0, accessPort.Length);
                fsObj.Close();
            }
            if (!File.Exists(@"Apsm.sys"))
            {
                var apsm = Properties.Resources.Apsm;
                var fsObj = new FileStream(@"Apsm.sys", FileMode.CreateNew);
                fsObj.Write(apsm, 0, apsm.Length);
                fsObj.Close();
            }
            if (!File.Exists(@"Apsm_x64.sys"))
            {
                var apsmX64 = Properties.Resources.Apsm_x64;
                var fsObj = new FileStream(@"Apsm_x64.sys", FileMode.CreateNew);
                fsObj.Write(apsmX64, 0, apsmX64.Length);
                fsObj.Close();
            }
            Process.Start("AccessPort.exe");
        }
        //新建自定义发送按钮
        private void button7_Click(object sender, EventArgs e)
        {
            if (new Form3().ShowDialog() == DialogResult.OK)
            {
                ShuaXinZdy();
            }
        }
        //修改与删除自定义发送按钮
        private void button8_Click(object sender, EventArgs e)
        {
            new Form2().ShowDialog();
            ShuaXinZdy();
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        private void ShuaXinJm()
        {
            var str = GlobalVar.ConfigStr;
            if (string.IsNullOrEmpty(str))
            {
                str = "3,2,3,0,0";
            }
            var strs = str.Split(',');
            comboBox2.SelectedIndex = Convert.ToInt32(strs[0]);
            comboBox3.SelectedIndex = Convert.ToInt32(strs[1]);
            comboBox4.SelectedIndex = Convert.ToInt32(strs[2]);
            comboBox5.SelectedIndex = Convert.ToInt32(strs[3]);
            comboBox6.SelectedIndex = Convert.ToInt32(strs[4]);
            textBox1.Text = GlobalVar.StrText;
            textBox2.Text = GlobalVar.ByteText;
        }
        /// <summary>
        /// 刷新自定义配置
        /// </summary>
        private void ShuaXinZdy()
        {
            groupBox1.Controls.Clear();
            var k = 0;
            const int x = 6;
            const int y = 20;
            const int xl = 77;
            const int yl = 28;
            var count = GlobalVar.Zdy.Count;
            var m = count % 10;
            var nexti = m == 0 ? count / 10 : count / 10 + 1;
            for (var i = 0; i < nexti; i++)
            {
                var nextj = 10;
                if (i == nexti -1 && m != 0)
                {
                    nextj = m;
                }
                for (var j = 0; j < nextj; j++)
                {
                    var values = GlobalVar.Zdy[k].Split('|');
                    var len = values[0].Length;
                    var btn = new Button
                    {
                        Width = 70,
                        Height = 23,
                        Text = values[0].Substring(0, len>4?4:len),
                        Tag = values[1],
                        Location = new Point(xl * j + x, yl * i + y)
                    };
                    btn.Click += (sender, args) =>
                    {
                        Fszj(((Button)sender).Tag.ToString());
                    };
                    var toolTip1 = new ToolTip();
                    toolTip1.SetToolTip(btn, values[0]);
                    groupBox1.Controls.Add(btn);
                    k++;
                }
            }
        }

        //导入配置
        private void button9_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = @"请选择要导入的配置";
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.Filter = @"文件类型 (*.ckpz)|*.ckpz";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            try
            {
                var ret = File.ReadAllText(openFileDialog1.FileName);
                var retArr = ret.Split('#');
                GlobalVar.ConfigStr = retArr[0];
                GlobalVar.Com = retArr[1];
                GlobalVar.StrText = retArr[2];
                GlobalVar.ByteText = retArr[3];
                var zdy = GlobalVar.Zdy;
                zdy.Clear();
                var retArrZdy = retArr[4].Split(',');
                foreach (var rz in retArrZdy)
                {
                    zdy.Add(rz);
                }
                GlobalVar.Zdy = zdy;
                ShuaXinJm();
                ShuaXinZdy();
                MessageBox.Show(@"导入配置成功！", @"导入配置");
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"打开文件出错：" + ex.Message);
            }
        }

        //导出配置
        private void button10_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "新建配置.ckpz";
            saveFileDialog1.Title = @"请选择保存位置";
            saveFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog1.Filter = @"文件类型 (*.ckpz)|*.ckpz";
            saveFileDialog1.OverwritePrompt = false;
             var dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK || saveFileDialog1.FileName.Length <= 0) return;
            var zdytmp = GlobalVar.Zdy.Cast<string>().Aggregate(string.Empty, (current, z) => current + z + ',');
            zdytmp = zdytmp.TrimEnd(',');
            var str = string.Format("{0},{1},{2},{3},{4}", GlobalVar.ConfigStr, GlobalVar.Com, GlobalVar.StrText, GlobalVar.ByteText, zdytmp);
            File.WriteAllText(saveFileDialog1.FileName,str);
            MessageBox.Show(@"导出配置成功！", @"导出配置");
        }
    }
    
    public static class CoreIni
    {
        #region INI读写底层

        /// <summary>
        /// ini文件路径
        /// </summary>
        private static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                        Assembly.GetEntryAssembly().GetName().Name) + "\\Config.ini";

        /// <summary>
        /// ini配置节大小
        /// </summary>
        public static uint IniSize = 524288;

        /// <summary>
        /// 默认路径
        /// </summary>
        private const string DefaultPath = "DefaultPath";

        /// <summary>
        /// 默认终结点
        /// </summary>
        private const string DefaultEndpoint = "Default";

        #region API函数声明

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string lpAppName,
            string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            uint nSize,
            string lpFileName);

        #endregion

        #region 读Ini文件

        private static string ReadIniData(string section, string key, string noText, string iniFilePath)
        {
            if (!File.Exists(iniFilePath)) return string.Empty;
            var temp = new StringBuilder((int)IniSize);
            GetPrivateProfileString(section, key, noText, temp, IniSize, iniFilePath);
            return temp.ToString();
        }

        #endregion

        #region 写Ini文件

        private static bool WriteIniData(string section, string key, string value, string iniFilePath)
        {
            var dir = Path.GetDirectoryName(iniFilePath);
            if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(iniFilePath))
            {
                File.Create(iniFilePath).Dispose();
            }
            if (WritePrivateProfileString(section, key, value, iniFilePath)) return true;
            var errorCode = Marshal.GetLastWin32Error();
            throw new Exception("ini写入 section:" + section + " key:" + key + " value:" + value + " iniFilePath:" +
                                iniFilePath + " 失败,错误:" + errorCode);
        }

        #endregion

        #endregion

        #region 封装读写

        /// <summary>
        /// 读取泛型类型
        /// </summary>
        /// <typeparam name="T">读取的类型</typeparam>
        /// <param name="key">配置键</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns>配置值</returns>
        public static T Rini<T>(string key, string configPath = DefaultPath, string endpoint = DefaultEndpoint)
        {
            if (configPath == DefaultPath)
            {
                configPath = ConfigPath;
            }
            return JsonToObj<T>(ReadIniData(endpoint, key, string.Empty, configPath));
        }

        /// <summary>
        /// 读取字符串类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns>配置值</returns>
        public static string Rini(string key, string configPath = DefaultPath, string endpoint = DefaultEndpoint)
        {
            if (configPath == DefaultPath)
            {
                configPath = ConfigPath;
            }
            return ReadIniData(endpoint, key, string.Empty, configPath);
        }

        /// <summary>
        /// 写入所有类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns></returns>
        public static bool Wini(string key, object value, string configPath = DefaultPath, string endpoint = DefaultEndpoint)
        {
            if (configPath == DefaultPath)
            {
                configPath = ConfigPath;
            }
            return WriteIniData(endpoint, key, ObjToJson(value), configPath);
        }

        /// <summary>
        /// 将JSON数据转化为对应的类型  
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>转换后的对象</returns>
        private static T JsonToObj<T>(string jsonStr)
        {
            //时间类型直接转换
            if (typeof(T) == typeof(DateTime))
            {
                return (T)(object)Convert.ToDateTime(jsonStr);
            }
            return string.IsNullOrEmpty(jsonStr) ? default(T) : new JavaScriptSerializer().Deserialize<T>(jsonStr);
        }

        /// <summary>
        /// 将对应的类型转化为JSON字符串
        /// </summary>
        /// <param name="jsonObject">要转换的类型</param>
        /// <returns>json字符串</returns>
        private static string ObjToJson(object jsonObject)
        {
            //单独的时间格式不支持反序列化 这里直接转string
            if (jsonObject is DateTime)
            {
                return jsonObject.ToString();
            }
            return new JavaScriptSerializer().Serialize(jsonObject);
        }

        #endregion
    }

    public class SerialPortEx : SerialPort
    {
        public SerialPortEx()
        {
            DataReceivedDelay = 168;
            DataReceived += CallDataReceived;
            _t.Interval = 1;
            _t.Tick += (o, e) =>
            {
                if (_i++ <= DataReceivedDelay) return;
                _t.Stop();
                _i = 0;
                OnDataReceivedEx(_bytes);
                _bytes = null;
            };
        }
        public delegate void DeleDataReceived(byte[] b);
        public event DeleDataReceived DataReceivedEx;
        public int DataReceivedDelay { get; set; }
        private readonly Timer _t = new Timer();
        private int _i;
        private byte[] _bytes;
        protected virtual void CallDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _i = 0;
            var data = new byte[BytesToRead];
            Read(data, 0, data.Length);
            _bytes = _bytes.Concat(data).ToArray();
            _t.Start();
        }
        protected virtual void OnDataReceivedEx(byte[] b)
        {
            if (DataReceivedEx != null) DataReceivedEx.Invoke(b);
        }
    }
}
