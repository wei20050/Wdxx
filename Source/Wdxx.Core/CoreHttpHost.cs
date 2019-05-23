using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Wdxx.Core
{

    /// <summary>
    /// 本地服务挂接类
    /// </summary>
    public class CoreHttpHost
    {

        #region 日志
        //日志文件夹默认根目录Logs文件夹
        private static readonly string FilePath =
            AppDomain.CurrentDomain.BaseDirectory + "Logs\\";

        //默认日志分隔文件大小 100M
        private const int FileSize = 100 * 1024 * 1024;

        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void Error(object log)
        {
            WriteFile("[Error] " + log, CreateLogPath(string.Empty));
        }

        /// <summary>
        /// 写操作日志
        /// </summary>
        public static void Info(object log)
        {
            WriteFile("[Info] " + log, CreateLogPath(string.Empty));
        }

        /// <summary>
        /// 生成日志文件路径
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        private static string CreateLogPath(string prefix)
        {
            var index = 0;
            string logPath;
            var bl = true;
            do
            {
                index++;
                logPath = FilePath + prefix + DateTime.Now.ToString("yyyyMMdd") + "_" + index + ".log";
                if (File.Exists(logPath))
                {
                    var fileInfo = new FileInfo(logPath);
                    if (fileInfo.Length < FileSize)
                    {
                        bl = false;
                    }
                }
                else
                {
                    bl = false;
                }
            } while (bl);
            return logPath;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        private static void WriteFile(string txt, string logPath)
        {
            new Thread(() =>
            {
                MutexExec(FilePath, () =>
                {
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    var value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + txt + Environment.NewLine;
                    File.AppendAllText(logPath, value);
                });
            }).Start();
        }

        #region 进程与线程保证同步

        /// <summary>
        /// 多线程与多进程间同步操作文件 默认不是递归
        /// </summary>
        private static void MutexExec(string filePath, Action action)
        {
            MutexExec(filePath, action, false);
        }

        /// <summary>
        /// 多线程与多进程间同步操作文件
        /// </summary>
        /// <param name="filePath">文件路径
        /// (如果将 name 指定为 null 或空字符串，则创建一个局部互斥体。 
        /// 如果名称以前缀“Global\”开头，则 mutex 在所有终端服务器会话中均为可见。 
        /// 如果名称以前缀“Local\”开头，则 mutex 仅在创建它的终端服务器会话中可见。 
        /// 如果创建已命名 mutex 时不指定前缀，则它将采用前缀“Local\”。)</param>
        /// <param name="action">同步处理操作</param>
        /// <param name="recursive">指示当前调用是否为递归处理，递归处理时检测到异常则抛出异常，避免进入无限递归</param>
        private static void MutexExec(string filePath, Action action, bool recursive)
        {
            //生成文件对应的同步键，可自定义格式（互斥体名称对特殊字符支持不友好，遂转换为BASE64格式字符串）
            var fileKey = Convert.ToBase64String(Encoding.Default.GetBytes(string.Format(@"FILE\{0}", filePath)));
            //转换为操作系统级的同步键
            var mutexKey = string.Format(@"Global\{0}", fileKey);
            //声明一个已命名的互斥体，实现进程间同步；该命名互斥体不存在则自动创建，已存在则直接获取
            //initiallyOwned: false：默认当前线程并不拥有已存在互斥体的所属权，即默认本线程并非为首次创建该命名互斥体的线程
            //注意：并发声明同名的命名互斥体时，若间隔时间过短，则可能同时声明了多个名称相同的互斥体，并且同名的多个互斥体之间并不同步，高并发用户请另行处理
            using (var mut = new Mutex(false, mutexKey))
            {
                try
                {
                    //上锁，其他线程需等待释放锁之后才能执行处理；若其他线程已经上锁或优先上锁，则先等待其他线程执行完毕
                    mut.WaitOne();
                    //执行处理代码（在调用WaitHandle.WaitOne至WaitHandle.ReleaseMutex的时间段里，只有一个线程处理，其他线程都得等待释放锁后才能执行该代码段）
                    action();
                }
                //当其他进程已上锁且没有正常释放互斥锁时(譬如进程忽然关闭或退出)，则会抛出AbandonedMutexException异常
                catch (AbandonedMutexException)
                {
                    //避免进入无限递归
                    if (recursive) throw;

                    //非递归调用，由其他进程抛出互斥锁解锁异常时，重试执行
                    MutexExec(mutexKey, action, true);
                }
                finally
                {
                    //释放锁，让其他进程(或线程)得以继续执行
                    mut.ReleaseMutex();
                }
            }
        }

        #endregion
        #endregion

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
        public CoreHttpHost(Type serviceClass) : this(serviceClass, GetPort()) { }

        /// <inheritdoc />
        /// <summary>
        /// 服务类 端口号 构造(默认IP{127.0.0.1})
        /// </summary>
        /// <param name="serviceClass">服务类</param>
        /// <param name="port">服务端口</param>
        public CoreHttpHost(Type serviceClass, int port) : this(serviceClass, port, "127.0.0.1") { }

        /// <summary>
        /// 服务类 端口 IP{ip写 + 代表所有本机ip} 构造
        /// </summary>
        /// <param name="serviceClass">服务类</param>
        /// <param name="port">服务端口</param>
        /// <param name="ip">服务ip地址</param>
        public CoreHttpHost(Type serviceClass, int port, string ip)
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
            return tcpConnInfoArray.All(tcpi => tcpi.LocalEndPoint.Port != port);
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
                Error("Network anomaly:" + ex);
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
            //客户端指定的方法
            var httpMethod = request.HttpMethod.ToUpper();
            return httpMethod == "GET" ? HandleRequestGet(request, response) : HandleRequestNotGet(request, response, httpMethod);
        }

        /// <summary>
        /// 处理请求Get
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private string HandleRequestGet(HttpListenerRequest request, HttpListenerResponse response)
        {
            //url路径
            var getSegments = request.Url.Segments;
            //查询信息
            var query = request.Url.Query;
            string getFunName;
            //根据url路径确定方法名
            switch (getSegments.Length)
            {
                case 1:
                    return string.Empty;
                case 2:
                    getFunName = getSegments[1].TrimEnd('/').ToUpper() + "GET";
                    break;
                default:
                    getFunName = getSegments[1].Trim('/').ToUpper().Replace("/", "_") + "GET";
                    break;
            }
            //获取方法名相同的所有方法
            var getMis = _serviceFunArr.Where(f => f.Name.ToUpper() == getFunName).ToList();
            if (getMis.Count == 0)
            {
                response.StatusDescription = "404";
                response.StatusCode = 404;
                return "Failure to invoke service";
            }
            //具体要执行的方法
            MethodInfo mi = null;
            //参数
            object[] objArr;
            //这里为空则没有参数
            if (string.IsNullOrEmpty(query))
            {
                foreach (var m in getMis)
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
                var queryArr = query.TrimStart('?').Split('&');
                objArr = new object[queryArr.Length];
                foreach (var m in getMis)
                {
                    var ps = m.GetParameters();
                    if (ps.Length != queryArr.Length) continue;
                    //找到参数数量对应的方法
                    mi = m;
                    for (var i = 0; i < ps.Length; i++)
                    {
                        foreach (var q in queryArr)
                        {
                            //拆分参数字符串 获得方法名和值
                            try
                            {
                                var qArr = q.Split('=');
                                if (ps[i].Name != qArr[0]) continue;
                                objArr[i] = Convert.ChangeType(System.Web.HttpUtility.UrlDecode(qArr[1], Encoding.UTF8),
                                    ps[i].ParameterType);
                                break;
                            }
                            catch (Exception e)
                            {
                                Error("参数错误:" + e);
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

        /// <summary>
        /// 处理请求非Get
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private string HandleRequestNotGet(HttpListenerRequest request, HttpListenerResponse response, string httpMethod)
        {
            if (request.InputStream != null)
            {
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
                    //获取参数
                    var dataObj = JsonConvert.DeserializeObject<IDictionary<string, object>>(data);
                    //url路径
                    var postSegments = request.Url.Segments;
                    string funName;
                    //根据url路径确定方法名
                    switch (postSegments.Length)
                    {
                        case 1:
                            return string.Empty;
                        case 2:
                            funName = postSegments[1].TrimEnd('/').ToUpper() + httpMethod;
                            break;
                        default:
                            funName = postSegments[1].Trim('/').ToUpper().Replace("/", "_") + httpMethod;
                            break;
                    }
                    //获取方法名相同的所有方法
                    var mis = _serviceFunArr.Where(f => f.Name.ToUpper() == funName).ToList();
                    if (mis.Count == 0)
                    {
                        response.StatusDescription = "404";
                        response.StatusCode = 404;
                        return "Failure to invoke service";
                    }
                    //具体要执行的方法
                    MethodInfo mi = null;
                    //参数
                    object[] objArr;
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
                                foreach (var d in dataObj)
                                {
                                    //拆分参数字符串 获得方法名和值
                                    try
                                    {
                                        if (ps[i].Name != d.Key) continue;
                                        var objStr = JsonConvert.SerializeObject(d.Value);
                                        objArr[i] = JsonConvert.DeserializeObject(objStr, ps[i].ParameterType);
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Error("参数错误:" + e);
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
                    Error(string.Format("接收数据时发生错误 Url:{0} httpMethod:{1} err:{2}", request.Url, httpMethod, ex));
                    return httpMethod + " Err: data cannot be parsed";
                }
            }
            else
            {
                response.StatusDescription = "404";
                response.StatusCode = 404;
                return httpMethod+" Err: does not allow empty submission";
            }
        }

        /// <summary>
        /// 执行服务器方法
        /// </summary>
        /// <param name="mi">方法</param>
        /// <param name="pos">参数</param>
        /// <returns></returns>
        private string Fun(MethodInfo mi, object[] pos)
        {
            //创建实例
            var o = Activator.CreateInstance(_serviceClass);
            //调用方法
            var ret = mi == null ? string.Empty : JsonConvert.SerializeObject(mi.Invoke(o, pos));
            return ret;
        }
    }
}
