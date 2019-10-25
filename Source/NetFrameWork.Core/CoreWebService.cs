using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Services.Description;
using Microsoft.CSharp;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core
{

    /// <summary>
    /// WebService动态调用核心
    /// </summary>
    public class CoreWebService
    {
        /// <summary>
        /// webservice反射类
        /// </summary>
        private static Type _type;

        /// <summary>
        /// WebServiceWsdl文件夹
        /// </summary>
        private readonly string _webServiceWsdl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebServiceWsdl");

        /// <summary>
        /// 带服务地址构造函数
        /// </summary>
        /// <param name="serviceUrl">服务地址</param>
        public CoreWebService(string serviceUrl)
        {
            try
            {
                var service = new Uri(serviceUrl);
                var fileName = $"{service.Host}_{service.Port}";
                // ReSharper disable once StringLiteralTypo
                fileName = service.Segments.Select(s => s.Trim('/')).Where(name => !string.IsNullOrEmpty(name)).Aggregate(fileName, (current, name) => current + $"_{name}").Replace(".asmx",string.Empty);
                var wsdlFile = Path.Combine(_webServiceWsdl, fileName);
                if (!File.Exists(wsdlFile))
                {
                    var file = GetWsdl(serviceUrl);
                    if (!Directory.Exists(_webServiceWsdl))
                    {
                        Directory.CreateDirectory(_webServiceWsdl);
                    }
                    File.WriteAllText(wsdlFile, file);
                }
                var wc = new WebClient();
                var stream = wc.OpenRead(wsdlFile);
                wc.Dispose();
                var sd = ServiceDescription.Read(stream ?? throw new InvalidOperationException());
                var classname = sd.Services[0].Name;
                var sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, string.Empty, string.Empty);
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
                throw new Exception("CoreWebService Err", e);
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
            try
            {
                var ret = Fun(method, sendData);
                return ret == null ? string.Empty : ObjToJsonData(ret);
            }
            catch (Exception e)
            {
                throw new Exception("CoreWebService.Send Err", e);
            }
        }

        /// <summary>
        /// 发送请求(返回有构造函数的类)
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="sendData">请求参数 (可变参数与服务端参数一致)</param>
        /// <returns></returns>
        public T Send<T>(string method, params object[] sendData) where T : new()
        {
            try
            {
                var ret = Send(method, sendData);
                return JsonDataToObj<T>(ret);
            }
            catch (Exception e)
            {
                throw new Exception("CoreWebService.Send<T> Err", e);
            }
        }

        private static object Fun(string method, params object[] sendData)
        {
            var mi = _type.GetMethod(method);
            //方法不存在直接异常
            if (mi == null)
            {
                throw new Exception("CoreWebService.Fun Err", new Exception($"There is no such method({method})"));
            }
            //暂存的xml参数组
            var sendDataXml = sendData.Select(ObjToJsonData).ToList();
            //具体的参数数组声明
            var objArr = new object[sendDataXml.Count];
            //获取参数组
            var ps = mi.GetParameters();
            for (var i = 0; i < ps.Length; i++)
            {
                //根据参数组中的类型反序列化成需要的类型
                objArr[i] = JsonDataToObj(sendDataXml[i], ps[i].ParameterType);
            }
            var obj = Activator.CreateInstance(_type);
            //执行方法
            return mi.Invoke(obj, objArr);
        }

        private static string GetWsdl(string httpUri)
        {
            httpUri = httpUri.TrimEnd('/');
            if (httpUri.LastIndexOf("?wsdl", StringComparison.CurrentCultureIgnoreCase) <= 0)
            {
                httpUri += "?wsdl";
            }
            string result;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(httpUri);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "text/xml";
            var webResponse = httpWebRequest.GetResponse();
            var httpWebResponse = (HttpWebResponse)webResponse;
            var stream = httpWebResponse.GetResponseStream();
            if (stream != null)
            {
                var streamReader = new StreamReader(stream);
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

        #region 序列化反序列化

        /// <summary>
        /// 将任意类型对象转化为数据JsonData字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>json字符串</returns>
        public static string ObjToJsonData(object obj)
        {
            if (obj is string)
            {
                return obj.ToString();
            }
            var js = new DataContractJsonSerializer(obj.GetType());
            var msObj = new MemoryStream();
            //将序列化之后的Json格式数据写入流中
            js.WriteObject(msObj, obj);
            msObj.Position = 0;
            //从0这个位置开始读取流中的数据
            var sr = new StreamReader(msObj, Encoding.UTF8);
            var json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            return json;
        }

        /// <summary>
        /// 将JsonData字符串转化为对应类型的对象
        /// </summary>
        /// <param name="jsonData">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static T JsonDataToObj<T>(string jsonData)
        {
            return (T)JsonDataToObj(jsonData, typeof(T));
        }

        /// <summary>
        /// 将JsonData字符串转化为对应类型的对象
        /// </summary>
        /// <param name="jsonData">json字符串</param>
        /// <param name="type"></param>
        /// <returns>转换后的对象</returns>
        public static object JsonDataToObj(string jsonData, Type type)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return null;
            }
            if (type == typeof(string))
            {
                return jsonData;
            }
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonData)))
            {
                var deserializer = new DataContractJsonSerializer(type);
                return deserializer.ReadObject(ms);
            }
        }

        #endregion

    }
}
