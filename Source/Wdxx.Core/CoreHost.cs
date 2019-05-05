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

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="t">服务接口类</param>
        /// <param name="uri">服务基地址</param>
        protected CoreHost(Type t, string uri) : this(t, t, uri) { }

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="it">服务类接口</param>
        /// <param name="t">要挂服务的类</param>
        /// <param name="uri">服务基地址</param>
        protected CoreHost(Type it, Type t, string uri) : base(t, new Uri(uri + "/" + t.Name))
        {
            const int m = 10;
            const int size = 2147483647;
            AddServiceEndpoint(it, new WebHttpBinding
            {
                CrossDomainScriptAccessEnabled = true,
                CloseTimeout = new TimeSpan(0, m, 0),
                OpenTimeout = new TimeSpan(0, m, 0),
                SendTimeout = new TimeSpan(0, m, 0),
                ReceiveTimeout = new TimeSpan(0, m, 0),
                MaxReceivedMessageSize = size,
                MaxBufferPoolSize = size,
                AllowCookies = true
            }, "api").Behaviors.Add(new WebHttpBehavior
            {
                AutomaticFormatSelectionEnabled = true,
                DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json
            });
            var wcfBing = new WSHttpBinding
            {
                CloseTimeout = new TimeSpan(0, m, 0),
                OpenTimeout = new TimeSpan(0, m, 0),
                SendTimeout = new TimeSpan(0, m, 0),
                ReceiveTimeout = new TimeSpan(0, m, 0),
                MaxReceivedMessageSize = size,
                MaxBufferPoolSize = size,
                AllowCookies = true
            };
            Description.Behaviors.Add(new ServiceMetadataBehavior
            {
                HttpGetEnabled = true
            });
            AddServiceEndpoint(it, wcfBing, "");
            AddServiceEndpoint(typeof(IMetadataExchange), wcfBing, "mex");
        }

        /// <summary>
        /// 判断端口是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsPortAvailble(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            return tcpConnInfoArray.All(tcpi => tcpi.LocalEndPoint.Port != port);
        }

        /// <summary>
        /// 动态获取可用的端口
        /// </summary>
        /// <returns></returns>
        public static int GetFreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        /// <summary>
        /// 根据Wcf服务类开启离线服务 返回开启离线服务地址  http请求模式地址后面加  api/
        /// </summary>
        /// <param name="t">挂载的服务类</param>
        public static string OpenHost(Type t)
        {
            return OpenHost(t, t);
        }

        /// <summary>
        /// 根据Wcf服务类开启离线服务 返回开启离线服务地址  http请求模式地址后面加  api/
        /// </summary>
        /// <param name="it">服务接口类</param>
        /// <param name="t">挂载的服务类</param>
        public static string OpenHost(Type it, Type t)
        {
            var port = 80;
            if (IsPortAvailble(80))
            {
                port = GetFreeTcpPort();
            }
            return OpenHost(it, t, "http://127.0.0.1:" + port);
        }

        /// <summary>
        /// 根据Wcf服务类开启离线服务 返回开启离线服务地址  http请求模式地址后面加  api/
        /// </summary>
        /// <param name="t">挂载的服务类</param>
        /// <param name="uri">指定的本地服务地址</param>
        public static string OpenHost(Type t, string uri)
        {
            return OpenHost(t, uri);
        }

        /// <summary>
        /// 根据Wcf服务类开启离线服务 返回开启离线服务地址  http请求模式地址后面加  api/
        /// </summary>
        /// <param name="it">服务接口类</param>
        /// <param name="t">挂载的服务类</param>
        /// <param name="uri">指定的本地服务地址</param>
        public static string OpenHost(Type it, Type t, string uri)
        {
            var serviceUri = uri + "/" + t.Name;
            var serviceHost = new CoreHost(it, t, serviceUri);
            Action open = () =>
            {
                serviceHost.Open();
            };
            //这里异步开启本地服务
            open.BeginInvoke(null, null);
            return serviceUri;
        }
    }
}
