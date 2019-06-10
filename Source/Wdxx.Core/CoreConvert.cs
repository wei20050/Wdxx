﻿using System;
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
        /// 将json字符串中的时间戳加上当前时区
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        private static string JsonTimeAddZone(string jsonStr)
        {
            return Regex.Replace(jsonStr, @"\\/Date\((\d+)\)\\/", match =>
            {
                var zone = DateTime.Now.ToString("zz00");
                var math = match.Groups[1].Value;
                return @"\/Date(" + math + zone + @")\/";
            });
        }

        /// <summary>
        /// 将json字符串中带时区的时间戳转换成字符串时间格式
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        private static string JsonTimeZone(string jsonStr)
        {
            return Regex.Replace(jsonStr, @"\\/Date\((\d+)\+(\d+)\)\\/", match =>
            {
                //拿到的时间戳
                var ts = match.Groups[1].Value;
                //时间戳double
                var datetime = Convert.ToDouble(ts);
                //时区秒数double
                var zone = Convert.ToDouble(match.Groups[2].Value) * 60 * 60;
                //这里如果判断到时间戳是13位带毫秒的
                if (ts.Length == 13)
                {
                    zone *= 1000;
                }
                var date = datetime + zone;
                return @"\/Date(" + date + @")\/";
            });
        }

        /// <summary>
        /// 将JSON字符串转化为对应类型的对象
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static T JsonToObj<T>(string json)
        {
            //这里把json中带时区的时间戳转换成准确时间
            json = JsonTimeZone(json);
            return string.IsNullOrEmpty(json) ? default(T) : new JavaScriptSerializer().Deserialize<T>(json);
        }

        /// <summary>
        /// 将JSON字符串转化为object对象
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static object JsonToObj(string json)
        {
            //这里把json中带时区的时间戳转换成准确时间
            json = JsonTimeZone(json);
            return new JavaScriptSerializer().DeserializeObject(json);
        }

        /// <summary>
        /// 将任意类型对象转化为JSON字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>json字符串</returns>
        public static string ObjToJson(object obj)
        {
            //这里是原序列化之后的json
            var jsonstr = new JavaScriptSerializer().Serialize(obj);
            //这里处理掉无法反序列化的构造(wcf自动创建的实体会出现这个问题) 暂时不用wcf注释掉
            //jsonstr = jsonstr.Replace("\"ExtensionData\":{},", string.Empty);
            //为json字符串时间戳补上时区
            return JsonTimeAddZone(jsonstr);
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
