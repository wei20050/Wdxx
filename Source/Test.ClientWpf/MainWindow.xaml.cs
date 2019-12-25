using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NetFrameWork.Core;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using Test.ClientWpf.Service;
using Test.ClientWpf.View;
using Test.ClientWpf.WsServiceReference;

namespace Test.ClientWpf
{
    public partial class MainWindow
    {
        public MainWindowModel MainWindowModel => DataContext as MainWindowModel;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void TestService(object sender, RoutedEventArgs e)
        {
            MainWindowModel.Text = ServiceHelp.ServiceIni(MainWindowModel.ServiceUrl) ? @"服务已连接!" : @"等待连接... ...";
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            MainWindowModel.Msg = App.CreateWsService().Test();
        }

        private void TestStr(object sender, RoutedEventArgs e)
        {
            MainWindowModel.Msg = App.CreateWsService().TestStr(123, "一二三");
        }

        private void GetTime(object sender, RoutedEventArgs e)
        {
            MainWindowModel.Msg = App.CreateWsService().GetTime().ToString(CultureInfo.InstalledUICulture);
        }

        private void Insert(object sender, RoutedEventArgs e)
        {
            var user = new user { id = 1, name = "张三" };
            var ret = App.CreateWsService().Insert(user);
            MainWindowModel.Msg = ret.ToString();
        }

        public void InsertEx(object sender, RoutedEventArgs e)
        {
            var user = new user { id = Math.Abs(CorePublic.GenerateId()), name = "李四" };
            var ret = App.CreateWsService().Insert(user);
            MainWindowModel.Msg = ret.ToString();
        }

        public void Update(object sender, RoutedEventArgs e)
        {
            var user = new user { id = 1, name = "张修改" };
            var ret = App.CreateWsService().Update(user);
            MainWindowModel.Msg = ret.ToString();
        }

        public void UpdateEx(object sender, RoutedEventArgs e)
        {
            var user = new user { id = MainWindowModel.EditId, name = "根修改" };
            var ret = App.CreateWsService().Update(user);
            MainWindowModel.Msg = ret.ToString();
        }

        public void Delete(object sender, RoutedEventArgs e)
        {
            var ret = App.CreateWsService().Delete(1);
            MainWindowModel.Msg = ret.ToString();
        }

        public void Select(object sender, RoutedEventArgs e)
        {
            var ret = App.CreateWsService().Select(1, "");
            MainWindowModel.UserList = ret.ToList();
        }

        public void SelectEx(object sender, RoutedEventArgs e)
        {
            var ret = App.CreateWsService().Select(1, "根修改");
            MainWindowModel.UserList = ret.ToList();
        }

        public void SelectAll(object sender, RoutedEventArgs e)
        {
            var ret = App.CreateWsService().SelectAll();
            MainWindowModel.UserList = ret.ToList();
        }

        public void Test1(object sender, RoutedEventArgs e)
        {
            MainWindowModel.Msg = "Test1";
            MessageBoxX.Show("dawdwaddawdwadwadwadawdawdawdwadwadwadawdawdawdwadwadwadawdawwadwadawdaw", "biaoti", null,
                MessageBoxButton.YesNoCancel, new MessageBoxXConfigurations
                {
                    MessageBoxIcon = MessageBoxIcon.Error,
                    ButtonBrush = "#FF4C4C".ToColor().ToBrush()
                });
        }

        public async void Test2(object sender, RoutedEventArgs e)
        {
            MainWindowModel.Msg = "Test2";
            var handler = PendingBox.Show("Please wait (1/2)...", "Processing", false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
            });
            await Task.Delay(2000);
            handler.UpdateMessage("Almost complete (2/2)...");
            await Task.Delay(2000);
            handler.Close();
        }

        public void Test3(object sender, RoutedEventArgs e)
        {
            MainWindowModel.Msg = "Test3";
            new NavbarWindow().Show();
        }

        public void ClearMsg(object sender, RoutedEventArgs e)
        {
            MainWindowModel.Msg = string.Empty;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
