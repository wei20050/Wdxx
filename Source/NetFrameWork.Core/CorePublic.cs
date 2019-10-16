using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core
{

    /// <summary>
    /// 公共方法核心
    /// </summary>
    public static class CorePublic
    {

        #region 系统相关

        /// <summary>
        /// 重启程序
        /// </summary>
        public static void Restart()
        {
            Application.Restart();
            Environment.Exit(0);
        }

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
        /// 判断系统是否已启动
        /// </summary>
        public static bool IsStart()
        {
            for (var i = 0; i < 8; i++)
            {
                //获取指定的进程名
                var myProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                //如果获取到的进程只有一个 判断没有重复启动
                if (myProcesses.Length <= 1) return false;
                System.Threading.Thread.Sleep(168);
            }
            //连续八次检测到多个进程判断是重复启动了
            return true;
        }

        /// <summary>  
        /// 获取当前首个IPV4地址
        /// </summary>  
        /// <returns></returns>  
        public static string GetLocalIp()
        {
            try
            {
                //本机名
                var hostName = Dns.GetHostName();
                //本机ip组
                var ips = Dns.GetHostAddresses(hostName);
                foreach (var ip in ips)
                {
                    var ipStr = ip.ToString();
                    var ipArr = ipStr.Split('.');
                    if (ipArr.Length == 4)
                    {
                        return ipStr;
                    }
                }
                return "127.0.0.1";
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
        }

        /// <summary>
        /// 获取当前所有IPV4地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLocalIps()
        {
            try
            {
                //本机名
                var hostName = Dns.GetHostName();
                //本机ip组
                var ips = Dns.GetHostAddresses(hostName);
                var strArr = (from ip in ips select ip.ToString() into ipStr let ipArr = ipStr.Split('.') where ipArr.Length == 4 select ipStr).ToList();
                return strArr.Count == 0
                    ? new List<string> { "127.0.0.1" }
                    : strArr;
            }
            catch
            {
                return new List<string> { "127.0.0.1" };
            }
        }

        /// <summary>
        /// 执行cmd命令
        /// </summary>
        /// <param name="cmd"></param>
        public static void ExecuteCommand(string cmd)
        {
            try
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        Verb = "runas",
                        FileName = cmd,
                        Arguments = "",
                        UseShellExecute = false,
                        RedirectStandardInput = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        CreateNoWindow = true
                    }
                };
                p.Start();
                p.WaitForExit();
                p.Close();
            }
            catch (Exception ex)
            {
                CoreLog.Error(ex, "CORE_");
            }
        }

        #endregion

        #region 注册表添加与删除开机启动

        /// <summary>
        /// 设置当前程序开机自启
        /// </summary>
        public static void AutoStart()
        {
            AutoStart(AppDomain.CurrentDomain.BaseDirectory + Path.GetFileName(Application.ExecutablePath));
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
            UnAutoStart(Path.GetFileNameWithoutExtension(Application.ExecutablePath));
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

        #region 操作进程

        /// <summary>
        /// 结束进程
        /// </summary>
        public static void KillProcess(string pcTask)
        {
            //获取已开启的所有进程
            var pro = Process.GetProcesses();
            foreach (var t in pro)
            {
                if (string.Equals(t.ProcessName, pcTask, StringComparison.CurrentCultureIgnoreCase))
                {
                    //结束进程
                    t.Kill();
                }
            }
        }

        /// <summary>
        /// 杀死进程
        /// </summary>
        public static void KillProcess(string startupPath, string killFilePath)
        {
            const string handlePath = @"C:\Windows\system32\handle.exe";
            if (!File.Exists(handlePath))
            {
                var path = Path.Combine(startupPath, "handle.exe");
                File.Copy(path, handlePath);
            }

            var tool = new Process
            {
                StartInfo =
                {
                    FileName = "handle.exe",
                    Arguments = killFilePath + " /accepteula",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            tool.Start();
            tool.WaitForExit();
            var outputTool = tool.StandardOutput.ReadToEnd();
            const string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
            foreach (Match match in Regex.Matches(outputTool, matchPattern))
            {
                Process.GetProcessById(int.Parse(match.Value)).Kill();
            }
        }

        /// <summary>
        /// 运行外部应用
        /// </summary>
        /// <param name="filePath">外部程序文件路径</param>
        public static void RunApp(string filePath)
        {
            Process.Start(filePath);
        }

        /// <summary>
        /// 运行外部应用等待退出(一直等待)
        /// </summary>
        /// <param name="filePath">外部程序文件路径</param>
        public static void RunAppWaitForExit(string filePath)
        {
            RunAppWaitForExit(filePath, 0);
        }

        /// <summary>
        /// 运行外部应用等待退出
        /// </summary>
        /// <param name="filePath">外部程序文件路径</param>
        /// <param name="s">等待毫秒数(小于等于0一直等待)</param>
        public static void RunAppWaitForExit(string filePath, int s)
        {
            try
            {
                var proc = Process.Start(filePath);
                if (proc == null) return;
                if (s <= 0)
                {
                    proc.WaitForExit();
                }
                else
                {
                    proc.WaitForExit(s);
                }
                // 如果外部程序没有结束运行则强行终止之。
                if (!proc.HasExited) proc.Kill();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("运行外部应用等待退出异常:" + ex);
            }
        }

        #endregion

        #region 绝对随机

        /// <summary>
        /// 返回double随机数0-1不包括1
        /// </summary>
        /// <returns></returns>
        public static double Random()
        {
            var rd = new Random(GenerateId());
            return rd.NextDouble();
        }

        /// <summary>
        /// 返回在指定范围内的任意整数
        /// </summary>
        /// <returns></returns>
        public static int Random(int minValue, int maxValue)
        {
            var rd = new Random(GenerateId());
            return rd.Next(minValue, maxValue);
        }

        /// <summary>
        /// 获随机整数(作为Random的种子 或其他用途)
        /// </summary>
        /// <returns></returns>
        public static int GenerateId()
        {
            var buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt32(buffer, 0);
        }

        #endregion

    }
}
