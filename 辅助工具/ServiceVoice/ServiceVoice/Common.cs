﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;
using Microsoft.Win32;

namespace ServiceVoice
{


    /// <summary>
    /// 公共方法核心
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
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
        /// 设置当前程序开机自启
        /// </summary>
        public static void AutoStart()
        {
            AutoStart(AppDomain.CurrentDomain.BaseDirectory + AppName + ".exe");
        }

        /// <summary>
        /// 设置注册表实现 开机自动启动
        /// </summary>
        /// <param name="appPath">程序路径</param>
        public static void AutoStart(string appPath)
        {
            try
            {
                var rKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (rKey == null)
                {
                    throw new Exception(@"添加开机自启注册表异常: 注册表项 SOFTWARE\Microsoft\Windows\CurrentVersion\Run 未找到");
                }
                rKey.SetValue(Path.GetFileNameWithoutExtension(appPath), "\"" + appPath + "\"");

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
            UnAutoStart(AppName);
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
            if (!Directory.Exists("ServiceVoiceLogs"))
            {
                Directory.CreateDirectory("ServiceVoiceLogs");
            }
            File.AppendAllText("ServiceVoiceLogs\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", DateTime.Now + content + Environment.NewLine);
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
