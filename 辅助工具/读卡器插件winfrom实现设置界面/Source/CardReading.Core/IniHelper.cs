using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CardReading.Core
{
    public static class IniHelper
    {

        static IniHelper()
        {
            var exe = Assembly.GetEntryAssembly();
            var appName = exe == null ? "DefaultCardReading" : exe.GetName().Name;
            ConfigPath = Path.Combine(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                appName), "CardReadingConfig.ini");
        }

        /// <summary>
        /// ini文件路径
        /// </summary>
        private static readonly string ConfigPath ;
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
        /// <summary>
        /// 默认
        /// </summary>
        private const string Default = "Default";

        /// <summary>
        /// 写入ini
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        public static void Write(string key, string value, string filePath = Default, string section = Default)
        {
            if (filePath == Default)
            {
                filePath = ConfigPath;
            }
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            if (WritePrivateProfileString(section, key, value, filePath)) return;
            var errorCode = Marshal.GetLastWin32Error();
            throw new ApplicationException($"读卡器配置初始化失败,错误代码:{errorCode}!");
        }

        /// <summary>
        /// 读取ini
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string Read(string key, string filePath = Default, string section = Default)
        {
            if (filePath == Default)
            {
                filePath = ConfigPath;
            }
            if (!File.Exists(filePath)) return string.Empty;
            var sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, string.Empty, sb, 255, filePath);
            return sb.ToString();
        }

        /// <summary>
        /// 检查ini是否有值若没有值写入默认值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        public static void CheckIni(string key, string defaultValue, string filePath, string section = Default)
        {
            if (filePath == Default)
            {
                filePath = ConfigPath;
            }
            if (string.IsNullOrEmpty(Read(key, filePath, section)))
            {
                Write(key, defaultValue, filePath, section);
            }
        }
    }
}
