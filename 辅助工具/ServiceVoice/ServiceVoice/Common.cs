using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;

namespace ServiceVoice
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

        #region 系统操作

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


        /// <summary>
        /// 判断系统是否已启动
        /// </summary>
        public static void IsStart()
        {
            for (var i = 0; i < 8; i++)
            {
                //获取指定的进程名
                var myProcesses = Process.GetProcessesByName(AppName);
                //如果可以获取到知道的进程名则说明已经启动
                if (myProcesses.Length <= 1) return;
                System.Threading.Thread.Sleep(168);
            }
            MessageBox.Show("程序已启动！");
            //关闭系统
            Environment.Exit(0);
        }

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
        /// 获取本机IP地址
        /// </summary>
        /// <returns>本机IP地址</returns>
        public static List<string> GetLocalIp()
        {
            var ips = new List<string>();
            try
            {
                ips.AddRange(from t in Dns.GetHostEntry(Dns.GetHostName()).AddressList where t.AddressFamily == AddressFamily.InterNetwork select t.ToString());
                return ips;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 重启程序
        /// </summary>
        public static void Restart()
        {
            System.Windows.Forms.Application.Restart();
            Environment.Exit(0);
        }

        /// <summary>
        /// 简单记录日志
        /// </summary>
        /// <param name="content"></param>
        public static void Log(string content)
        {
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }
            File.AppendAllText($"logs\\{DateTime.Now:yyyy-MM-dd}.txt", DateTime.Now + content + Environment.NewLine);
        }

        /// <summary>
        /// 获取本地服务宿主
        /// </summary>
        /// <param name="t"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static ServiceHost CreateServiceHost(Type t, string uri)
        {
            const int m = 10;
            const int size = 2147483647;
            var host = new ServiceHost(t, new Uri(uri));
            host.AddServiceEndpoint(t, new WebHttpBinding
            {
                CrossDomainScriptAccessEnabled = true,
                CloseTimeout = new TimeSpan(0, m, 0),
                OpenTimeout = new TimeSpan(0, m, 0),
                SendTimeout = new TimeSpan(0, m, 0),
                ReceiveTimeout = new TimeSpan(0, m, 0),
                MaxReceivedMessageSize = size,
                MaxBufferPoolSize = size,
                AllowCookies = true
            }, "").Behaviors.Add(new WebHttpBehavior
            {
                AutomaticFormatSelectionEnabled = true,
                DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json
            });
            return host;
        }
    }
}
