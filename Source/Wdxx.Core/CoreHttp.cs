using System;
using System.IO;
using System.Net;
using System.Text;

namespace Wdxx.Core
{

    /// <summary>
    /// Http通信核心
    /// </summary>
    public static class CoreHttp
    {

        /// <summary>
        /// Get请求(返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static T Get<T>(string httpUri)
        {
            return HttpSend<T>(httpUri, "GET");
        }

        /// <summary>
        /// Get请求(返回字符串)
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
            return HttpSend<T>(httpUri, "POST", postData);
        }

        /// <summary>
        /// Post请求(匿名对象 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="postData">请求参数 匿名类型代替的JSON对象 例:var postData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static string Post(string httpUri, object postData)
        {
            return HttpSend(httpUri, "POST", postData);
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
        /// Delete请求(返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static T Delete<T>(string httpUri)
        {
            return HttpSend<T>(httpUri, "DELETE");
        }

        /// <summary>
        /// Delete请求(返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static string Delete(string httpUri)
        {
            return HttpSend(httpUri, "DELETE");
        }

        /// <summary>
        /// Delete请求(匿名对象 返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 匿名类型代替的JSON对象 例:var PutData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static T Delete<T>(string httpUri, object putData)
        {
            return HttpSend<T>(httpUri, "DELETE", putData);
        }

        /// <summary>
        /// Delete请求(匿名对象 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 匿名类型代替的JSON对象 例:var PutData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static string Delete(string httpUri, object putData)
        {
            return HttpSend(httpUri, "DELETE", putData);
        }

        /// <summary>
        /// Delete请求(字符串数据 返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 例:{"参数名": "参数值"} C#格式(@"{""参数名"":""参数值""}")</param>
        /// <returns></returns>
        public static T Delete<T>(string httpUri, string putData)
        {
            return HttpSend<T>(httpUri, "DELETE", putData);
        }

        /// <summary>
        /// Delete请求(字符串数据 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 例:{"参数名": "参数值"} C#格式(@"{""参数名"":""参数值""}")</param>
        /// <returns></returns>
        public static string Delete(string httpUri, string putData)
        {
            return HttpSend(httpUri, "DELETE", putData);
        }

        /// <summary>
        /// Put请求(返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static T Put<T>(string httpUri)
        {
            return HttpSend<T>(httpUri, "PUT");
        }

        /// <summary>
        /// Put请求(返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <returns></returns>
        public static string Put(string httpUri)
        {
            return HttpSend(httpUri, "PUT");
        }

        /// <summary>
        /// Put请求(匿名对象 返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 匿名类型代替的JSON对象 例:var PutData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static T Put<T>(string httpUri, object putData)
        {
            return HttpSend<T>(httpUri, "PUT", putData);
        }

        /// <summary>
        /// Put请求(匿名对象 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 匿名类型代替的JSON对象 例:var PutData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static string Put(string httpUri, object putData)
        {
            return HttpSend(httpUri, "PUT", putData);
        }

        /// <summary>
        /// Put请求(字符串数据 返回泛型)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 例:{"参数名": "参数值"} C#格式(@"{""参数名"":""参数值""}")</param>
        /// <returns></returns>
        public static T Put<T>(string httpUri, string putData)
        {
            return HttpSend<T>(httpUri, "PUT", putData);
        }

        /// <summary>
        /// Put请求(字符串数据 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 例:{"参数名": "参数值"} C#格式(@"{""参数名"":""参数值""}")</param>
        /// <returns></returns>
        public static string Put(string httpUri, string putData)
        {
            return HttpSend(httpUri, "PUT", putData);
        }

        /// <summary>
        /// Http发送请求 返回泛型
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <returns></returns>
        public static T HttpSend<T>(string httpUri, string method)
        {
            return HttpSend<T>(httpUri, method, null);
        }

        /// <summary>
        /// Http发送请求 返回字符串
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <returns></returns>
        public static string HttpSend(string httpUri, string method)
        {
            return HttpSend(httpUri, method, null);
        }

        /// <summary>
        /// Http发送请求 字符串参数 返回泛型
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <param name="httpData">请求参数 例:{"value": "http"} C#格式(@"{""value"":""http""}")</param>
        /// <returns></returns>
        public static T HttpSend<T>(string httpUri, string method, string httpData)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)HttpSend(httpUri, method, httpData);
            }
            return JsonToObj<T>(HttpSend(httpUri, method, httpData));
        }

        /// <summary>
        /// Http发送请求 匿名类型参数 返回字符串
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <param name="httpData">请求参数 匿名类型代替的JSON对象 例:var httpData = new {参数名1 = 参数值1,参数名2 = 参数值2}</param>
        /// <returns></returns>
        public static string HttpSend(string httpUri, string method, object httpData)
        {
            var data = ObjToJson(httpData);
            return HttpSend(httpUri, method, data);
        }

        /// <summary>
        /// Http发送请求 匿名类型参数 返回泛型
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <param name="httpData">请求参数 匿名类型代替的JSON对象 例:var httpData = new {参数名1 = 参数值1,参数名2 = 参数值2}</param>
        /// <returns></returns>
        public static T HttpSend<T>(string httpUri, string method, object httpData)
        {
            var data = ObjToJson(httpData);
            if (typeof(T) == typeof(string))
            {
                return (T)(object)HttpSend(httpUri, method, data);
            }
            return JsonToObj<T>(HttpSend(httpUri, method, data));
        }

        /// <summary>
        /// Http发送请求(核心)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <param name="httpData">请求参数 例:{"value": "HttpSend"} C#格式(@"{""value"":""HttpSend""}")</param>
        /// <returns></returns>
        private static string HttpSend(string httpUri, string method, string httpData)
        {
            try
            {
                string result;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(httpUri);
                httpWebRequest.Method = method;
                httpWebRequest.ContentType = "application/json";
                if (method != "GET")
                {
                    //这个在非GET的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据做返回数据
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                    if (!string.IsNullOrEmpty(httpData))
                    {
                        var data = Encoding.UTF8.GetBytes(httpData);
                        httpWebRequest.ContentLength = data.Length;
                        var outStream = httpWebRequest.GetRequestStream();
                        outStream.Write(data, 0, data.Length);
                        outStream.Close();
                    }
                    else
                    {
                        httpWebRequest.ContentLength = 0;
                    }
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
                throw new Exception("HttpErr method:" + method + " uri:" + httpUri + " postData:" + httpData + "err:" + ex);
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
            string json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            return json;
        }

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
    }
}
