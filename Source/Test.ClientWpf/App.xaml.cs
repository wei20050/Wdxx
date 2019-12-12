using System;
using System.ServiceModel;
using System.Windows.Threading;
using NetFrameWork.Core;
using NetFrameWork.Core.WebService;
using Test.ClientWpf.Service;
using Test.ClientWpf.WsServiceReference;

namespace Test.ClientWpf
{
    public partial class App
    {
        public App()
        {
            //设置本地数据库
            LocalDatabaseHelp.SetDatabase();
        }
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CoreLog.Error(e);
        }
        /// <summary>
        /// 创建服务对象
        /// </summary>
        /// <returns></returns>
        public static WsSoap CreateWsService(string url = null, TimeSpan timeout = new TimeSpan())
        {
            var client = new WsSoapClient();
            client.Endpoint.Address = new EndpointAddress(url ?? GlobalVar.ServiceBaseUrl);
            var userInfo = GlobalVar.UserInfo ?? new UserInfo();
            var userInfoJson = CoreConvert.ObjToJson(userInfo);
            var authorization = CoreEncrypt.AesEncrypt(userInfoJson, GlobalConst.AesKey);
            client.Endpoint.Behaviors.Add(AuthHelper.CreateAuthHeaderBehavior(authorization));
            //设置超时时间
            if (client.Endpoint.Binding == null) return client;
            client.Endpoint.Binding.ReceiveTimeout = timeout;
            client.Endpoint.Binding.SendTimeout = timeout;
            return client;
        }
    }
}
