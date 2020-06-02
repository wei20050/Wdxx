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
        /// 调用Get
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">url地址</param>
        /// <param name="p">url参数</param>
        /// <param name="con">控制器名</param>
        /// <returns></returns>
        public static T Get<T>(string url, string p = null, [CallerMemberName]string con = "")
        {
            p = p == null ? string.Empty : "/" + p;
            url = url.TrimEnd('/') + "/" + con.TrimStart("Get".ToArray()) + p;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default;
            }
        }

        /// <summary>
        /// 调用Post
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="data">Body数据</param>
        /// <param name="con">控制器名</param>
        /// <returns></returns>
        public static T Post<T>(string url, object data, [CallerMemberName]string con = "")
        {
            url = url.TrimEnd('/') + "/" + con.TrimStart("Post".ToArray());
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(url, data).Result;
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default;
            }
        }

        /// <summary>
        /// 调用Put
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="data">Body数据</param>
        /// <param name="p">url参数</param>
        /// <param name="con">控制器名</param>
        /// <returns></returns>
        public static T Put<T>(string url, object data, string p = null, [CallerMemberName]string con = "")
        {
            p = p == null ? string.Empty : "/" + p;
            url = url.TrimEnd('/') + "/" + con.TrimStart("Put".ToArray()) + p;
            using (var client = new HttpClient())
            {
                var response = client.PutAsJsonAsync(url, data).Result;
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default;
            }
        }

        /// <summary>
        /// 调用Delete
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="p">url参数</param>
        /// <param name="con">控制器名</param>
        /// <returns></returns>
        public static T Delete<T>(string url, string p = null, [CallerMemberName]string con = "")
        {
            p = p == null ? string.Empty : "/" + p;
            url = url.TrimEnd('/') + "/" + con.TrimStart("Delete".ToArray()) + p;
            using (var client = new HttpClient())
            {
                var response = client.DeleteAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default;
            }
        }

    }
}
