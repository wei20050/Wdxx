using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Test.Client.Service;
using Tset.Entity;
using Wdxx.Core;

namespace Test.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //界面初始化
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        //无参返回字符串
        private void button1_Click(object sender, EventArgs e)
        {
            var ret = GlobalVar.TestService.Send("Test");
            listBox1.Items.Add(ret);
        }
        //无参返回泛型
        private void button2_Click(object sender, EventArgs e)
        {
            var ret = GlobalVar.TestService.Send<DateTime>("GetTime");
            listBox1.Items.Add(ret.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        //带参数返回泛型
        private void button3_Click(object sender, EventArgs e)
        {
            var ret = GlobalVar.TestService.Send<user>("TestStr", 1, "张三丰");
            listBox1.Items.Add(ret.name);
        }

        //新增id=1
        private void button8_Click(object sender, EventArgs e)
        {
            var user = new user { id = 1, name = "李四" };
            var ret = GlobalVar.TestService.Send<int>("Insert", user);
            listBox1.Items.Add(ret);
        }

        //新增id根据时间来
        private void button4_Click(object sender, EventArgs e)
        {
            var user = new user { id = Convert.ToInt32(DateTime.Now.ToString("HHmmssfff")), name = "张三" };
            var ret = GlobalVar.TestService.Send<int>("Insert", user);
            listBox1.Items.Add(ret);
        }

        //修改
        private void button9_Click(object sender, EventArgs e)
        {
            var user = new user { id = 1, name = "山大圣诞礼物" };
            var ret = GlobalVar.TestService.Send<int>("Update", user);
            listBox1.Items.Add(ret);
        }

        //删除
        private void button10_Click(object sender, EventArgs e)
        {
            var ret = GlobalVar.TestService.Send<int>("Delete", 1);
            listBox1.Items.Add(ret);
        }

        //查询id=1
        private void button11_Click(object sender, EventArgs e)
        {
            var ret = GlobalVar.TestService.Send<List<user>>("Select", 1);
            dataGridView1.DataSource = ret;
        }

        //查询id=1name=张三
        private void button12_Click(object sender, EventArgs e)
        {
            var ret = GlobalVar.TestService.Send<List<user>>("Select", 1, "张三");
            dataGridView1.DataSource = ret;
        }

        //查询所有
        private void button13_Click(object sender, EventArgs e)
        {
            var ret = GlobalVar.TestService.Send<List<user>>("SelectAll");
            dataGridView1.DataSource = ret;
        }

        //清空列表框
        private void button15_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        //连接服务器
        private void button14_Click(object sender, EventArgs e)
        {
            if (ServiceHelp.ServiceIni(textBox1.Text))
            {
                Text = @"服务已连接!";
                MessageBox.Show(@"服务连接成功!" );
            }
            else
            {
                Text = @"等待连接... ...";
                MessageBox.Show(@"服务没有连接!");
            }
        }

        //测试
        private void button7_Click(object sender, EventArgs e)
        {
            GlobalVar.TestService = new CoreClient("http://localhost:6600/QueueService.asmx/");
            var ret = GlobalVar.TestService.Send<List<queue_call_station>>("GetQueueCallStationList");
            MessageBox.Show(ret.ToString());
        }
    }

    /// <summary>
    /// 叫号站表
    /// </summary>
    public class queue_call_station
    {
        /// <summary>
        /// 叫号站id
        /// </summary>
        public int? queue_call_station_id { get; set; }
        /// <summary>
        /// 叫号站名称
        /// </summary>
        public string call_station_name { get; set; }
        /// <summary>
        /// 推送方式(0 共享 1 不共享 2全部)
        /// </summary>
        public int? push_share { get; set; }
        /// <summary>
        /// 取号来源(0 共享 1 不共享 2全部)
        /// </summary>
        public int? takeno_share { get; set; }
        /// <summary>
        /// 号类型 1 预检 2 登记 3 接种 6 儿保测量室 7 儿保诊疗室
        /// </summary>
        public int? takeno_type { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? update_date { get; set; }
        /// <summary>
        /// 删除标记0：正常 1：删除
        /// </summary>
        public int? del_flag { get; set; }
    }
}
