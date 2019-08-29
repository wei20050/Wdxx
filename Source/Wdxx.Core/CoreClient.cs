using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Description;
using Microsoft.CSharp;

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
        /// webservice类
        /// </summary>
        private static Type _type;

        /// <summary>
        /// 带服务地址构造函数
        /// </summary>
        /// <param name="serviceUrl">服务地址</param>
        public CoreClient(string serviceUrl)
        {
            ServiceUrl = serviceUrl.TrimEnd('/');
            try
            {
                //获取WSDL
                var wc = new WebClient();
                var stream = wc.OpenRead(ServiceUrl + "?WSDL");
                wc.Dispose();
                if (stream == null)
                {
                    _type = null;
                    return;
                }
                var sd = ServiceDescription.Read(stream);
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
                var cplist = new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true
                };
                //动态编译后的程序集不生成可执行文件
                //动态编译后的程序集只存在于内存中，不在硬盘的文件上
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类
                var cr = csc.CompileAssemblyFromDom(cplist, ccu);
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
                CoreLog.Error(e);
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
            var ret = SendCore(method, sendData);
            if (string.IsNullOrEmpty(ret))
            {
                return string.Empty;
            }
            return (string)CoreConvert.JsonDataToObj(ret, typeof(string));
        }

        /// <summary>
        /// 发送请求(返回有构造函数的类)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        public T Send<T>(string method, params object[] sendData) where T : new()
        {
            var ret = SendCore(method, sendData);
            return (T)CoreConvert.JsonDataToObj(ret, typeof(T));
        }

        /// <summary>
        /// 发送请求(核心)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        private string SendCore(string method, params object[] sendData)
        {
            try
            {
                //若存在webservice类 执行反射调用
                if (_type != null)
                {
                    var ret = Fun(method, sendData);
                    return ret == null ? string.Empty : CoreConvert.ObjToJsonData(ret);
                }
                string result;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(ServiceUrl + "/WebSrviceSoap");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text;charset=UTF-8";
                //这个在非GET的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据做返回数据
                httpWebRequest.ServicePoint.Expect100Continue = false;
                var datas = sendData == null || sendData.Length == 0 ? string.Empty : CoreConvert.ObjToJsonData(sendData.Select(CoreConvert.ObjToJsonData).ToList());
                var tmpData = new SendData { Method = method, Datas = datas };
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
                throw new Exception("HttpErr" + " uri:" + ServiceUrl + " method:" + method + " httpData:" + sendData + "err:" + ex);
            }
        }

        private static object Fun(string method, params object[] sendData)
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
    }
}
