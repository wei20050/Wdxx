using System;
using System.Windows;
using NetFrameWork.Core;
using NetFrameWork.Core.WebService;
using Panuon.UI.Silver;
using Test.ClientWpf.WsServiceReference;

namespace Test.ClientWpf.Service
{
    public static class ServiceHelp
    {
        /// <summary>
        /// 是否在线
        /// </summary>
        public static bool IsOnLine = true;

        /// <summary>
        /// 服务器初始化确定在线离线
        /// </summary>
        /// <param name="url">在线服务地址</param>
        /// <returns>返回是否初始化成功</returns>
        public static bool ServiceIni(string url)
        {
            try
            {
                //检测服务是否正常连接  若无法连接 开启离线模式
                GlobalVar.ServiceBaseUrl = url;
                App.CreateWsService().Test();
                GlobalVar.UserInfo = App.CreateWsService().GetUserInfo(new UserInfo { UserName = "Admin" });
                IsOnLine = true;
            }
            catch (Exception ex)
            {
                CoreLog.Error(ex);
                //离线服务开启
                if (MessageBoxX.Show(@"离线中！是否还要继续？", @"提示", null, MessageBoxButton.YesNo) != MessageBoxResult.Yes) return false;
                IsOnLine = false;
                var httpUrl = new CoreHost(typeof(Test.Service.Ws)).Open();
                try
                {
                    GlobalVar.ServiceBaseUrl = httpUrl;
                    App.CreateWsService().Test();
                    GlobalVar.UserInfo = App.CreateWsService().GetUserInfo(new UserInfo { UserName = "Admin" });
                }
                catch (Exception e)
                {
                    CoreLog.Error(e);
                    return false;
                }
            }
            return true;
        }

    }
}
