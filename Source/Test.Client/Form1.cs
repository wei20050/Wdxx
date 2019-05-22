using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tset.Entity;
using Wdxx.Core;

namespace Test.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //CorePublic.Administrator();
            CorePublic.IsStart();
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
        }

        private void button1_Click(object sender, System.EventArgs e)
        {

        }

        private void button2_Click(object sender, System.EventArgs e)
        {

        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            var ret = CoreHttp.Get<string>(textBox1.Text + "/test?id=11&msg=张三");
            listBox1.Items.Add(ret);
        }

        private void button4_Click(object sender, System.EventArgs e)
        {

        }

        private void button5_Click(object sender, System.EventArgs e)
        {
        }

        private void button6_Click(object sender, System.EventArgs e)
        {
            var u1 = new user
            {
                id = 1,
                name = "张三"
            };
            var u2 = new user
            {
                id = 1,
                name = "张三"
            };
            var ret = CoreHttp.Post<string>(textBox1.Text + "/DateTime", new {u1,u2});
            listBox1.Items.Add(ret);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var jsonstr = CoreConvert.ObjToJson("http://127.0.0.1:888/");
            var obj = CoreConvert.JsonToObj<string>(jsonstr);

        }
    }
}
