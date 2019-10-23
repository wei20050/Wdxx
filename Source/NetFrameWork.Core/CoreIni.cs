using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core
{
    /// <summary>
    /// INI文件操作核心
    /// 配置默认路径 C:\Users\{用户名}\AppData\Local\AppName\Config.ini 或当前目录下\Config.ini
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
                defValue = ObjToJsonData(defaultValue);
            }
            return JsonDataToObj<T>(ReadIni(key, defValue, configPath, endpoint));
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
            if (WritePrivateProfileString(endpoint, key, ObjToJsonData(value), configPath)) return true;
            var errorCode = Marshal.GetLastWin32Error();
            throw new Exception("CoreIni.WriteIni Err", new Exception($"endpoint=>{endpoint} key=>{key} value=>{value} configPath=>{configPath}errorCode=>{errorCode}"));
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

        #region 序列化反序列化

        /// <summary>
        /// 将任意类型对象转化为数据JsonData字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>json字符串</returns>
        public static string ObjToJsonData(object obj)
        {
            if (obj is string)
            {
                return obj.ToString();
            }
            var js = new DataContractJsonSerializer(obj.GetType());
            var msObj = new MemoryStream();
            //将序列化之后的Json格式数据写入流中
            js.WriteObject(msObj, obj);
            msObj.Position = 0;
            //从0这个位置开始读取流中的数据
            var sr = new StreamReader(msObj, Encoding.UTF8);
            var json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            return json;
        }

        /// <summary>
        /// 将JsonData字符串转化为对应类型的对象
        /// </summary>
        /// <param name="jsonData">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static T JsonDataToObj<T>(string jsonData)
        {
            return (T)JsonDataToObj(jsonData, typeof(T));
        }

        /// <summary>
        /// 将JsonData字符串转化为对应类型的对象
        /// </summary>
        /// <param name="jsonData">json字符串</param>
        /// <param name="type"></param>
        /// <returns>转换后的对象</returns>
        public static object JsonDataToObj(string jsonData, Type type)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return null;
            }
            if (type == typeof(string))
            {
                return jsonData;
            }
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonData)))
            {
                var deserializer = new DataContractJsonSerializer(type);
                return deserializer.ReadObject(ms);
            }
        }

        #endregion

    }
}
