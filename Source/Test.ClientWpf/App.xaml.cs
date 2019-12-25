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
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            LocalDatabaseHelp.SetDatabase();
        }

        private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CoreLog.Error(e.Exception);
            e.Handled = true;
        }

        public static WsSoap CreateWsService(string url = null, int timeoutSeconds = 60)
        {
            var client = new WsSoapClient();
            client.Endpoint.Address = new EndpointAddress(url ?? GlobalVar.ServiceBaseUrl);
            var userInfo = GlobalVar.UserInfo ?? new UserInfo();
            var userInfoJson = CoreConvert.ObjToJson(userInfo);
            var authorization = CoreEncrypt.AesEncrypt(userInfoJson, GlobalConst.AesKey);
            client.Endpoint.Behaviors.Add(AuthHelper.CreateAuthHeaderBehavior(authorization));
            if (client.Endpoint.Binding == null) return client;
            client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 0, 0, timeoutSeconds);
            client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 0, timeoutSeconds);
            return client;
        }

    }
}
