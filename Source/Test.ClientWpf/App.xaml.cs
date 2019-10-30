using System.Windows.Threading;
using NetFrameWork.Core;
using Test.ClientWpf.Service;

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
        private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CoreLog.Error(e);
        }
    }
}
