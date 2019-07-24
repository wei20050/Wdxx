using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace CardReading.ServiceHost
{

    /// <summary>
    /// 本地服务宿主类
    /// </summary>
    public class CoreClientHost
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
        private static HttpListener _httpobj;

        /// <inheritdoc />
        /// <summary>
        /// 服务类 构造(默认端口{80}若不可用自动生成随机端口,默认IP{127.0.0.1})
        /// </summary>
        /// <param name="serviceClass">服务类</param>
        public CoreClientHost(Type serviceClass) : this(serviceClass, GetPort()) { }

        /// <inheritdoc />
        /// <summary>
        /// 服务类 端口号 构造(默认IP{127.0.0.1})
        /// </summary>
        /// <param name="serviceClass">服务类</param>
        /// <param name="port">服务端口</param>
        public CoreClientHost(Type serviceClass, int port) : this(serviceClass, port, "127.0.0.1") { }

        /// <summary>
        /// 服务类 端口 IP{ip写 + 代表所有本机ip} 构造
        /// </summary>
        /// <param name="serviceClass">服务类</param>
        /// <param name="port">服务端口</param>
        /// <param name="ip">服务ip地址</param>
        public CoreClientHost(Type serviceClass, int port, string ip)
        {
            _serviceClass = serviceClass;
            _serviceFunArr = _serviceClass.GetMethods();
            _uri = "http://" + ip + ":" + port + "/";
            //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            _httpobj = new HttpListener();
            IsOpen = false;
        }

        /// <summary>
        /// 判断端口是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool IsPortAvailble(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            foreach (var t in tcpConnInfoArray)
            {
                if (t.LocalEndPoint.Port == port)
                {
                    return true;
                }
            }
            return false;
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
        /// 获取离线Uri
        /// </summary>
        /// <returns></returns>
        private static int GetPort()
        {
            var port = 80;
            if (IsPortAvailble(80))
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
            _httpobj.Prefixes.Add(_uri.TrimEnd('/') + '/');
            //启动监听器
            _httpobj.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            _httpobj.BeginGetContext(Result, _httpobj);
            IsOpen = true;
            return _uri;
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Close()
        {
            //关闭监听器
            _httpobj.Stop();
        }

        /// <summary>
        /// 接收请求的委托
        /// </summary>
        /// <param name="ar"></param>
        private void Result(IAsyncResult ar)
        {
            //当接收到请求后程序流会走到这里
            //获得context对象
            var context = _httpobj.EndGetContext(ar);
            //继续异步监听
            _httpobj.BeginGetContext(Result, _httpobj);
            var request = context.Request;
            //将发送到客户端的请求响应中的客户端的对象
            var response = context.Response;
            //后台跨域请求，通常设置为配置文件   如果是js的ajax请求，还可以设置跨域的ip地址与参数
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            //后台跨域参数设置，通常设置为配置文件
            //context.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW");
            //后台跨域请求设置，通常设置为配置文件
            //context.Response.AppendHeader("Access-Control-Allow-Method", "post");
            //告诉客户端返回的ContentType类型为JSON格式，编码为UTF-8
            context.Response.ContentType = "application/json;charset=UTF-8";
            //添加响应头信息
            context.Response.AddHeader("Content-type", "application/json");
            //设置响应的编码格式
            context.Response.ContentEncoding = Encoding.UTF8;
            //处理客户端发送的请求并返回处理信息
            var retData = HandleRequest(request, response);
            //设置客户端返回信息的编码
            var retByteArr = Encoding.UTF8.GetBytes(retData);
            try
            {
                using (var stream = response.OutputStream)
                {
                    //把处理信息返回到客户端
                    stream.Write(retByteArr, 0, retByteArr.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Network anomaly:" + ex);
            }
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private string HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.InputStream != null)
            {
                string funName = null;
                //具体要执行的方法
                MethodInfo mi = null;
                //参数
                //这里是有客户端发送的正文本数据流的请求 
                try
                {
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
                    //url路径
                    var postSegments = request.Url.Segments;
                    //根据url路径确定方法名
                    if (postSegments.Length < 2)
                    {
                        return "Service method not found";
                    }
                    funName = postSegments[1].TrimEnd('/').ToUpper();
                    //获取方法名相同的所有方法
                    var mis = new List<MethodInfo>();
                    foreach (var s in _serviceFunArr)
                    {
                        if (s.Name.ToUpper() == funName)
                        {
                            mis.Add(s);
                        }
                    }
                    if (mis.Count == 0)
                    {
                        response.StatusDescription = "404";
                        response.StatusCode = 404;
                        return "Service method not found";
                    }
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
                                    try
                                    {
                                        var dArr = d.Split('=');
                                        if (ps[i].Name != dArr[0]) continue;
                                        objArr[i] = dArr[1];
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(@"参数错误:" + e);
                                        response.StatusDescription = "404";
                                        response.StatusCode = 404;
                                        return string.Empty;
                                    }
                                }
                            }
                        }
                    }
                    return Fun(mi, objArr);
                }
                catch (Exception ex)
                {
                    response.StatusDescription = "404";
                    response.StatusCode = 404;
                    //把服务端错误信息直接返回可能会导致信息不安全，此处仅供参考
                    Console.WriteLine(@"接收数据时发生错误 Url:{0} Method:{1} err:{2}", request.Url, funName, ex);
                    return " Err: data cannot be parsed";
                }
            }
            response.StatusDescription = "404";
            response.StatusCode = 404;
            return " Err: does not allow empty submission";
        }

        /// <summary>
        /// 执行服务器方法
        /// </summary>
        /// <param name="mi">方法</param>
        /// <param name="pos">参数</param>
        /// <returns></returns>
        private string Fun(MethodInfo mi, object[] pos)
        {
            try
            {
                //创建实例
                var o = Activator.CreateInstance(_serviceClass);
                //调用方法
                if (mi != null) return mi.Invoke(o, pos).ToString();
                Console.WriteLine(@"Number mismatch of parameters");
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
