using NetFrameWork.Core;
using System;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using Ztest.ServiceReference1;

namespace Ztest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var xcs = Convert.ToInt32(textBox2.Text);
            var xccs = Convert.ToInt32(textBox3.Text);
            var cc = new WsSoapClient();
            cc.Endpoint.Address = new EndpointAddress(textBox1.Text);
            for (var i = 0; i < xcs; i++)
            {
                var i1 = i;
                new Thread(() =>
                {
                    CoreLog.Info("xc :" + i1 + " Start");
                    for (var j = 0; j < xccs; j++)
                    {
                        CoreLog.Info("xc :" + i1 + " j :" + j + " ks");
                        var n = CorePublic.GenerateId();
                        var user = new user { id = n, name = i1 + "并发测试" + j };
                        var ret = cc.Insert(user);
                        CoreLog.Info("id :" + n + " xc :" + i1 + " j :" + j + " " + ret);
                        Thread.Sleep(10);
                    }
                }
                ).Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var cc = new WsSoapClient();
            cc.Endpoint.Address = new EndpointAddress(textBox1.Text);
            CoreLog.Info(" ks");
            var n = CorePublic.GenerateId();
            var user = new user { id = n, name = "单次测试" };
            var ret = cc.Insert(user);
            CoreLog.Info("id :" + n + "  " + ret);
            MessageBox.Show(@"调用成功返回:" + ret);
            cc.Close();
        }

    }
}
