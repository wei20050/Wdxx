using PortsEx;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
                Properties.Settings.Default.configStr =
                    $"{comboBox2.SelectedIndex},{comboBox3.SelectedIndex},{comboBox4.SelectedIndex},{comboBox5.SelectedIndex},{comboBox6.SelectedIndex}";
                Properties.Settings.Default.Com = comboBox1.Text;
                Properties.Settings.Default.Save();
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
                Properties.Settings.Default.strText = textBox1.Text;
                Properties.Settings.Default.Save();
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
            Properties.Settings.Default.byteText = textBox2.Text;
            Properties.Settings.Default.Save();
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
                    if (coms.Contains(Properties.Settings.Default.Com))
                    {
                        comboBox1.Text = Properties.Settings.Default.Com;
                    }
                }));
            };
            if (Properties.Settings.Default.zdy == null)
            {
                Properties.Settings.Default.zdy = new StringCollection();
                Properties.Settings.Default.Save();
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
            var zdytmp = Properties.Settings.Default.zdy.Cast<string>().Aggregate(string.Empty, (current, z) => current + z + ',');
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
            var strs = Properties.Settings.Default.configStr.Split(',');
            comboBox2.SelectedIndex = Convert.ToInt32(strs[0]);
            comboBox3.SelectedIndex = Convert.ToInt32(strs[1]);
            comboBox4.SelectedIndex = Convert.ToInt32(strs[2]);
            comboBox5.SelectedIndex = Convert.ToInt32(strs[3]);
            comboBox6.SelectedIndex = Convert.ToInt32(strs[4]);
            textBox1.Text = Properties.Settings.Default.strText;
            textBox2.Text = Properties.Settings.Default.byteText;
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
            var count = Properties.Settings.Default.zdy.Count;
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
                    var values = Properties.Settings.Default.zdy[k].Split('|');
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
                Properties.Settings.Default.configStr = retArr[0];
                Properties.Settings.Default.Com = retArr[1];
                Properties.Settings.Default.strText = retArr[2];
                Properties.Settings.Default.byteText = retArr[3];
                Properties.Settings.Default.zdy.Clear();
                var retArrZdy = retArr[4].Split(',');
                foreach (var rz in retArrZdy)
                {
                    Properties.Settings.Default.zdy.Add(rz);
                }
                Properties.Settings.Default.Save();
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
            var zdytmp = Properties.Settings.Default.zdy.Cast<string>().Aggregate(string.Empty, (current, z) => current + z + ',');
            zdytmp = zdytmp.TrimEnd(',');
            var str = $@"{Properties.Settings.Default.configStr}#{Properties.Settings.Default.Com}#{Properties.Settings.Default.strText}#{Properties.Settings.Default.byteText}#{zdytmp}";
            File.WriteAllText(saveFileDialog1.FileName,str);
            MessageBox.Show(@"导出配置成功！", @"导出配置");
        }
    }
}
