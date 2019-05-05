using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Wdxx.Core
{

    /// <summary>
    /// 公共方法核心
    /// </summary>
    public static class CorePublic
    {
        #region 系统相关

        /// <summary>
        /// 应用程序目录
        /// </summary>
        /// <returns></returns>
        public static string AppPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Assembly.GetEntryAssembly().GetName().Name);
            }
        }

        /// <summary>
        /// exe所在目录
        /// </summary>
        /// <returns></returns>
        public static string ExePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
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
                var myProcesses = Process.GetProcessesByName(Assembly.GetEntryAssembly().GetName().Name);
                //如果可以获取到知道的进程名则说明已经启动
                if (myProcesses.Length <= 1) return;
                System.Threading.Thread.Sleep(168);
            }
            MessageBox.Show("程序已启动！");
            //关闭系统
            Environment.Exit(0);
        }

        /// <summary>
        /// 重启程序
        /// </summary>
        public static void Restart()
        {
            Application.Restart();
            Environment.Exit(0);
        }

        #endregion

        #region 窗体操作

        //删除菜单中的按钮
        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        //获取系统菜单句柄
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        //获取当前获得焦点窗口的句柄
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();
        //指定窗口如何显示
        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        private static extern bool ShowWindow(IntPtr hWnd, int type);

        /// <summary>
        /// 禁用当前应用程序的关闭按钮
        /// </summary>
        public static void DeleteExit()
        {
            DeleteMenu(GetSystemMenu(GetForegroundWindow(), false), 0xF060, 0);
        }

        /// <summary>
        /// 禁用当前应用程序的最大化按钮
        /// </summary>
        public static void DeleteMax()
        {
            DeleteMenu(GetSystemMenu(GetForegroundWindow(), false), 0xF030, 0);
        }

        /// <summary>
        /// <para>指定当前窗口如何显示</para>
        /// <para>参数nCmdShow：</para>
        /// <para>0：隐藏窗口并激活其他窗口。</para>
        /// <para>1：激活并显示一个窗口。如果窗口被最小化或最大化，系统将其恢复到原来的尺寸和大小。应用程序在第一次显示窗口的时候应该指定此标志。</para>
        /// <para>2：激活窗口并将其最小化。</para>
        /// <para>3：激活窗口并将其最大化。</para>
        /// <para>4：以窗口最近一次的大小和状态显示窗口。激活窗口仍然维持激活状态。</para>
        /// <para>5：在窗口原来的位置以原来的尺寸激活和显示窗口。</para>
        /// <para>6：最小化指定的窗口并且激活在Z序中的下一个顶层窗口。</para>
        /// <para>7：窗口最小化，激活窗口仍然维持激活状态。</para>
        /// <para>8：以窗口原来的状态显示窗口。激活窗口仍然维持激活状态。</para>
        /// <para>9：激活并显示窗口。如果窗口最小化或最大化，则系统将窗口恢复到原来的尺寸和位置。在恢复最小化窗口时，应用程序应该指定这个标志。</para>
        /// <para>10：依据在STARTUPINFO结构中指定的SW_FLAG标志设定显示状态，STARTUPINFO 结构是由启动应用程序的程序传递给CreateProcess函数的。</para>
        /// <para>11：在WindowNT5.0中最小化窗口，即使拥有窗口的线程被挂起也会最小化。在从其他线程最小化窗口时才使用这个参数。</para>
        /// </summary>
        /// <param name="nCmdShow">显示模式</param>
        public static void ShowWindow(int nCmdShow)
        {
            ShowWindow(GetForegroundWindow(), nCmdShow);
        }

        #endregion

        #region 注册表添加与删除开机启动

        /// <summary>
        /// 设置当前程序开机自启
        /// </summary>
        public static void AutoStart()
        {
            AutoStart(AppDomain.CurrentDomain.BaseDirectory + Assembly.GetEntryAssembly().GetName().Name + ".exe");
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
        // 关闭64位（文件系统）的操作转向
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        // 开启64位（文件系统）的操作转向
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        /// <summary>
        /// 运行外部应用
        /// </summary>
        /// <param name="filePath">外部程序文件路径</param>
        public static void RunApp(string filePath)
        {
            var oldWow64State = new IntPtr();
            try
            {
                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                {
                    // 关闭64位（文件系统）的操作转向
                    Wow64DisableWow64FsRedirection(ref oldWow64State);
                }
                Process.Start(filePath);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("运行外部应用异常:" + ex);
            }
            finally
            {
                Wow64RevertWow64FsRedirection(oldWow64State);
            }
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
            var oldWow64State = new IntPtr();
            try
            {
                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                {
                    // 关闭64位（文件系统）的操作转向
                    Wow64DisableWow64FsRedirection(ref oldWow64State);
                }
                var proc = Process.Start(filePath);
                Wow64RevertWow64FsRedirection(oldWow64State);
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
            finally
            {
                Wow64RevertWow64FsRedirection(oldWow64State);
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
        public static double Random(int minValue,int maxValue)
        {
            var rd = new Random(GenerateId());
            return rd.Next(minValue, maxValue);
        }

        /// <summary>
        /// 获取9位随机正整数(作为Random的种子 或其他用途)
        /// </summary>
        /// <returns></returns>
        public static int GenerateId()
        {
            var buffer = Guid.NewGuid().ToByteArray();
            return (int) (BitConverter.ToInt64(buffer, 0) / 999999999);
        }

        #endregion

    }
}
