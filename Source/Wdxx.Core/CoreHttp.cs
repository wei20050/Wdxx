using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Wdxx.Core
{

    /// <summary>
    /// Http通信核心 Get Post
    /// </summary>
    public static class CoreHttp
    {

        /// <summary>
        /// Http Get请求(返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static T Get<T>(string httpUri)
        {
            return HttpSend<T>(httpUri, "GET");
        }

        /// <summary>
        /// Http Get请求(返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static string Get(string httpUri)
        {
            return HttpSend(httpUri, "GET");
        }

        /// <summary>
        /// Post请求(返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static T Post<T>(string httpUri)
        {
            return HttpSend<T>(httpUri, "POST");
        }

        /// <summary>
        /// Post请求(返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static string Post(string httpUri)
        {
            return HttpSend(httpUri, "POST");
        }

        /// <summary>
        /// Post请求(匿名对象 返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="postData">请求参数 匿名类型代替的JSON对象 例:var postData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static T Post<T>(string httpUri, object postData)
        {
            var data = ObjToJson(postData);
            return HttpSend<T>(httpUri, "POST", data);
        }

        /// <summary>
        /// Post请求(匿名对象 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="postData">请求参数 匿名类型代替的JSON对象 例:var postData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static string Post(string httpUri, object postData)
        {
            var data = ObjToJson(postData);
            return HttpSend(httpUri, "POST", data);
        }

        /// <summary>
        /// Post请求(字符串数据 返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="postData">请求参数 例:{"参数名": "参数值"} C#格式(@"{""参数名"":""参数值""}")</param>
        /// <returns></returns>
        public static T Post<T>(string httpUri, string postData)
        {
            return HttpSend<T>(httpUri, "POST", postData);
        }

        /// <summary>
        /// Post请求(字符串数据 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="postData">请求参数 例:{"参数名": "参数值"} C#格式(@"{""参数名"":""参数值""}")</param>
        /// <returns></returns>
        public static string Post(string httpUri, string postData)
        {
            return HttpSend(httpUri,"POST", postData);
        }
        

        /// <summary>
        /// Http发送请求 返回泛型
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <returns></returns>
        public static T HttpSend<T>(string httpUri, string method)
        {
            return HttpSend<T>(httpUri, method, string.Empty);
        }

        /// <summary>
        /// Http发送请求 返回字符串
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <returns></returns>
        public static string HttpSend(string httpUri, string method)
        {
            return HttpSend(httpUri, method, string.Empty);
        }

        /// <summary>
        /// Http发送请求 返回泛型
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <param name="postData">请求参数 例:{"value": "post"} C#格式(@"{""value"":""post""}")</param>
        /// <returns></returns>
        public static T HttpSend<T>(string httpUri, string method, string postData)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)HttpSend(httpUri, method, postData);
            }
            return JsonToObj<T>(HttpSend(httpUri, method, postData));
        }

        /// <summary>
        /// 将json字符串中的时间戳加上当前时区
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string JsonTimeAddZone(string jsonStr)
        {
            return Regex.Replace(jsonStr, @"\\/Date\((\d+)\)\\/", match =>
            {
                var zone = DateTime.Now.ToString("zz00");
                var math = match.Groups[1].Value;
                return @"\/Date(" + math + zone + @")\/";
            });
        }

        /// <summary>
        /// 将json字符串中的时间戳转换成字符串时间格式
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string JsonTime(string jsonStr)
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
        /// 将json字符串中带时区的时间戳转换成字符串时间格式
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string JsonTimeZone(string jsonStr)
        {
            return Regex.Replace(jsonStr, @"\\/Date\((\d+)\+(\d+)\)\\/", match =>
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
            //这里把json中的时间戳转换成时间字符串 并且改成当前时区
            jsonStr = JsonTime(jsonStr);
            //这里把json中带时区的时间戳转换成时间字符串 并且改成当前时区
            jsonStr = JsonTimeZone(jsonStr);
            return string.IsNullOrEmpty(jsonStr) ? default(T) : JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <summary>
        /// 将对应的类型转化为JSON字符串
        /// </summary>
        /// <param name="jsonObject">要转换的类型</param>
        /// <returns>json字符串</returns>
        public static string ObjToJson(object jsonObject)
        {
            //这里是原序列化之后的json
            var jsonstr = JsonConvert.SerializeObject(jsonObject);
            //这里处理掉无法反序列化的构造(wcf自动创建的实体会出现这个问题)
            jsonstr = jsonstr.Replace("\"ExtensionData\":{},", string.Empty);
            //为json字符串时间戳补上时区
            jsonstr = JsonTimeAddZone(jsonstr);
            return jsonstr;
        }
        
        /// <summary>
        /// Http发送请求(核心)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <param name="postData">请求参数 例:{"value": "post"} C#格式(@"{""value"":""post""}")</param>
        /// <returns></returns>
        private static string HttpSend(string httpUri, string method, string postData)
        {
            try
            {
                string result;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(httpUri);
                if (method != "GET")
                {
                    //这个在非GET的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据做返回数据
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                }
                httpWebRequest.Method = method;
                httpWebRequest.ContentType = "application/json";
                if (!string.IsNullOrEmpty(postData))
                {
                    var data = Encoding.UTF8.GetBytes(postData);
                    httpWebRequest.ContentLength = data.Length;
                    var outStream = httpWebRequest.GetRequestStream();
                    outStream.Write(data, 0, data.Length);
                    outStream.Close();
                }
                var webResponse = httpWebRequest.GetResponse();
                var httpWebResponse = (HttpWebResponse)webResponse;
                var stream = httpWebResponse.GetResponseStream();
                if (stream != null)
                {
                    var streamReader = new StreamReader(stream, Encoding.GetEncoding("UTF-8"));
                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                    stream.Close();
                }
                else
                {
                    result = string.Empty;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("HttpErr method:" + method + " uri:" + httpUri + " postData:" + postData + "err:" + ex);
            }
        }
    }
}
