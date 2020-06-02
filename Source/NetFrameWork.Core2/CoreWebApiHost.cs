using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global
namespace NetFrameWork.Core2
{
    /// <summary>
    /// WebApi宿主核心
    /// </summary>
    public class CoreWebApiHost
    {
        /// <summary>
        /// 开始宿主
        /// </summary>
        /// <param name="assemblyName">宿主的dll名称</param>
        /// <param name="port">端口号</param>
        public static string OpenAsync(string assemblyName = null, int port = 80)
        {
            GetPort(ref port);
            var url = "http://localhost:" + port;
            var config = new HttpSelfHostConfiguration(url);
            config.Routes.MapHttpRoute("default", "{controller}/{id}", new
            {
                id = RouteParameter.Optional
            });
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(new JsonMediaTypeFormatter()));
            var server = new HttpSelfHostServer(config);
            if (assemblyName != null)
            {
                server.Configuration.Services.Replace(typeof(IAssembliesResolver), new PluginsResolver(assemblyName));
            }
            server.OpenAsync().Wait();
            return url;
        }

        /// <summary>
        /// 获取离线U端口
        /// </summary>
        /// <returns></returns>
        private static void GetPort(ref int port)
        {
            if (!IsPortAvailable(80))
            {
                port = GetFreeTcpPort();
            }
        }

        /// <summary>
        /// 判断端口是否可用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool IsPortAvailable(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            return tcpConnInfoArray.All(tcp => tcp.LocalEndPoint.Port != port);
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
        ///  在全局设置中，使用自定义的只返回Json Result。只让api接口中替换xml，返回json。这种方法的性能是最高的
        /// </summary>
        public class JsonContentNegotiator : IContentNegotiator
        {
            private readonly JsonMediaTypeFormatter _jsonFormatter;

            /// <summary>
            /// json内容处理
            /// </summary>
            /// <param name="formatter"></param>
            public JsonContentNegotiator(JsonMediaTypeFormatter formatter)
            {
                _jsonFormatter = formatter;
            }

            /// <summary>
            /// 返回值处理
            /// </summary>
            /// <param name="type"></param>
            /// <param name="request"></param>
            /// <param name="formatters"></param>
            /// <returns></returns>
            public ContentNegotiationResult Negotiate(Type type, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
            {
                _jsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//解决json时间带T的问题
                _jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;//解决json格式化缩进问题
                _jsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//解决json序列化时的循环引用问题
                return new ContentNegotiationResult(_jsonFormatter, new MediaTypeHeaderValue("application/json"));
            }
        }

        /// <summary>
        /// 插件处理
        /// </summary>
        public class PluginsResolver : DefaultAssembliesResolver
        {
            private readonly string _assemblyName;

            /// <summary>
            /// 赋值程序集名称的构造
            /// </summary>
            /// <param name="an"></param>
            public PluginsResolver(string an)
            {
                _assemblyName = an;
            }

            /// <summary>
            /// 获得程序集
            /// </summary>
            /// <returns></returns>
            public override ICollection<Assembly> GetAssemblies()
            {
                var assemblies = new List<Assembly>(base.GetAssemblies())
                {
                    Assembly.LoadFrom(_assemblyName + ".dll")
                };
                return assemblies;
            }

        }

    }
}
