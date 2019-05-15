using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Wdxx.Core
{
    /// <summary>
    /// 转换核心
    /// </summary>
    public static class CoreConvert
    {

        /// <summary>
        /// 将json字符串中的事件戳转换成字符串时间格式
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        private static string JsonTime(string jsonStr)
        {
            return Regex.Replace(jsonStr, @"\\/Date\((\d+)\)\\/", match =>
            {
                var dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            });
        }

        /// <summary>
        /// 将JSON数据转化为对应的类型  
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static T JsonToObj<T>(string jsonStr)
        {
            return string.IsNullOrEmpty(jsonStr) ? default(T) : new JavaScriptSerializer().Deserialize<T>(jsonStr);
        }

        /// <summary>
        /// 将对应的类型转化为JSON字符串
        /// </summary>
        /// <param name="jsonObject">要转换的类型</param>
        /// <returns>json字符串</returns>
        public static string ObjToJson(object jsonObject)
        {
            //这里是原序列化之后的json
            var jsonstr = new JavaScriptSerializer().Serialize(jsonObject);
            //这里处理掉无法反序列化的构造(wcf自动创建的实体会出现这个问题)
            jsonstr = jsonstr.Replace("\"ExtensionData\":{},", string.Empty);
            //这里把json中时间戳转换成时间字符串 并且改成当前时区
            jsonstr = JsonTime(jsonstr);
            return jsonstr;
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
