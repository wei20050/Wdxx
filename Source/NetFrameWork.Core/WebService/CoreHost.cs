using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core.WebService
{

    /// <summary>
    /// 本地服务宿主类
    /// </summary>
    public class CoreHost
    {

        /// <summary>
        /// 是否开启服务
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// 服务地址
        /// </summary>
        private readonly string _uri;

        /// <summary>
        /// 服务类
        /// </summary>
        private readonly Type _serviceClass;

        /// <summary>
        /// 服务方法组
        /// </summary>
        private readonly MethodInfo[] _serviceFunArr;

        /// <summary>
        /// http协议侦听
        /// </summary>
        private readonly HttpListener _httpListener;

        /// <inheritdoc />
        /// <summary>
        /// 服务类 构造(默认端口{80}若不可用自动生成随机端口,默认开启当前电脑所有ip})
        /// </summary>
        /// <param name="serviceClass">服务类</param>
        public CoreHost(Type serviceClass) : this(serviceClass, GetPort()) { }

        /// <inheritdoc />
        /// <summary>
        /// 服务类 端口号 构造(默认开启当前电脑所有ip)
        /// </summary>
        /// <param name="serviceClass">服务类</param>
        /// <param name="port">服务端口</param>
        public CoreHost(Type serviceClass, int port) : this(serviceClass, port, "+") { }

        /// <summary>
        /// 服务类 端口 IP{ip写 + 代表所有本机ip} 构造
        /// </summary>
        /// <param name="serviceClass">服务类</param>
        /// <param name="port">服务端口</param>
        /// <param name="ip">服务ip地址</param>
        public CoreHost(Type serviceClass, int port, string ip)
        {
            _serviceClass = serviceClass;
            _serviceFunArr = _serviceClass.GetMethods();
            _uri = $"http://{ip}:{port}/";
            //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            _httpListener = new HttpListener();
            IsOpen = false;
        }

        /// <summary>
        /// 判断端口是否可用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool IsPortAvailable(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            return tcpConnInfoArray.All(tcp => tcp.LocalEndPoint.Port != port);
        }

        /// <summary>
        /// 动态获取可用的端口
        /// </summary>
        /// <returns></returns>
        private static int GetFreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        /// <summary>
        /// 获取可用端口
        /// </summary>
        /// <returns></returns>
        private static int GetPort()
        {
            var port = 80;
            if (!IsPortAvailable(80))
            {
                port = GetFreeTcpPort();
            }
            return port;
        }

        /// <summary>
        /// 开启服务 返回服务地址
        /// </summary>
        public string Open()
        {
            //定义url及端口号，通常设置为配置文件
            _httpListener.Prefixes.Add(_uri);
            //启动监听器
            _httpListener.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            _httpListener.BeginGetContext(Result, _httpListener);
            IsOpen = true;
            return _uri.Replace("://+", "://127.0.0.1");
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Close()
        {
            //关闭监听器
            _httpListener.Stop();
        }

        /// <summary>
        /// 接收请求的委托
        /// </summary>
        /// <param name="ar"></param>
        private void Result(IAsyncResult ar)
        {
            //当接收到请求后程序流会走到这里
            //获得context对象
            var context = _httpListener.EndGetContext(ar);
            //继续异步监听
            _httpListener.BeginGetContext(Result, _httpListener);
            try
            {
                var request = context.Request;
                //将发送到客户端的请求响应中的客户端的对象
                var response = context.Response;

                if (request.Headers.AllKeys.ToList().Contains("Authorization"))
                {
                    HttpContext.Current = new HttpContext(new HttpRequest(null, request.Url.ToString(), null),
                        new HttpResponse(null))
                    {
                        User = new GenericPrincipal(new GenericIdentity(request.Headers["Authorization"]), null)
                    };
                }

                //后台跨域请求，通常设置为配置文件   如果是js的ajax请求，还可以设置跨域的ip地址与参数
                //context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                //后台跨域参数设置，通常设置为配置文件
                //context.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW");
                //后台跨域请求设置，通常设置为配置文件
                //context.Response.AppendHeader("Access-Control-Allow-Method", "post");
                //告诉客户端返回的ContentType类型为text/xml格式，编码为UTF-8
                context.Response.ContentType = "text/xml;charset=UTF-8";
                //添加响应头信息
                context.Response.AddHeader("Content-type", "text/xml");
                //设置响应的编码格式
                context.Response.ContentEncoding = Encoding.UTF8;
                //处理客户端发送的请求并返回处理信息
                var retData = HandleRequest(request) ?? string.Empty;
                //设置客户端返回信息的编码
                var retByteArr = Encoding.UTF8.GetBytes(retData);
                using (var stream = response.OutputStream)
                {
                    //把处理信息返回到客户端
                    stream.Write(retByteArr, 0, retByteArr.Length);
                }
            }
            catch (Exception e)
            {
                //将发送到客户端的请求响应中的客户端的对象
                var response = context.Response;
                response.StatusDescription = "500";
                response.StatusCode = 500;
                //告诉客户端返回的ContentType类型为text/xml格式，编码为UTF-8
                context.Response.ContentType = "text/xml;charset=UTF-8";
                //添加响应头信息
                context.Response.AddHeader("Content-type", "text/xml");
                //设置响应的编码格式
                context.Response.ContentEncoding = Encoding.UTF8;
                //处理客户端发送的请求并返回处理信息
                var retData = ErrXml(e);
                //设置客户端返回信息的编码
                var retByteArr = Encoding.UTF8.GetBytes(retData);
                using (var stream = response.OutputStream)
                {
                    //把处理信息返回到客户端
                    stream.Write(retByteArr, 0, retByteArr.Length);
                }
            }

        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private static string ErrXml(Exception e)
        {
            var retSb = new StringBuilder();
            retSb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            retSb.AppendLine("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap =\"http://schemas.xmlsoap.org/soap/envelope/\">");
            retSb.AppendLine("  <soap:Body>");
            retSb.AppendLine("    <soap:Fault>");
            retSb.AppendLine("      <faultcode>soap:Server</faultcode>");
            retSb.AppendFormat("      <faultstring>System.Web.Services.Protocols.SoapException: 服务器无法处理请求。 {0}</faultstring>{1}", e.InnerException, Environment.NewLine);
            retSb.AppendLine("    </soap:Fault>");
            retSb.AppendLine("  </soap:Body>");
            retSb.AppendLine("</soap:Envelope>");
            return retSb.ToString();
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string HandleRequest(HttpListenerRequest request)
        {
            //过滤非post请求
            if (request.HttpMethod.ToUpper() != "POST")
            {
                return null;
            }
            //不允许空提交
            if (request.InputStream == null) throw new Exception(" Err: does not allow empty submission");
            //具体要执行的方法
            MethodInfo mi = null;
            //这里是有客户端发送的正文本数据流的请求 
            var byteList = new List<byte>();
            var byteArr = new byte[2048];
            int readLen;
            var len = 0;
            //接收客户端传过来的数据并转成字符串类型
            do
            {
                readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                len += readLen;
                byteList.AddRange(byteArr);
            } while (readLen != 0);
            //获取得到数据data
            var data = Encoding.UTF8.GetString(byteList.ToArray(), 0, len);
            object[] objArr;
            //判断是否是WebService  SOAP调用 若是/是WebService 否则是/加方法名
            if (request.RawUrl == "/")
            {
                var funName = GetFunName(data).ToUpper();
                //获取方法名相同的所有方法
                var mis = _serviceFunArr.Where(f => f.Name.ToUpper() == funName).ToList();
                //获取参数
                var dataObj = GetParams(data);
                //这里是参数数组
                objArr = new object[dataObj.Count];
                foreach (var m in mis)
                {
                    var ps = m.GetParameters();
                    if (ps.Length != dataObj.Count) continue;
                    //找到参数数量对应的方法
                    mi = m;
                    for (var i = 0; i < ps.Length; i++)
                    {
                        //拆分参数字符串 获得方法名和值
                        objArr[i] = GetParam(dataObj[i], ps[i].ParameterType);
                    }
                }
            }
            else
            {
                //获取方法名相同的所有方法
                var mis = _serviceFunArr.Where(f => string.Equals(f.Name, request.RawUrl.Trim('/'), StringComparison.CurrentCultureIgnoreCase)).ToList();
                //这里为空则没有参数
                if (string.IsNullOrEmpty(data))
                {
                    foreach (var m in mis)
                    {
                        var ps = m.GetParameters();
                        if (ps.Length != 0) continue;
                        //找到没有参数的方法
                        mi = m;
                        break;
                    }
                    //参数默认值null
                    objArr = null;
                }
                else
                {
                    //获取参数
                    var dataArr = data.Split('&');
                    //这里是参数数组
                    objArr = new object[dataArr.Length];
                    foreach (var m in mis)
                    {
                        var ps = m.GetParameters();
                        if (ps.Length != dataArr.Length) continue;
                        //找到参数数量对应的方法
                        mi = m;
                        for (var i = 0; i < ps.Length; i++)
                        {
                            foreach (var d in dataArr)
                            {
                                //拆分参数字符串 获得方法名和值
                                var dArr = d.Split('=');
                                if (ps[i].Name != dArr[0]) continue;
                                objArr[i] = dArr[1];
                                break;
                            }
                        }
                    }
                }
            }
            return Fun(mi, objArr);
        }

        /// <summary>
        /// 执行服务器方法
        /// </summary>
        /// <param name="mi">方法</param>
        /// <param name="pos">参数</param>
        /// <returns></returns>
        private string Fun(MethodBase mi, object[] pos)
        {
            //创建实例
            var o = Activator.CreateInstance(_serviceClass);
            //调用方法
            return XmlData(mi.Name, mi.Invoke(o, pos));
        }

        /// <summary>
        /// 获取XML参数列表
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static List<string> GetParams(string xml)
        {
            var retList = new List<string>();
            if (string.IsNullOrEmpty(xml)) return null;
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var envelope = doc.DocumentElement;
            if (envelope == null) return null;
            var body = envelope.ChildNodes[0];
            var function = body.ChildNodes[0];
            retList.AddRange(from XmlElement node in function.ChildNodes select node.OuterXml.Replace($" xmlns=\"{node.NamespaceURI}\"", string.Empty));
            return retList;
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="xml">参数xml</param>
        /// <param name="t">参数类型</param>
        /// <returns></returns>
        private static object GetParam(string xml, Type t)
        {
            //字段的xmlDoc对象
            var dic = new XmlDocument();
            dic.LoadXml(xml);
            //这里检测到字段类型是string类型 直接返回内容
            if (t == typeof(string))
            {
                return dic.InnerText;
            }
            //这里检测到字段类型是枚举类型 直接拿到对应的枚举
            if (t.BaseType == typeof(Enum))
            {
                return Enum.Parse(t, dic.InnerText);
            }
            try
            {
                //字段xml的根节点对象
                var rootDic = dic.DocumentElement;
                //临时字段类型
                var tmpObj = Activator.CreateInstance(t);
                //临时字段类型序列化的xml
                var tmpXml = ObjToXml(tmpObj);
                //临时字段的xmlDoc对象
                var tmpDic = new XmlDocument();
                tmpDic.LoadXml(tmpXml);
                //临时字段xml的根节点对象
                var tmpRootDic = tmpDic.DocumentElement;
                if (tmpRootDic == null || rootDic == null)
                {
                    return null;
                }
                //把字段内容填充到临时字段
                tmpRootDic.InnerXml = rootDic.InnerXml;
                //把临时字段反序列化成字段值
                return XmlToObj(tmpDic.InnerXml, t);
            }
            catch
            {
                //这里异常肯定是参数无法序列化的类尝试直接改变根节点名称为类名
                var pXmlDocument = new XmlDocument();
                pXmlDocument.LoadXml(xml);
                var rootElement = pXmlDocument.DocumentElement;
                var newElement = pXmlDocument.CreateElement(t.Name);
                if (rootElement != null)
                {
                    newElement.InnerXml = rootElement.InnerXml;
                    pXmlDocument.ReplaceChild(newElement, rootElement);
                }
                return XmlToObj(pXmlDocument.InnerXml, t);
            }

        }

        /// <summary>
        /// 获取XML方法名
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static string GetFunName(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return null;
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var definitions = doc.DocumentElement;
            if (definitions == null) return null;
            var types = definitions.ChildNodes[0];
            var schema = types.ChildNodes[0];
            return schema.Name;
        }

        /// <summary>
        /// 数据包装
        /// </summary>
        /// <returns></returns>
        private static string XmlData(string funName, object data)
        {
            var retSb = new StringBuilder();
            retSb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            retSb.AppendLine("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap =\"http://schemas.xmlsoap.org/soap/envelope/\">");
            retSb.AppendLine("  <soap:Body>");
            retSb.AppendFormat("    <{0}Response xmlns =\"http://tempuri.org/\">{1}", funName, Environment.NewLine);
            if (data != null)
            {
                var dataXml = ObjToXml(data);
                var pXmlDocument = new XmlDocument();
                pXmlDocument.LoadXml(dataXml);
                var rootElement = pXmlDocument.DocumentElement;
                var newElement = pXmlDocument.CreateElement(funName + "Result");
                if (rootElement != null)
                {
                    newElement.InnerXml = rootElement.InnerXml;
                    pXmlDocument.ReplaceChild(newElement, rootElement);
                }
                retSb.AppendLine(pXmlDocument.InnerXml);
            }
            retSb.AppendFormat("    </{0}Response>{1}", funName, Environment.NewLine);
            retSb.AppendLine("  </soap:Body>");
            retSb.AppendLine("</soap:Envelope>");
            return retSb.ToString();
        }

        /// <summary>
        /// XML序列化(省略声明 去掉命名空间)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ObjToXml(object obj)
        {
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = Encoding.Default };
            var mem = new MemoryStream();
            using (var writer = XmlWriter.Create(mem, settings))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);
                var formatter = new XmlSerializer(obj.GetType());
                formatter.Serialize(writer, obj, ns);
            }
            return Encoding.Default.GetString(mem.ToArray());
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        private static object XmlToObj(string xmlStr, Type t)
        {
            using (var sr = new StringReader(xmlStr))
            {
                var serializer = new XmlSerializer(t);
                return serializer.Deserialize(sr);
            }
        }

    }
}
