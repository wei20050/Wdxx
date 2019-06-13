using System;
using System.Runtime.InteropServices;

namespace Test.ServiceHost
{
    internal class Program
    {
        #region API
        
        [DllImport("User32.dll ")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll ")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll ")]
        private static extern int RemoveMenu(IntPtr hMenu, int nPos, int flags);

        #endregion
        private static void Main()
        {
            const string serviceName = "测试服务";
            Console.Title = serviceName;

            #region 禁用控制台窗口的关闭按钮
            var windowHandler = FindWindow(null, serviceName);
            var closeMenu = GetSystemMenu((IntPtr)windowHandler, IntPtr.Zero);
            RemoveMenu(closeMenu, 0xF060, 0x0);
            #endregion
            var closeFlag = true;
            using (var host = new System.ServiceModel.ServiceHost(typeof(HttpService.Service)))
            {
                host.Opened += (o, e) =>
                {
                    Console.WriteLine("启动时间：" + DateTime.Now);
                    Console.WriteLine(serviceName + " 启动 ... ...");
                };
                host.Open();
                while (closeFlag)
                {
                    var exit = Console.ReadLine();
                    if (!string.IsNullOrEmpty(exit) && exit.ToLower() == "exit")
                    {
                        closeFlag = false;
                    }
                }
            }
        }
    }
}
