using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Wdxx.Core
{

    /// <summary>
    /// 客户端通信核心
    /// </summary>
    public class CoreClient
    {

        /// <summary>
        /// 服务地址
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// WSDL信息
        /// </summary>
        private readonly string _wsdl;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CoreClient()
        {
        }

        /// <summary>
        /// 带服务地址构造函数
        /// </summary>
        /// <param name="serviceUrl">服务地址</param>
        public CoreClient(string serviceUrl)
        {
            ServiceUrl = serviceUrl.TrimEnd('/');
            _wsdl = Wsdl();
        }

        /// <summary>
        /// 发送请求(返回字符串类型)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        public string Send(string method, params object[] sendData)
        {
            var retXml = SendCore(method, sendData);
            var resultXml = GetResult(retXml);
            return string.IsNullOrEmpty(resultXml) ? string.Empty : resultXml;
        }

        /// <summary>
        /// 发送请求(返回有构造函数的类)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        public T Send<T>(string method, params object[] sendData) where T : new()
        {
            var retXml = SendCore(method, sendData);
            var resultXml = GetResult(retXml);
            if (string.IsNullOrEmpty(resultXml)) return new T();
            var xmlTmp = ObjectToXml(new T());
            var docTmp = new XmlDocument();
            docTmp.LoadXml(xmlTmp);
            var root = docTmp.DocumentElement;
            if (root == null) return new T();
            root.InnerXml = resultXml;
            return string.IsNullOrEmpty(docTmp.OuterXml) ? new T() : XmlToObject<T>(docTmp.OuterXml);
        }

        /// <summary>
        /// 发送请求(核心)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        public string SendCore(string method, params object[] sendData)
        {
            try
            {
                string result;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ServiceUrl);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/xml; charset=utf-8";
                //这个在非GET的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据做返回数据
                httpWebRequest.ServicePoint.Expect100Continue = false;
                if (sendData != null)
                {
                    var xmlData = GetData(method, sendData);
                    var data = Encoding.UTF8.GetBytes(xmlData);
                    httpWebRequest.ContentLength = data.Length;
                    var outStream = httpWebRequest.GetRequestStream();
                    outStream.Write(data, 0, data.Length);
                    outStream.Close();
                }
                else
                {
                    httpWebRequest.ContentLength = 0;
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
                throw new Exception("HttpErr" + " uri:" + ServiceUrl + " method:" + method + " httpData:" + sendData + "err:" + ex);
            }
        }

        /// <summary>
        /// 发送请求(POST)
        /// </summary>
        /// <param name="serviceUrl">请求地址</param>
        /// <param name="sendData">请求参数</param>
        /// <returns></returns>
        public static string HttpSend(string serviceUrl, string sendData)
        {
            try
            {
                string result;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/xml; charset=utf-8";
                //这个在非GET的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据做返回数据
                httpWebRequest.ServicePoint.Expect100Continue = false;
                if (sendData != null)
                {
                    var data = Encoding.UTF8.GetBytes(sendData);
                    httpWebRequest.ContentLength = data.Length;
                    var outStream = httpWebRequest.GetRequestStream();
                    outStream.Write(data, 0, data.Length);
                    outStream.Close();
                }
                else
                {
                    httpWebRequest.ContentLength = 0;
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
                throw new Exception("HttpErr" + " uri:" + serviceUrl + " httpData:" + sendData + "err:" + ex);
            }
        }

        /// <summary>
        /// 获取发送的数据
        /// </summary>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string GetData(string method, params object[] data)
        {
            var ps = GetParams(method);
            var xmlData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine +
                          "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + Environment.NewLine +
                             "<soap:Body>" + Environment.NewLine +
                              "<" + method + " xmlns=\"http://tempuri.org/\">" + Environment.NewLine;
            for (var i = 0; i < data.Length; i++)
            {
                var d = ObjectToXml(data[i]);
                d = d.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", string.Empty);
                if (ps != null)
                {
                    var docd = new XmlDocument();
                    docd.LoadXml(d);
                    var noded = docd.DocumentElement;
                    if (noded != null)
                    {
                        var tmp = "<" + ps[i] + ">";
                        tmp += noded.InnerXml;
                        tmp += "</" + ps[i] + ">";
                        d = tmp;
                    }
                }
                d = d.Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty);
                d = d.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", string.Empty);
                xmlData += d + Environment.NewLine;
            }
            xmlData += "</" + method + ">" + Environment.NewLine +
                "</soap:Body>" + Environment.NewLine +
                  "</soap:Envelope>";
            return xmlData;
        }

        /// <summary>
        /// 获取接收的数据
        /// </summary>
        /// <returns></returns>
        private static string GetResult(string resultXml)
        {
            if (string.IsNullOrEmpty(resultXml)) return string.Empty;
            var doc = new XmlDocument();
            doc.LoadXml(resultXml);
            var envelope = doc.DocumentElement;
            if (envelope == null) return string.Empty;
            var body = envelope.ChildNodes[0];
            if (body == null) return string.Empty;
            var response = body.ChildNodes[0];
            if (response == null) return string.Empty;
            var result = response.ChildNodes[0];
            return result == null ? string.Empty : result.InnerXml.Replace(" xmlns=\"http://tempuri.org/\"", string.Empty);
        }

        /// <summary>
        /// 获取参数列表
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private List<string> GetParams(string method)
        {
            var retList = new List<string>();
            if (string.IsNullOrEmpty(_wsdl)) return null;
            var docWsdl = new XmlDocument();
            docWsdl.LoadXml(_wsdl);
            var definitions = docWsdl.DocumentElement;
            if (definitions == null) return null;
            var types = definitions.ChildNodes[0];
            var schema = types.ChildNodes[0];
            //方法节点
            var methodNode = schema.ChildNodes.Cast<XmlElement>().FirstOrDefault(node => node.GetAttribute("name") == method);
            if (methodNode == null)
            {
                return null;
            }
            var complexType = methodNode.ChildNodes[0];
            var sequence = complexType.ChildNodes[0];
            if (sequence == null)
            {
                return null;
            }
            retList.AddRange(from XmlElement x in sequence.ChildNodes select x.GetAttribute("name"));
            return retList;
        }

        /// <summary>
        /// 获取WSDL信息
        /// </summary>
        /// <returns></returns>
        private string Wsdl()
        {
            try
            {
                string result;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ServiceUrl + "?WSDL");
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "text/xml; charset=utf-8";
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
                throw new Exception("HttpErr uri:" + ServiceUrl + "?WSDL err:" + ex);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        private static T XmlToObject<T>(string xml)
        {
            try
            {
                using (var sr = new StringReader(xml))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("反序列化失败 err:" + ex);
            }
        }

        /// <summary> 
        /// 序列化
        /// </summary>
        private static string ObjectToXml<T>(T obj)
        {
            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        }

    }
}
