﻿using System.Collections.Generic;
using System.Windows;
using Client.WcfServiceReference;
using Client.Service;
using Wdxx.Core;

namespace Client
{

    /// <inheritdoc cref="Window" />
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
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
            CorePublic.DeleteExit();
            CorePublic.DeleteMax();
        }

        /// <summary>
        /// httpget
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(CoreHttp.HttpGet<string>(ServiceHelp.HttpUrl + "get", "id=1&name=张三"));
        }

        /// <summary>
        /// httpgetuser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var u = CoreHttp.HttpGet<user>(ServiceHelp.HttpUrl + "getuser", "id=2&name=李四");
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
            MessageBox.Show(CoreHttp.HttpPost<string>(ServiceHelp.HttpUrl + "post", data));
        }

        /// <summary>
        /// httppostuser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var user = new { id = 4, name = "六麻子" };
            var u = CoreHttp.HttpPost<user>(ServiceHelp.HttpUrl + "postuser", user);
            MessageBox.Show(u.id + u.name);
        }

        /// <summary>
        /// wcf get
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var str = GlobalVar.Service.Get(100, "aaa");
            MessageBox.Show(str);
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
            MessageBox.Show(CoreHttp.HttpPost<int>(ServiceHelp.HttpUrl + "insert", data).ToString());
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var data = new { id = 1 };
            MessageBox.Show(CoreHttp.HttpPost<int>(ServiceHelp.HttpUrl + "delete", data).ToString());
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
            MessageBox.Show(CoreHttp.HttpPost<int>(ServiceHelp.HttpUrl + "update", data).ToString());
        }

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            //CoreHttp.HttpGet(GlobalVar.HttpUrl + "select", "id=4", out user u);
            var u = GlobalVar.Service.Select(4, "啊啊啊");
            DataGrid1.ItemsSource = new List<user> { u };
        }

        /// <summary>
        /// 查询多条数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            var us = CoreHttp.HttpPost<List<user>>(ServiceHelp.HttpUrl + "selectall", null);
            DataGrid1.ItemsSource = us;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var url = TextBoxUrl.Text;
            Setting.ServiceUrl = url;
            ServiceHelp.ServiceIni(url);
            GlobalVar.Service = ServiceHelp.CreateServiceClient();
        }

        private struct Tmxx
        {
            public string Dyjg;
            public string Xm;
            public string Ycxx;
        }

        private void ButtonBase_OnClick1(object sender, RoutedEventArgs e)
        {
            var tm = CoreEncrypt.AesEncrypt("123456789012", "wondersgroupjztm");
            var tmEncode = System.Web.HttpUtility.UrlEncode(tm, System.Text.Encoding.UTF8);
            var url = "http://10.1.93.110/ipms/api/getPdjhszzjbxxByQuery?szztm=" + tmEncode;
            var tmxx = CoreHttp.HttpPost<Tmxx>(url, null);
            if (tmxx.Dyjg == "0")
            {
                MessageBox.Show("成功获取到姓名:" + System.Web.HttpUtility.UrlDecode(tmxx.Xm, System.Text.Encoding.UTF8) );
            }
            else
            {
                MessageBox.Show("错误:" + System.Web.HttpUtility.UrlDecode(tmxx.Ycxx, System.Text.Encoding.UTF8) );
            }
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            var tm = CoreEncrypt.Core("123451");
            MessageBox.Show(tm);
        }
    }
}
