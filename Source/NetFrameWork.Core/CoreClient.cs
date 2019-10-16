using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Description;
using Microsoft.CSharp;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core
{

    /// <summary>
    /// 客户端通信核心
    /// </summary>
    public class CoreClient
    {

        /// <summary>
        /// 服务地址
        /// </summary>
        private readonly string _serviceUrl;

        /// <summary>
        /// 是否是webservice调用
        /// </summary>
        private readonly bool _isWebService = true;

        /// <summary>
        /// webservice反射类
        /// </summary>
        private static Type _type;

        /// <summary>
        /// WebServiceWsdl文件夹
        /// </summary>
        private const string WebServiceWsdl = "WebServiceWsdl";

        /// <summary>
        /// 带服务地址构造函数
        /// </summary>
        /// <param name="serviceUrl">服务地址</param>
        public CoreClient(string serviceUrl)
        {
            _serviceUrl = serviceUrl.TrimEnd('/');
            if (_serviceUrl.LastIndexOf("?WSDL", StringComparison.CurrentCultureIgnoreCase) <= 0)
            {
                _isWebService = false;
                return;
            }
            var fileName = CoreEncrypt.AesEncrypt(_serviceUrl, "1234567890123456");
            var wsdlPath = Path.Combine(WebServiceWsdl, fileName);
            if (!File.Exists(wsdlPath))
            {
                var file = CoreHttp.Get(_serviceUrl);
                if (!Directory.Exists(WebServiceWsdl))
                {
                    Directory.CreateDirectory(WebServiceWsdl);
                }
                File.WriteAllText(wsdlPath, file);
            }
            try
            {
                var wc = new WebClient();
                var stream = wc.OpenRead(wsdlPath);
                wc.Dispose();
                var sd = ServiceDescription.Read(stream ?? throw new InvalidOperationException());
                var classname = sd.Services[0].Name;
                var sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                var cn = new CodeNamespace();
                //生成客户端代理类代码
                var ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                var csc = new CSharpCodeProvider();
                //设定编译参数
                var plist = new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true
                };
                //动态编译后的程序集不生成可执行文件
                //动态编译后的程序集只存在于内存中，不在硬盘的文件上
                plist.ReferencedAssemblies.Add("System.dll");
                plist.ReferencedAssemblies.Add("System.XML.dll");
                plist.ReferencedAssemblies.Add("System.Web.Services.dll");
                plist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类
                var cr = csc.CompileAssemblyFromDom(plist, ccu);
                if (cr.Errors.HasErrors)
                {
                    var sb = new StringBuilder();
                    foreach (CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce);
                        sb.Append(Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                //生成代理实例，并调用方法
                var assembly = cr.CompiledAssembly;
                _type = assembly.GetType(classname, true, true);
            }
            catch (Exception e)
            {
                _type = null;
                CoreLog.Error(e, "CORE_");
            }
        }

        /// <summary>
        /// 发送请求(返回字符串)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        public string Send(string method, params object[] sendData)
        {
            return SendCore(method, 60, sendData);
        }

        /// <summary>
        /// 发送请求(返回有构造函数的类)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        public T Send<T>(string method, params object[] sendData) where T : new()
        {
            var ret = SendCore(method, 60, sendData);
            return CoreConvert.JsonDataToObj<T>(ret);
        }

        /// <summary>
        /// 发送请求(核心)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        public string SendCore(string method, int timeout, params object[] sendData)
        {
            try
            {
                //若是webservice调用 执行反射调用
                if (_isWebService)
                {
                    var ret = Fun(method, sendData);
                    return ret == null ? string.Empty : CoreConvert.ObjToJsonData(ret);
                }
                string result;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(_serviceUrl + "/WebServiceSoap");
                httpWebRequest.Timeout = 1000 * timeout;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text;charset=UTF-8";
                //这个在非GET的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据做返回数据
                httpWebRequest.ServicePoint.Expect100Continue = false;
                var dataArr = sendData == null || sendData.Length == 0 ? string.Empty : CoreConvert.ObjToJsonData(sendData.Select(CoreConvert.ObjToJsonData).ToList());
                var tmpData = new SendDataArr { Method = method, DataArr = dataArr };
                var data = Encoding.UTF8.GetBytes(CoreConvert.ObjToJsonData(tmpData));
                httpWebRequest.ContentLength = data.Length;
                var outStream = httpWebRequest.GetRequestStream();
                outStream.Write(data, 0, data.Length);
                outStream.Close();
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
                throw new Exception("HttpErr" + " uri:" + _serviceUrl + " method:" + method + " httpData:" + sendData + "err:" + ex);
            }
        }

        private static object Fun(string method, params object[] sendData)
        {
            try
            {
                var mi = _type.GetMethod(method);
                //方法不存在直接返回null
                if (mi == null)
                {
                    return null;
                }
                //暂存的xml参数组
                var sendDataXml = sendData.Select(CoreConvert.ObjToJsonData).ToList();
                //具体的参数数组声明
                var objArr = new object[sendDataXml.Count];
                //获取参数组
                var ps = mi.GetParameters();
                for (var i = 0; i < ps.Length; i++)
                {
                    //根据参数组中的类型反序列化成需要的类型
                    objArr[i] = CoreConvert.JsonDataToObj(sendDataXml[i], ps[i].ParameterType);
                }
                var obj = Activator.CreateInstance(_type);
                //执行方法
                return mi.Invoke(obj, objArr);
            }
            catch (Exception e)
            {
                CoreLog.Error(e, "CORE_");
                return null;
            }
        }
    }
}
