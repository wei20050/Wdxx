using System;

namespace HttpService
{
    public partial class Service
    {

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetTime()
        {
            return DateTime.Now;
        }

    }
}