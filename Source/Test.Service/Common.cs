using System;
using System.Web.Services;

namespace Test.Service
{
    public partial class Ws
    {

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public DateTime GetTime()
        {
            return DateTime.Now;
        }

    }
}