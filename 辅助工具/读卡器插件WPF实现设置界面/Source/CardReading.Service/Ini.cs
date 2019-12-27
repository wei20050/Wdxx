using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CardReading.Service
{
    /// <summary>
    /// INI文件操作核心
    /// 配置默认路径 C:\Users\{用户名}\AppData\Local\{程序集名}\Config.ini
    /// </summary>
    public class Ini
    {
        #region INI读写底层

        /// <summary>
        /// ini文件路径
        /// </summary>
        private static readonly string ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Config.ini";

        /// <summary>
        /// ini配置节大小
        /// </summary>
        public static uint IniSize = 524288;

        /// <summary>
        /// 默认路径
        /// </summary>
        private const string DefaultPath = "DefaultPath";

        /// <summary>
        /// 默认终结点
        /// </summary>
        private const string DefaultEndpoint = "Default";

        #region API函数声明

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string lpAppName,
            string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            uint nSize,
            string lpFileName);

        #endregion

        #region 读Ini文件

        private static string ReadIniData(string section, string key, string noText, string iniFilePath)
        {
            if (!File.Exists(iniFilePath)) return string.Empty;
            var temp = new StringBuilder((int)IniSize);
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
                File.Create(iniFilePath).Dispose();
            }
            if (WritePrivateProfileString(section, key, value, iniFilePath)) return true;
            var errorCode = Marshal.GetLastWin32Error();
            throw new Exception("ini写入 section:" + section + " key:" + key + " value:" + value + " iniFilePath:" +
                                iniFilePath + " 失败,错误:" + errorCode);
        }

        #endregion

        #endregion

        #region 封装读写

        /// <summary>
        /// 读取字符串类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>配置值</returns>
        public static string Rini(string key)
        {
            return Rini(key, DefaultPath, DefaultEndpoint);
        }

        /// <summary>
        /// 读取字符串类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="configPath">配置文件路径</param>
        /// <returns>配置值</returns>
        public static string Rini(string key, string configPath)
        {
            return Rini(key, configPath, DefaultEndpoint);
        }

        /// <summary>
        /// 读取字符串类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns>配置值</returns>
        public static string Rini(string key, string configPath, string endpoint)
        {
            if (configPath == DefaultPath)
            {
                configPath = ConfigPath;
            }
            return ReadIniData(endpoint, key, string.Empty, configPath);
        }


        /// <summary>
        /// 写入字符串类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <returns></returns>
        public static bool Wini(string key, string value)
        {
            return Wini(key, value, DefaultPath, DefaultEndpoint);
        }

        /// <summary>
        /// 写入字符串类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <param name="configPath">配置文件路径</param>
        /// <returns></returns>
        public static bool Wini(string key, string value, string configPath)
        {
            return Wini(key, value, configPath, DefaultEndpoint);
        }

        /// <summary>
        /// 写入字符串类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns></returns>
        public static bool Wini(string key, string value, string configPath, string endpoint)
        {
            if (configPath == DefaultPath)
            {
                configPath = ConfigPath;
            }
            return WriteIniData(endpoint, key, value, configPath);
        }

        #endregion
    }
}
