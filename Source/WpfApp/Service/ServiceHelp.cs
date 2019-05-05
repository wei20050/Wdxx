using System;
using System.ServiceModel;
using System.Windows;
using Client.WcfServiceReference;
using Wdxx.Core;

namespace Client.Service
{
    public static class ServiceHelp
    {
        /// <summary>
        /// 是否在线
        /// </summary>
        public static bool IsOnLine = true;

        /// <summary>
        /// Http服务地址
        /// </summary>
        public static string HttpUrl;

        /// <summary>
        /// 在线服务地址
        /// </summary>
        public static string ServiceUrl;

        /// <summary>
        /// 离线服务地址
        /// </summary>
        public static string OfflineServiceUrl;

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
                CreateServiceClient(url).Test();
                ServiceUrl = url;
                HttpUrl = ServiceUrl + "/api/";
                IsOnLine = true;
            }
            catch (Exception ex)
            {
                CoreLog.Error(ex);
                //离线服务开启
                if (MessageBoxResult.Yes != MessageBox.Show("离线中！是否还要继续？", "提示", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning)) return false;
                IsOnLine = false;
                OfflineServiceUrl = CoreHost.OpenHost(typeof(WcfService.IService),typeof(WcfService.Service),"http://localhost:88/a/");
                HttpUrl = OfflineServiceUrl + "/api/";
                //设置本地数据库
                LocalDatabaseHelp.SetDatabase();
            }
            return true;
        }

        /// <summary>
        /// 获取服务操作对象根据离线状态获取
        /// </summary>
        /// <returns></returns>
        public static ServiceClient CreateServiceClient()
        {
            var serviceUrl = IsOnLine ? ServiceUrl : OfflineServiceUrl;
            return CreateServiceClient(serviceUrl);
        }

        /// <summary>
        /// 获取服务操作对象 根据url获取
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <returns></returns>
        private static ServiceClient CreateServiceClient(string serviceUrl)
        {
            var client = new ServiceClient();
            client.Endpoint.Address = new EndpointAddress(new Uri(serviceUrl));
            return client;
        }

    }
}
