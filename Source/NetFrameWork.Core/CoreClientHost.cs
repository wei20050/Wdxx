using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace NetFrameWork.Core
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
        private static HttpListener _httpObj;

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
            if (!IsPortAvailable(port))
            {
                port = GetPort();
            }
            _uri = "http://" + ip + ":" + port + "/";
            //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            _httpObj = new HttpListener();
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
        /// 获取离线Uri
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
            _httpObj.Prefixes.Add(_uri);
            //启动监听器
            _httpObj.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            _httpObj.BeginGetContext(Result, _httpObj);
            IsOpen = true;
            return _uri;
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Close()
        {
            //关闭监听器
            _httpObj.Stop();
        }

        /// <summary>
        /// 接收请求的委托
        /// </summary>
        /// <param name="ar"></param>
        private void Result(IAsyncResult ar)
        {
            //当接收到请求后程序流会走到这里
            //获得context对象
            var context = _httpObj.EndGetContext(ar);
            //继续异步监听
            _httpObj.BeginGetContext(Result, _httpObj);
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
            var retData = HandleRequest(request, response) ?? string.Empty;
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
                CoreLog.Error("网络异常:" + ex, "CORE_");
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
            if (request.HttpMethod.ToUpper() != "POST")
            {
                return null;
            }
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
                    //判断是否是WebService模式调用
                    object[] objArr;
                    //url路径
                    var postSegments = request.Url.Segments;
                    //根据url路径确定方法名
                    if (postSegments.Length < 2)
                    {
                        response.StatusDescription = "404";
                        response.StatusCode = 404;
                        CoreLog.Error("找不到服务方法", "CORE_");
                        return null;
                    }
                    funName = postSegments[1].TrimEnd('/');
                    //判断是否是WebServiceSoap模式调用
                    if (funName == "WebServiceSoap")
                    {
                        if (string.IsNullOrEmpty(data))
                        {
                            response.StatusDescription = "404";
                            response.StatusCode = 404;
                            CoreLog.Error("找不到服务方法", "CORE_");
                            return null;
                        }
                        var sendDataArr = (SendDataArr)CoreConvert.JsonDataToObj(data, typeof(SendDataArr));
                        //获取方法名相同的所有方法
                        var mis = _serviceFunArr.Where(f =>
                        string.Equals(f.Name, sendDataArr.Method, StringComparison.CurrentCultureIgnoreCase)).ToList();
                        if (mis.Count == 0)
                        {
                            response.StatusDescription = "404";
                            response.StatusCode = 404;
                            CoreLog.Error("找不到服务方法", "CORE_");
                            return null;
                        }
                        //获取参数
                        var dataList = (List<string>)CoreConvert.JsonDataToObj(sendDataArr.DataArr, typeof(List<string>));
                        //这里为空则没有参数
                        if (dataList == null)
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
                            //这里是参数数组
                            objArr = new object[dataList.Count];
                            foreach (var m in mis)
                            {
                                var ps = m.GetParameters();
                                if (ps.Length != dataList.Count) continue;
                                //找到参数数量对应的方法
                                mi = m;
                                for (var i = 0; i < ps.Length; i++)
                                {
                                    objArr[i] = CoreConvert.JsonDataToObj(dataList[i], ps[i].ParameterType);
                                }
                            }

                        }
                    }
                    else
                    {
                        //获取方法名相同的所有方法
                        var mis = _serviceFunArr.Where(f => string.Equals(f.Name, funName, StringComparison.CurrentCultureIgnoreCase)).ToList();
                        if (mis.Count == 0)
                        {
                            response.StatusDescription = "404";
                            response.StatusCode = 404;
                            CoreLog.Error("找不到服务方法", "CORE_");
                            return null;
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
                                            CoreLog.Error("参数错误:" + e, "CORE_");
                                            response.StatusDescription = "404";
                                            response.StatusCode = 404;
                                            return null;
                                        }
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
                    CoreLog.Error($"接收数据时发生错误 Url:{request.Url} Method:{funName} err:{ex}", "CORE_");
                    return null;
                }
            }
            response.StatusDescription = "404";
            response.StatusCode = 404;
            CoreLog.Error("不允许空提交", "CORE_");
            return null;
        }

        /// <summary>
        /// 执行服务器方法
        /// </summary>
        /// <param name="mi">方法</param>
        /// <param name="pos">参数</param>
        /// <returns></returns>
        private string Fun(MethodBase mi, object[] pos)
        {
            try
            {
                //创建实例
                var o = Activator.CreateInstance(_serviceClass);
                //调用方法
                return mi != null ? CoreConvert.ObjToJson(mi.Invoke(o, pos)) : null;
            }
            catch (Exception ex)
            {
                CoreLog.Error("方法:" + mi?.Name + "执行错误:" + ex, "CORE_");
                throw new Exception(ex.Message);
            }
        }
    }

    /// <summary>
    /// 离线webservice传输数据类
    /// </summary>
    public class SendDataArr
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public string Method;
        /// <summary>
        /// 参数数据组
        /// </summary>
        public string DataArr;
    }
}
