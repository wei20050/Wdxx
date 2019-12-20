// ReSharper disable UnusedMember.Global

using System.Web;

namespace NetFrameWork.Core.WebService
{
    /// <summary>
    /// 宿主帮助类
    /// </summary>
    public class HostHelper
    {
        /// <summary>
        /// webservice返回json标志
        /// </summary>
        public const string WebServiceReturnJson = "|WebService_Return_Json|";
        /// <summary>
        /// 返回json字符串
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string ReturnJson(string jsonStr,HttpContext context)
        {
            if (context.Handler == null) ReturnJson(jsonStr);
            context.Response.Write(jsonStr);
            context.Response.End();
            return string.Empty;
        }
        /// <summary>
        /// 返回json字符串
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string ReturnJson(string jsonStr)
        {
            return $"{WebServiceReturnJson}{jsonStr}{WebServiceReturnJson}";
        }
    }
}
