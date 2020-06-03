using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global
namespace NetFrameWork.Core2
{
    /// <summary>
    /// WebApi调用核心
    /// </summary>
    public class CoreWebApi
    {

        /// <summary>
        /// WebApi调用核心
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">url地址</param>
        /// <param name="p">url参数</param>
        /// <param name="data">Body数据</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="con">控制器名</param>
        /// <returns></returns>
        public static T Send<T>(string url, string p = null, object data = null, int timeout = 60, [CallerMemberName]string con = "")
        {
            p = p == null ? string.Empty : "/" + p;
            if (con.StartsWith("Get"))
            {
                url = url.TrimEnd('/') + "/" + con.TrimStart("Get".ToArray()) + p;
                return Get<T>(url, timeout);
            }
            if (con.StartsWith("Post"))
            {
                url = url.TrimEnd('/') + "/" + con.TrimStart("Post".ToArray()) + p;
                return Post<T>(url, data, timeout);
            }
            if (con.StartsWith("Put"))
            {
                url = url.TrimEnd('/') + "/" + con.TrimStart("Put".ToArray()) + p;
                return Put<T>(url, data, timeout);
            }
            if (con.StartsWith("Delete"))
            {
                url = url.TrimEnd('/') + "/" + con.TrimStart("Delete".ToArray()) + p;
                return Delete<T>(url, timeout);
            }
            return default;
        }

        private static T Get<T>(string url, int timeout)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default;
            }
        }

        private static T Post<T>(string url, object data, int timeout)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                var response = client.PostAsJsonAsync(url, data).Result;
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default;
            }
        }

        private static T Put<T>(string url, object data, int timeout)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                var response = client.PutAsJsonAsync(url, data).Result;
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default;
            }
        }

        private static T Delete<T>(string url, int timeout)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                var response = client.DeleteAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default;
            }
        }

    }
}
