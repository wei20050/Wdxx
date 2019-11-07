using System;
using System.IO;
using System.Net;
using System.Text;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core
{

    /// <summary>
    /// Http通信核心
    /// </summary>
    public static class CoreHttp
    {

        /// <summary>
        /// Http发送参数
        /// </summary>
        public class SendParam
        {

            /// <summary>
            /// 超时时间(单位:毫秒)
            /// </summary>
            public int TimeOut { get; set; } = 60000;

            /// <summary>
            /// 协议标头
            /// </summary>
            public WebHeaderCollection Headers { get; set; }

            /// <summary>
            /// 编码
            /// </summary>
            public string Encoding { get; set; } = "UTF-8";

            /// <summary>
            /// 内容类型(例:text/xml   application/json)
            /// </summary>
            public string ContentType { get; set; } = "text/xml";

        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="httpUri">请求地址</param>
        /// <param name="method">请求的方法</param>
        /// <param name="httpData">请求参数 例:{"value": "HttpSend"} C#格式(@"{""value"":""HttpSend""}")</param>
        /// <param name="sendParam">发送参数</param>
        /// <returns></returns>
        public static string Send(string httpUri, string method = "GET", string httpData = null, SendParam sendParam = null)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            Stream reqStream = null;
            Stream resStream = null;
            try
            {
                if (sendParam == null)
                {
                    sendParam = new SendParam();
                }
                httpWebRequest = (HttpWebRequest)WebRequest.Create(httpUri);
                if (sendParam.Headers != null)
                {
                    httpWebRequest.Headers = sendParam.Headers;
                }
                httpWebRequest.Method = method;
                httpWebRequest.Timeout = sendParam.TimeOut;
                httpWebRequest.ContentType = sendParam.ContentType;
                if (method != "GET")
                {
                    //这个在非GET的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据做返回数据
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                    if (!string.IsNullOrEmpty(httpData))
                    {
                        var data = Encoding.UTF8.GetBytes(httpData);
                        httpWebRequest.ContentLength = data.Length;
                        reqStream = httpWebRequest.GetRequestStream();
                        reqStream.Write(data, 0, data.Length);
                        reqStream.Close();
                    }
                    else
                    {
                        httpWebRequest.ContentLength = 0;
                    }
                }
                var webResponse = httpWebRequest.GetResponse();
                httpWebResponse = (HttpWebResponse)webResponse;
                resStream = httpWebResponse.GetResponseStream();
                string result;
                if (resStream != null)
                {
                    var streamReader = new StreamReader(resStream, Encoding.GetEncoding(sendParam.Encoding));
                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                    resStream.Close();
                }
                else
                {
                    result = string.Empty;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("CoreHttp.HttpSend Err: method=>" + method + " uri=>" + httpUri + " httpData=>" + httpData + "err=>" + ex);
            }
            finally
            {
                resStream?.Close();
                reqStream?.Close();
                httpWebResponse?.Close();
                httpWebRequest?.Abort();
            }
        }
    }
}
