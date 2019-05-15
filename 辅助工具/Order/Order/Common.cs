using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows;
using Microsoft.Win32;
// ReSharper disable PossibleNullReferenceException

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
        /// 服务地址
        /// </summary>
        public static string ServiceUri
        {
            get
            {
                return Ini.Rini("ServiceUri");
            }
            set
            {
                Ini.Wini("ServiceUri", value);
            }
        }

        /// <summary>
        /// 拉取预约数据
        /// </summary>
        /// <param name="time"></param>
        public static string PullOrderService(string time)
        {
            try
            {
                var orderInfo = HttpPost(ServiceUri + "/PullOrderService", @"{ ""time"":""" + time + @"""}");
                Log("成功获取到预约数据:" + orderInfo);
                return string.Empty;
            }
            catch (Exception e)
            {
                Log("拉取预约数据异常:" + e);
                return e.Message;
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
            if (!Directory.Exists("OrderLogs"))
            {
                Directory.CreateDirectory("OrderLogs");
            }
            File.AppendAllText("OrderLogs\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", DateTime.Now + content + Environment.NewLine);
        }

        /// <summary>
        /// Http Post请求(字符串数据 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="postData">请求参数 例:{"参数名": "参数值"} C#格式(@"{""参数名"":""参数值""}")</param>
        /// <returns></returns>
        public static string HttpPost(string httpUri, string postData)
        {
            string ret;
            if (HttpPost(httpUri, postData, out ret))
            {
                return ret;
            }
            throw new Exception("Http Post请求 发生异常:" + ret);
        }

        /// <summary>
        /// Http Post请求(核心)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="postData">请求参数 例:{"value": "post"} C#格式(@"{""value"":""post""}")</param>
        /// <param name="result">返回结果</param>
        /// <returns></returns>
        private static bool HttpPost(string httpUri, string postData, out string result)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(httpUri);
                //这个在Post的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据做返回数据
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                var data = Encoding.UTF8.GetBytes(postData ?? string.Empty);
                httpWebRequest.ContentLength = data.Length;
                var outStream = httpWebRequest.GetRequestStream();
                outStream.Write(data, 0, data.Length);
                outStream.Close();
                var webResponse = httpWebRequest.GetResponse();
                var httpWebResponse = (HttpWebResponse)webResponse;
                var stream = httpWebResponse.GetResponseStream();
                if (stream != null)
                {
                    var streamReader = new StreamReader(stream, Encoding.GetEncoding("UTF-8"));
                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                    stream.Close();
                }
                else
                {
                    result = string.Empty;
                }
                return true;
            }
            catch (Exception ex)
            {
                result = "HttpPostErr uri:" + httpUri + " postData:" + postData + "err:" + ex;
                return false;
            }
        }
    }
}
