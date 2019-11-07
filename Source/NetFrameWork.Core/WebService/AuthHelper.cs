using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace NetFrameWork.Core.WebService
{
    /// <summary>
    /// 验证辅助类
    /// </summary>
    public class AuthHelper
    {

        /// <summary>
        /// 初始化验证
        /// </summary>
        public static void AuthIni()
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.User?.Identity?.Name)) return;
            if (!HttpContext.Current.Request.Headers.AllKeys.ToList().Contains("Authorization"))
            {
                throw new Exception("401");
            }
            var authHeader = HttpContext.Current.Request.Headers["Authorization"];
            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(authHeader), null);
        }

        /// <summary>
        /// 获取验证字符串
        /// </summary>
        public static string GetAuth()
        {
            return string.IsNullOrEmpty(HttpContext.Current.User?.Identity?.Name) ? string.Empty : HttpContext.Current.User?.Identity?.Name;
        }

        /// <summary>
        /// 创建验证用Behavior
        /// </summary>
        /// <param name="authorization">验证文本</param>
        /// <returns></returns>
        public static AuthHeaderBehavior CreateAuthHeaderBehavior(string authorization)
        {
            var inserter = new AuthHeaderInserter { Authorization = authorization };
            return new AuthHeaderBehavior(inserter);
        }
    }
}
