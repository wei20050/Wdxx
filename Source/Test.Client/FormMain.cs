using System;
using System.Windows.Forms;
using Test.Client.Service;
using Test.Client.View;
using Test.Entity;

namespace Test.Client
{
    public partial class FormMain : FormBase
    {
        public FormMain()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            LocalDatabaseHelp.SetDatabase();
            ServiceHelp.ServiceIni("http://localhost:64276");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FormYuan().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //var ret = ServiceHelp.A.Get<List<user>>("User");
                //var ret2 =ApiUser.Get<List<user>>();
                //var ret1 = ServiceHelp.A.Get<user>("User", "1", "张三");
                MessageBox.Show("ok");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var test = Api.GetTest();
            var b = Api.PostUser(new user{id = "123",name = "张三四"});
            var us = Api.GetUser();
            var u = Api.GetUser("123");
        }
    }
}
