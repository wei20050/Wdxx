using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ClickOnceHelp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 检测当前目录有没有Setup.exe
        /// </summary>
        /// <returns></returns>
        private static bool JcExe(out string info)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Setup.exe"))
            {
                info = "当前目录已检测到Setup.exe 可执行写入动作!";
                return true;
            }
            info = "当前目录没有检测到Setup.exe 请确认目录是否正确!";
            return false;
        }
        private void BtnUrlSave_Click(object sender, EventArgs e)
        {
            Url = TxtUrl.Text;
            if (!JcExe(out var info))
            {
                MessageBox.Show($@"写入失败!{Environment.NewLine}{info}", @"提示");
                return;
            }
            try
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe",
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                p.Start();
                p.StandardInput.AutoFlush = true;
                p.StandardInput.WriteLine($"Setup.exe -url={TxtUrl.Text}");
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();
                p.Close();
                MessageBox.Show(@"写入安装地址成功!", @"提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"写入失败!{Environment.NewLine}{ex}", @"提示");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            JcExe(out var info);
            Text = info;
            TxtUrl.Text = string.IsNullOrEmpty(Url)?"http://localhost/":Url;
        }

        private string Url
        {
            get => CoreIni.Rini(nameof(Url));
            set => CoreIni.Wini(nameof(Url),value);
        }
    }
    /// <summary>
    /// INI文件操作核心
    /// 配置默认路径 C:\Users\{用户名}\AppData\Local\{程序集名}\Config.ini
    /// </summary>
    public class CoreIni
    {
        #region INI读写底层
        /// <summary>
        /// ini文件路径
        /// </summary>
        private static readonly string ConfigPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{Assembly.GetEntryAssembly().GetName().Name}\\Config.ini");
        /// <summary>
        /// ini配置节大小
        /// </summary>
        private const int IniSize = 524288;

        #region API函数声明
        [DllImport("kernel32")]
        //返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);
        [DllImport("kernel32")]
        //返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region 读Ini文件
        private static string ReadIniData(string section, string key, string noText, string iniFilePath)
        {
            if (!File.Exists(iniFilePath)) return string.Empty;
            var temp = new StringBuilder(IniSize);
            GetPrivateProfileString(section, key, noText, temp, IniSize, iniFilePath);
            return temp.ToString();
        }
        #endregion

        #region 写Ini文件
        private static bool WriteIniData(string section, string key, string value, string iniFilePath)
        {
            var dir = Path.GetDirectoryName(iniFilePath);
            if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(iniFilePath))
            {
                File.WriteAllText(iniFilePath, $@"[{section}]");
            }
            try
            {
                var opStation = WritePrivateProfileString(section, key, value, iniFilePath);
                return opStation != 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #endregion

        #region 封装读写

        /// <summary>
        /// 字符串类型的读取
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns>配置值</returns>
        public static string Rini(string key, string configPath = "", string endpoint = "root")
        {
            if (configPath == "")
            {
                configPath = ConfigPath;
            }
            return ReadIniData(endpoint, key, string.Empty, configPath);
        }

        /// <summary>
        /// 字符串类型的写入
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns></returns>
        public static bool Wini(string key, string value, string configPath = "", string endpoint = "root")
        {
            if (configPath == "")
            {
                configPath = ConfigPath;
            }
            return WriteIniData(endpoint, key, value, configPath);
        }

        #endregion
    }
}
