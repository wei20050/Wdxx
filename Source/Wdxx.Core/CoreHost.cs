using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Wdxx.Core
{

    /// <inheritdoc />
    /// <summary>
    /// 本地服务挂接类
    /// </summary>
    public class CoreHost : ServiceHost
    {

        /// <summary>
        /// 创建的服务地址
        /// </summary>
        private readonly string _uri;

        /// <inheritdoc />
        /// <summary>
        /// 单服务类构造
        /// </summary>
        /// <param name="t">要挂服务的类</param>
        public CoreHost(Type t) : this(t, t, GetUri()) { }

        /// <inheritdoc />
        /// <summary>
        /// 服务接口与服务实现类构造
        /// </summary>
        /// <param name="it">服务类接口</param>
        /// <param name="t">要挂服务的类</param>
        public CoreHost(Type it,Type t) : this(it, t, GetUri()) { }

        /// <inheritdoc />
        /// <summary>
        /// 单服务类带基地址构造
        /// </summary>
        /// <param name="t">要挂服务的类</param>
        /// <param name="uri">服务基地址</param>
        public CoreHost(Type t, string uri) : this(t, t, uri) { }

        /// <inheritdoc />
        /// <summary>
        /// 服务接口与服务实现类带基地址构造
        /// </summary>
        /// <param name="it">服务类接口</param>
        /// <param name="t">要挂服务的类</param>
        /// <param name="uri">服务基地址</param>
        public CoreHost(Type it, Type t, string uri) : base(t, new Uri(uri))
        {
            //保存基地址
            _uri = uri.TrimEnd('/') + "/";
            //默认超时时间分钟数
            const int m = 10;
            //最大数据大小
            const int size = 2147483647;
            AddServiceEndpoint(it, new WebHttpBinding
            {
                CrossDomainScriptAccessEnabled = true,
                CloseTimeout = new TimeSpan(0, m, 0),
                OpenTimeout = new TimeSpan(0, m, 0),
                SendTimeout = new TimeSpan(0, m, 0),
                ReceiveTimeout = new TimeSpan(0, m, 0),
                MaxReceivedMessageSize = size,
                MaxBufferSize = size,
                MaxBufferPoolSize = size,
                AllowCookies = true
            },string.Empty).Behaviors.Add(new WebHttpBehavior
            {
                AutomaticFormatSelectionEnabled = true,
                DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json
            });
        }

        /// <summary>
        /// 判断端口是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool IsPortAvailble(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            return tcpConnInfoArray.All(tcpi => tcpi.LocalEndPoint.Port != port);
        }

        /// <summary>
        /// 动态获取可用的端口
        /// </summary>
        /// <returns></returns>
        private static int GetFreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        /// <summary>
        /// 获取离线Uri
        /// </summary>
        /// <returns></returns>
        private static string GetUri()
        {
            var port = 80;
            if (IsPortAvailble(80))
            {
                port = GetFreeTcpPort();
            }
            return "http://127.0.0.1:" + port + "/";
        }

        /// <summary>
        /// 开启服务 返回服务地址
        /// </summary>
        public string OpenHost()
        {
            Action open = Open;
            //这里异步开启本地服务
            open.BeginInvoke(null, null);
            return _uri;
        }
    }
}
