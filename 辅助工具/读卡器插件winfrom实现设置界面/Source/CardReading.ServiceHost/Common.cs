using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

// ReSharper disable PossibleNullReferenceException

namespace CardReading.ServiceHost
{

    /// <summary>
    /// 公共方法核心
    /// </summary>
    public class Common
    {

        /// <summary>
        /// 管理员身份运行程序
        /// 在界面初始化之前调用此方法程序将以管理员权限运行
        /// ClickOnce发布仅支持WPF应用程序 WinFrom不支持
        /// </summary>
        public static void Administrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            if (wp.IsInRole(WindowsBuiltInRole.Administrator)) return;
            var exePath = AppDomain.CurrentDomain.BaseDirectory + Path.GetFileName(Application.ExecutablePath);
            Process.Start(new ProcessStartInfo(exePath)
            {
                UseShellExecute = true,
                Verb = "runas"
            });
            Environment.Exit(0);
        }

        /// <summary>
        /// 当前运行的程序集名称
        /// </summary>

        private static readonly string AppName = Assembly.GetEntryAssembly().GetName().Name;

        #region 系统操作


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
            MessageBox.Show(@"程序已启动！");
            //关闭系统
            Environment.Exit(0);
        }

        /// <summary>
        /// 设置当前程序开机自启
        /// </summary>
        public static void AutoStart()
        {
            AutoStart(Environment.CurrentDirectory + "\\" + Assembly.GetEntryAssembly().GetName().Name + ".exe");
        }

        /// <summary>
        /// 设置注册表实现 开机自动启动
        /// </summary>
        /// <param name="appPath">程序路径</param>
        public static void AutoStart(string appPath)
        {
            try
            {
                var name = Path.GetFileNameWithoutExtension(appPath);
                var rKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (rKey == null)
                {
                    throw new Exception(@"添加开机自启注册表异常: 注册表项 SOFTWARE\Microsoft\Windows\CurrentVersion\Run 未找到");
                }
                var reg = rKey.GetValue(name);
                if (reg != null)
                {
                    return;
                }
                rKey.SetValue(name, "\"" + appPath + "\"");

            }
            catch (Exception e)
            {
                throw new Exception("添加开机自启注册表异常:" + e);
            }
        }

        /// <summary>
        /// 删除当前程序开机自启
        /// </summary>
        public static void UnAutoStart()
        {
            UnAutoStart(Assembly.GetEntryAssembly().GetName().Name);
        }

        /// <summary>
        /// 删除注册表实现 解除开机自动启动
        /// </summary>
        /// <param name="appName">程序名称(不带后缀)</param>
        public static void UnAutoStart(string appName)
        {
            try
            {
                var rKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (rKey == null)
                {
                    throw new Exception(@"删除开机自启注册表异常: 注册表项 SOFTWARE\Microsoft\Windows\CurrentVersion\Run 未找到");
                }
                rKey.DeleteValue(appName, false);
            }
            catch (Exception e)
            {
                throw new Exception("删除开机自启注册表异常:" + e);
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
                foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ips.Add(ip.ToString());
                    }
                }
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
            Application.Restart();
            Environment.Exit(0);
        }

        /// <summary>
        /// 简单记录日志
        /// </summary>
        /// <param name="content"></param>
        public static void Error(string content)
        {
            if (!Directory.Exists("ServiceCardReadLogs"))
            {
                Directory.CreateDirectory("ServiceCardReadLogs");
            }
            File.AppendAllText("ServiceCardReadLogs\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", DateTime.Now + content + Environment.NewLine);
        }

    }
}
