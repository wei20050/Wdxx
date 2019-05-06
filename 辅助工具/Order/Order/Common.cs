using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using Microsoft.Win32;
using Order.QueueService;

namespace Order
{

    /// <summary>
    /// 公共方法核心
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 当前运行的程序集名称
        /// </summary>

        private static readonly string AppName = Assembly.GetEntryAssembly().GetName().Name;

        #region 判断系统是否已启动

        /// <summary>
        /// 管理员身份运行程序
        /// 在界面初始化之前调用此方法程序将以管理员权限运行
        /// </summary>
        public static void Administrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            if (wp.IsInRole(WindowsBuiltInRole.Administrator)) return;
            var exePath = Assembly.GetEntryAssembly().CodeBase;
            Process.Start(new ProcessStartInfo(exePath)
            {
                UseShellExecute = true,
                Verb = "runas"
            });
            Environment.Exit(0);
        }

        public static void IsStart()
        {
            //获取指定的进程名
            var myProcesses = Process.GetProcessesByName(AppName);
            if (myProcesses.Length > 1) //如果可以获取到知道的进程名则说明已经启动
            {
                MessageBox.Show("程序已启动！");
                Environment.Exit(0);              //关闭系统
            }

        }

        #endregion

        #region 注册表添加与删除开机启动

        /// <summary>
        /// 设置注册表实现开机自动启动
        /// </summary>
        public static void AutoStart()
        {
            try
            {
                Log($"开机自动启动:{AppName}");
                var rKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                rKey?.SetValue(AppName, $@"""{AppDomain.CurrentDomain.BaseDirectory}{AppName}.exe""");
            }
            catch (Exception e)
            {
                Log("设置注册表实现开机自动启动异常:" + e);
            }
        }

        /// <summary>
        /// 删除注册表实现解除开机自动启动
        /// </summary>
        public static void UnAutoStart()
        {
            try
            {
                var rKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                var value = rKey?.GetValue(AppName);
                if (value == null || value.ToString() == string.Empty) return;
                Log($"解除开机自动启动:{AppName}");
                rKey.DeleteValue(AppName, true);
            }
            catch (Exception e)
            {
                Log("删除注册表实现解除开机自动启动异常:" + e);
            }
        }

        #endregion

        /// <summary>
        /// 服务对象
        /// </summary>
        private static readonly QueueServiceClient Sc = new QueueServiceClient();

        /// <summary>
        /// 拉取预约数据
        /// </summary>
        /// <param name="time"></param>
        public static string PullOrderService(string time = "")
        {
            if (time == "")
            {
                time = DateTime.Now.ToString("yyyy-MM-dd");
            }
            try
            {
                var orderInfo = Sc.PullOrderService(time);
                return orderInfo;
            }
            catch (Exception e)
            {
                Log("拉取预约数据异常:" + e);
                return e.Message;
            }
        }

        public static void Log(string content)
        {
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }
            File.AppendAllText($"logs\\{DateTime.Now:yyyy-MM-dd}.txt", DateTime.Now + content + Environment.NewLine);
        }
    }
}
