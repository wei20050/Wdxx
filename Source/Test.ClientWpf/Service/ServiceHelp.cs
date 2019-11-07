using System;
using System.ServiceModel;
using System.Windows;
using NetFrameWork.Core;
using NetFrameWork.Core.WebService;
using Test.ClientWpf.WsService;

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
                GlobalVar.TestService = new WsSoapClient { Endpoint = { Address = new EndpointAddress(url) } };
                GlobalVar.TestService.Test();
                IsOnLine = true;
            }
            catch (Exception ex)
            {
                CoreLog.Error(ex);
                //离线服务开启
                if (MessageBox.Show(@"离线中！是否还要继续？", @"提示", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return false;
                IsOnLine = false;
                var httpUrl = new CoreHost(typeof(Test.Service.Ws)).Open();
                try
                {
                    GlobalVar.TestService = new WsSoapClient { Endpoint = { Address = new EndpointAddress(httpUrl) } };
                    GlobalVar.TestService.Test();
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
