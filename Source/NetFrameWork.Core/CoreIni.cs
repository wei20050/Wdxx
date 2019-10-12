using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace NetFrameWork.Core
{
    /// <summary>
    /// INI文件操作核心
    /// 配置默认路径 C:\Users\{用户名}\AppData\Local\AppName\Config.ini或或当前目录下\Config.ini
    /// </summary>
    public static class CoreIni
    {

        /// <summary>
        /// 默认ini文件路径
        /// </summary>
        private static readonly string DefaultPath;

        /// <summary>
        /// ini配置节大小
        /// </summary>
        public static uint IniSize = 524288;

        /// <summary>
        /// 默认终结点
        /// </summary>
        private const string DefaultEndpoint = "Default";

        static CoreIni()
        {
            var exe = Assembly.GetEntryAssembly();
            var defPath = exe == null ? AppDomain.CurrentDomain.BaseDirectory : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), exe.GetName().Name);
            DefaultPath = Path.Combine(defPath, "Config.ini");
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T">读取的类型</typeparam>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns>配置值</returns>
        public static T ReadIni<T>(string key, object defaultValue = null, string configPath = "", string endpoint = DefaultEndpoint)
        {
            if (configPath == "")
            {
                configPath = DefaultPath;
            }
            var defValue = string.Empty;
            if (defaultValue != null)
            {
                defValue = CoreConvert.ObjToJson(defaultValue);
            }
            return CoreConvert.JsonToObj<T>(ReadIni(key, defValue, configPath, endpoint));
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue"></param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns>配置值</returns>
        public static string ReadIni(string key, string defaultValue = "", string configPath = "", string endpoint = DefaultEndpoint)
        {
            if (string.IsNullOrEmpty(configPath))
            {
                configPath = DefaultPath;
            }
            if (!File.Exists(configPath)) return string.Empty;
            var temp = new StringBuilder((int)IniSize);
            GetPrivateProfileString(endpoint, key, defaultValue, temp, IniSize, configPath);
            return temp.ToString();
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认Default)</param>
        /// <returns></returns>
        public static bool WriteIni(string key, object value, string configPath = "", string endpoint = DefaultEndpoint)
        {
            if (string.IsNullOrEmpty(configPath))
            {
                configPath = DefaultPath;
            }
            var dir = Path.GetDirectoryName(configPath);
            if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(configPath))
            {
                File.Create(configPath).Dispose();
            }
            if (WritePrivateProfileString(endpoint, key, CoreConvert.ObjToJson(value), configPath)) return true;
            var errorCode = Marshal.GetLastWin32Error();
            throw new Exception("ini写入 section:" + endpoint + " key:" + key + " value:" + value + " iniFilePath:" +
                                configPath + " 失败,错误:" + errorCode);
        }


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
    }
}
