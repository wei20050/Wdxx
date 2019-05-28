using System;
using System.IO;
using System.Text;

namespace Wdxx.Core
{
    /// <summary>
    /// 转换核心
    /// </summary>
    public static class CoreConvert
    {

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static T JsonToObj<T>(string jsonStr)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonStr)))
            {
                var deseralizer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                return (T)deseralizer.ReadObject(ms);
            }
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <param name="type">要转换的类型</param>
        /// <returns>转换后的对象</returns>
        public static object JsonToObj(string jsonStr, Type type)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonStr)))
            {
                var deseralizer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(type);
                return deseralizer.ReadObject(ms);
            }
        }

        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <param name="jsonObject">要转换的类型</param>
        /// <returns>json字符串</returns>
        public static string ObjToJson(object jsonObject)
        {
            var js = new System.Runtime.Serialization.Json.DataContractJsonSerializer(jsonObject.GetType());
            var msObj = new MemoryStream();
            js.WriteObject(msObj, jsonObject);
            msObj.Position = 0;
            var sr = new StreamReader(msObj, Encoding.UTF8);
            var json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            return json;
        }

        /// <summary>
        /// 文件转Base64字符串
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>Base64字符串</returns>
        public static string FileToBase64(string fileName)
        {
            var fs = File.OpenRead(fileName);
            var br = new BinaryReader(fs);
            var bt = br.ReadBytes(Convert.ToInt32(fs.Length));
            fs.Close();
            fs.Dispose();
            return Convert.ToBase64String(bt);
        }

        /// <summary>
        /// Base64字符串转文件
        /// </summary>
        /// <param name="base64Str">Base64字符串</param>
        /// <param name="fileName">文件路径</param>
        public static void Base64ToFile(string base64Str, string fileName)
        {
            var contents = Convert.FromBase64String(base64Str);
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(contents, 0, contents.Length);
                fs.Flush();
            }
        }
    }
}
