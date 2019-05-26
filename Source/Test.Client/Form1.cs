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
            CorePublic.Administrator();
            CorePublic.IsStart();
            InitializeComponent();
        }
        //界面初始化
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        //get无参返回字符串
        private void button1_Click(object sender, EventArgs e)
        {
            var ret = CoreHttp.Get(ServiceHelp.HttpUrl + "test");
            listBox1.Items.Add(ret);
        }
        //get无参返回泛型
        private void button2_Click(object sender, EventArgs e)
        {
            var ret = CoreHttp.Get<DateTime>(ServiceHelp.HttpUrl + "test");
            listBox1.Items.Add(ret);
        }
        //get带参数返回字符串
        private void button3_Click(object sender, EventArgs e)
        {
            var ret = CoreHttp.Get(ServiceHelp.HttpUrl + "test?id=11&msg=张三");
            listBox1.Items.Add(ret);
        }
        //post无参返回字符串
        private void button4_Click(object sender, EventArgs e)
        {
            var ret = CoreHttp.Post(ServiceHelp.HttpUrl + "test");
            listBox1.Items.Add(ret);
        }
        //post无参返回泛型
        private void button5_Click(object sender, EventArgs e)
        {
            var ret = CoreHttp.Post<DateTime>(ServiceHelp.HttpUrl + "test");
            listBox1.Items.Add(ret);
        }
        //post带参数返回字符串
        private void button6_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id",1);
            dic.Add("msg", "post带参数");
            var ret = CoreHttp.Post(ServiceHelp.HttpUrl + "test", dic);
            listBox1.Items.Add(ret);
        }
        //新增
        private void button8_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("u", CoreConvert.ObjToJson(new user
            {
                id = 1,
                name = "张三"
            }));
            var ret = CoreHttp.Post<int>(ServiceHelp.HttpUrl + "user", dic);
            listBox1.Items.Add(ret);
        }
        //修改
        private void button9_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("u", CoreConvert.ObjToJson(new user
            {
                id = 1,
                name = CorePublic.GenerateId().ToString()
            }));
            var ret = CoreHttp.Put<int>(ServiceHelp.HttpUrl + "user", dic);
            listBox1.Items.Add(ret);
        }
        //删除
        private void button10_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", 1);
            var ret = CoreHttp.Delete<int>(ServiceHelp.HttpUrl + "user", dic);
            listBox1.Items.Add(ret);
        }
        //查询id=1
        private void button11_Click(object sender, EventArgs e)
        {
            var ret = CoreHttp.Get<user>(ServiceHelp.HttpUrl + "user?id=1");
            dataGridView1.DataSource = new List<user> {ret};
        }
        //查询id=1name=张三
        private void button12_Click(object sender, EventArgs e)
        {
            var ret = CoreHttp.Get<user>(ServiceHelp.HttpUrl + "user?id=1&name=张三");
            dataGridView1.DataSource = new List<user> { ret };
        }
        //查询所有
        private void button13_Click(object sender, EventArgs e)
        {
            var ret = CoreHttp.Get<List<user>>(ServiceHelp.HttpUrl + "user");
            dataGridView1.DataSource = ret;
        }
        //清空列表框
        private void button15_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
        //测试服务器连接
        private void button14_Click(object sender, EventArgs e)
        {
            ServiceHelp.ServiceIni(textBox1.Text);
            MessageBox.Show(@"服务连接成功!");
        }
        //测试
        private void button7_Click(object sender, EventArgs e)
        {
            
        }

    }
}
