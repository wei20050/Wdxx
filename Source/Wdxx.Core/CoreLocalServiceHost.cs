using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace Wdxx.Core
{

    /// <inheritdoc />
    /// <summary>
    /// 本地服务挂接类
    /// </summary>
    public class CoreLocalServiceHost : ServiceHost
    {

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">要挂服务的类</param>
        protected CoreLocalServiceHost(Type type) : base(type)
        { }

        /// <summary>
        /// 离线服务命名空间名称
        /// </summary>
        protected static string LocalNamespace { get; set; }

        /// <summary>
        /// 离线服务名称
        /// </summary>
        protected static string LocalService { get; set; }


        /// <summary>
        /// 离线服务端IP
        /// </summary>
        protected static string ServiceIp { get; set; }

        /// <summary>
        /// 离线服务端口号
        /// </summary>
        private static int LocalPort
        {
            get
            {
                return CoreIni.Rini<int>("LocalPort");
            }
            set
            {
                CoreIni.Wini("LocalPort", value);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 初始化服务端配置文件
        /// </summary>
        protected override void ApplyConfiguration()
        {
            var configFileName = "Local" + LocalNamespace + ".dll.config";
            if (string.IsNullOrEmpty(LocalNamespace))
            {
                throw new Exception("请先给 {localNamespace} 属性赋值!");
            }
            if (!File.Exists(configFileName))
            {
                CoreLog.Info("没找到 " + configFileName + " 文件 自动创建 " + configFileName);
                //替换config文件中的服务名称
                var configValue = Config.Replace("LocalNamespace", LocalNamespace).Replace("LocalService", LocalService);
                //写本地服务配置
                File.WriteAllText(configFileName, configValue);
            }
            //加载配置
            LoadConfig(configFileName);
        }

        /// <summary>
        /// 加载配置配置
        /// </summary>
        /// <param name="configFilename"></param>
        private void LoadConfig(string configFilename)
        {
            //读取配置
            var filemap = new ExeConfigurationFileMap { ExeConfigFilename = configFilename };
            var config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);
            var serviceModel = ServiceModelSectionGroup.GetSectionGroup(config);
            //判断端口是否占用
            var port = LocalPort;
            if (port == 0)
            {
                port = 80;
                LocalPort = port;
            }
            if (!IsPortAvailble(port))
            {
                port = GetFreeTcpPort();
                CoreLog.Info("默认端口{" + LocalPort + "}已被占用，采用动态获取的端口{" + port + "}");
                LocalPort = port;
            }
            if (serviceModel == null)
            {
                throw new Exception("服务配置异常,请删除{" + configFilename + "} 后重试!");
            }
            serviceModel.Services.Services[0].Host.BaseAddresses[0].BaseAddress =
                "http://" + ServiceIp + ":" + port + "/" + LocalNamespace + "/" + LocalService + "/";
            var loaded = false;
            foreach (ServiceElement se in serviceModel.Services.Services)
            {
                if (se.Name != Description.ConfigurationName)
                {
                    continue;
                }
                LoadConfigurationSection(se);
                loaded = true;
                break;
            }
            if (!loaded)
            {
                throw new Exception("服务配置异常,请删除{" + configFilename + "} 后重试!");
            }
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
        /// 根据Wcf服务类开启离线服务 返回开启离线服务地址  http请求模式地址后面加  Api/
        /// </summary>
        /// <param name="t">挂载的服务类</param>
        /// <returns>返回服务url</returns>
        public static string OpenHost(Type t)
        {
            return OpenHost(t, "localhost");
        }

        /// <summary>
        /// 根据Wcf服务类开启离线服务 返回开启离线服务地址  http请求模式地址后面加  Api/
        /// </summary>
        /// <param name="t">挂载的服务类</param>
        /// <param name="ip">指定的本地服务ip</param>
        /// <returns>返回服务url</returns>
        public static string OpenHost(Type t, string ip)
        {
            ServiceIp = ip;
            LocalService = t.Name;
            LocalNamespace = t.Assembly.ManifestModule.Name.Replace(".dll", string.Empty);
            var serviceHost = new CoreLocalServiceHost(t);
            Action open = () =>
            {
                serviceHost.Open();
            };
            //这里异步开启本地服务
            open.BeginInvoke(null, null);
            //返回本地服务的地址
            return serviceHost.Description.Endpoints[0].Address.Uri.ToString();
        }

        #region 离线服务默认配置内容
        /// <summary>
        /// 离线服务默认配置内容
        /// </summary>
        private const string Config = @"<?xml version=""1.0""?>
                                    <configuration>
                                        <system.web>
                                            <compilation debug = ""true"" targetFramework=""4.0""/>
                                        </system.web>
                                        <system.serviceModel>
                                            <services>
                                                <service name = ""LocalNamespace.LocalService"" behaviorConfiguration=""Default"">
                                                    <endpoint address = """" binding=""basicHttpBinding"" contract=""LocalNamespace.ILocalService""/>
                                                    <endpoint address = ""Api"" binding=""webHttpBinding"" behaviorConfiguration=""WebBehavior"" bindingConfiguration=""basicTransport"" contract=""LocalNamespace.ILocalService""/>
                                                    <host>
                                                        <baseAddresses>
                                                            <add baseAddress = ""http://localhost/LocalService"" />
                                                        </baseAddresses >
                                                    </host >
                                                </service >
                                            </services >
                                            <behaviors >
                                                <endpointBehaviors >
                                                    <behavior name = ""WebBehavior"">
                                                        <webHttp/>
                                                    </behavior>
                                                </endpointBehaviors>
                                                <serviceBehaviors>
                                                    <behavior name = ""Default"" >
                                                        <serviceMetadata httpGetEnabled = ""true"" />
                                                        <serviceDebug includeExceptionDetailInFaults = ""true"" />
                                                    </behavior >
                                                </serviceBehaviors >
                                            </behaviors >
                                            <bindings >
                                                <webHttpBinding >
                                                    <binding name = ""basicTransport"" closeTimeout=""00:10:00"" receiveTimeout=""00:20:00"" sendTimeout=""00:20:00"" maxBufferSize=""2147483647"" maxReceivedMessageSize=""2147483647"">
                                                        <security mode = ""None"" />
                                                    </binding >
                                                </webHttpBinding >
                                            </bindings >
                                            <serviceHostingEnvironment multipleSiteBindingsEnabled = ""true""/>
                                        </system.serviceModel>
                                        <system.webServer>
                                            <modules runAllManagedModulesForAllRequests = ""true"" />
                                            <directoryBrowse enabled = ""true""/>
                                        </system.webServer>
                                    </configuration>";
        #endregion

    }
}
