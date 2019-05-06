using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;

namespace ServiceVoice
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
        private static readonly string ConfigPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{Assembly.GetEntryAssembly().GetName().Name}\\Config.ini";

        /// <summary>
        /// ini配置节大小
        /// </summary>
        private const int IniSize = 524288;

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
                File.Create(iniFilePath).Dispose();
            }
            if (WritePrivateProfileString(section, key, value, iniFilePath)) return true;
            var errorCode = Marshal.GetLastWin32Error();
            throw new Exception($"ini写入 section:{section} key:{key} value:{value} iniFilePath:{iniFilePath} 失败,错误代码:{errorCode}!");
        }
        #endregion

        #endregion

        #region INI泛型读写序列化底层

        /// <summary>
        /// 将JSON数据转化为对应的类型  
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>转换后的对象</returns>
        private static T JsonToObj<T>(string jsonStr)
        {
            //单独的时间格式不支持反序列化 这里直接转时间格式
            if (typeof(T) == typeof(DateTime))
            {
                return (T)(object)Convert.ToDateTime(jsonStr);
            }
            return string.IsNullOrEmpty(jsonStr) ? default(T) : new JavaScriptSerializer().Deserialize<T>(jsonStr);
        }

        /// <summary>
        /// 将对应的类型转化为JSON字符串
        /// </summary>
        /// <param name="jsonObject">要转换的类型</param>
        /// <returns>json字符串</returns>
        private static string ObjToJson(object jsonObject)
        {
            //单独的时间格式不支持反序列化 这里直接转string
            if (jsonObject is DateTime)
            {
                return jsonObject.ToString();
            }
            return new JavaScriptSerializer().Serialize(jsonObject);
        }

        #endregion

        #region 封装读写

        /// <summary>
        /// 读取泛型类型
        /// </summary>
        /// <typeparam name="T">读取的类型</typeparam>
        /// <param name="key">配置键</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns>配置值</returns>
        public static T Rini<T>(string key, string configPath = DefaultPath, string endpoint = DefaultEndpoint)
        {
            if (configPath == DefaultPath)
            {
                configPath = ConfigPath;
            }
            return JsonToObj<T>(ReadIniData(endpoint, key, string.Empty, configPath));
        }

        /// <summary>
        /// 读取字符串类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns>配置值</returns>
        public static string Rini(string key, string configPath = DefaultPath, string endpoint = DefaultEndpoint)
        {
            if (configPath == DefaultPath)
            {
                configPath = ConfigPath;
            }
            return ReadIniData(endpoint, key, string.Empty, configPath);
        }

        /// <summary>
        /// 写入所有类型
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="endpoint">终结点(默认root)</param>
        /// <returns></returns>
        public static bool Wini(string key, object value, string configPath = DefaultPath, string endpoint = DefaultEndpoint)
        {
            if (configPath == DefaultPath)
            {
                configPath = ConfigPath;
            }
            return WriteIniData(endpoint, key, ObjToJson(value), configPath);
        }

        #endregion
    }
}
