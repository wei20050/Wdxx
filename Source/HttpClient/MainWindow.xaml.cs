using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Client.Service;
using Tset.Entity;
using Wdxx.Core;

namespace Client
{
    
    public partial class MainWindow
    {
        public MainWindow()
        {
            CorePublic.Administrator();
            CorePublic.IsStart();
            InitializeComponent();
        }

        /// <summary>
        /// 主界面初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //CorePublic.DeleteExit();
            CorePublic.DeleteMax();
        }

        /// <summary>
        /// httpget
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(CoreHttp.Get<string>(ServiceHelp.HttpUrl + "get?id=1&name=张三"));
        }

        /// <summary>
        /// httpgetuser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var u = CoreHttp.Get<user>(ServiceHelp.HttpUrl + "getuser?id=2&name=李四");
            MessageBox.Show(u.id + u.name);
        }

        /// <summary>
        /// httppost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var data = new { id = 3, name = "王五" };
            MessageBox.Show(CoreHttp.Post<string>(ServiceHelp.HttpUrl + "post", data));
        }

        /// <summary>
        /// httppostuser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var user = new { id = 4, name = "六麻子" };
            var u = CoreHttp.Post<user>(ServiceHelp.HttpUrl + "postuser", user);
            MessageBox.Show(u.id + u.name);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var user = new user { id = 1, name = "六麻子111" };
            var data = new { u = user };
            MessageBox.Show(CoreHttp.Post<int>(ServiceHelp.HttpUrl + "insert", data).ToString());
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var data = new { id = 1 };
            MessageBox.Show(CoreHttp.Post<int>(ServiceHelp.HttpUrl + "delete", data).ToString());
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            var user = new user { id = 4, name = "山大圣诞礼物" };
            var data = new { u = user };
            MessageBox.Show(CoreHttp.Post<int>(ServiceHelp.HttpUrl + "update", data).ToString());
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            var u = CoreHttp.Get<user>(ServiceHelp.HttpUrl + "select?id=4&name=hdads");
            DataGrid1.ItemsSource = new List<user> { u };
        }

        /// <summary>
        /// 查询多条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            var us = CoreHttp.Post<List<user>>(ServiceHelp.HttpUrl + "selectall");
            DataGrid1.ItemsSource = us;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Setting.ServiceUrl = TextBoxUrl.Text;
            ServiceHelp.ServiceIni(TextBoxUrl.Text);
            MessageBox.Show("服务连接成功!");
        }

        /// <summary>
        /// 测试保留
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(CoreHttp.Post<DateTime>(ServiceHelp.HttpUrl + "TestStr", new {dt = DateTime.Now})
                .ToString(CultureInfo.InvariantCulture));
        }

        //private struct Tmxx
        //{
        //    public string Dyjg;
        //    public string Xm;
        //    public string Ycxx;
        //}

        private void ButtonBase_OnClick1(object sender, RoutedEventArgs e)
        {
            //测试是否能取到姓名
            //var tm = CoreEncrypt.AesEncrypt("123456789011", "wondersgroupjztm");
            //var url = "http://10.1.93.110/ipms/api/getPdjhszzjbxxByQuery?szztm=" + tm;
            //var tmxx = CoreHttp.HttpPost<Tmxx>(url, null);
            //if (tmxx.Dyjg == "0")
            //{
            //    MessageBox.Show("成功获取到姓名:" + System.Web.HttpUtility.UrlDecode(tmxx.Xm, System.Text.Encoding.UTF8) );
            //}
            //else
            //{
            //    MessageBox.Show("错误:" + System.Web.HttpUtility.UrlDecode(tmxx.Ycxx, System.Text.Encoding.UTF8) );
            //}
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
           var time = CoreConvert.ObjToJson(DateTime.Now);

            var ret = CoreConvert.JsonToObj<DateTime>(time);
            MessageBox.Show(ret.ToString(CultureInfo.InvariantCulture));
        }
    }
}
