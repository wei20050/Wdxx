using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

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
            var data = JsonConvert.SerializeObject(postData);
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
            var data = JsonConvert.SerializeObject(postData);
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
            var data = JsonConvert.SerializeObject(putData);
            return HttpSend<T>(httpUri, "DELETE", data);
        }

        /// <summary>
        /// Delete请求(匿名对象 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 匿名类型代替的JSON对象 例:var PutData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static string Delete(string httpUri, object putData)
        {
            var data = JsonConvert.SerializeObject(putData);
            return HttpSend(httpUri, "DELETE", data);
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
            var data = JsonConvert.SerializeObject(putData);
            return HttpSend<T>(httpUri, "PUT", data);
        }

        /// <summary>
        /// Put请求(匿名对象 返回字符串)
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="putData">请求参数 匿名类型代替的JSON对象 例:var PutData = new {参数名1 = 参数值1,参数名2 = 参数值2};</param>
        /// <returns></returns>
        public static string Put(string httpUri, object putData)
        {
            var data = JsonConvert.SerializeObject(putData);
            return HttpSend(httpUri, "PUT", data);
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
        /// Http发送请求 返回泛型
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <param name="httpData">请求参数 例:{"value": "post"} C#格式(@"{""value"":""post""}")</param>
        /// <returns></returns>
        public static T HttpSend<T>(string httpUri, string method, string httpData)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)HttpSend(httpUri, method, httpData);
            }
            return JsonConvert.DeserializeObject<T>(HttpSend(httpUri, method, httpData));
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
    }
}
