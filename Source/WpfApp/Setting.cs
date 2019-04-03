using Wdxx.Core;

namespace Client
{
    public static class Setting
    {

        /// <summary>
        /// 在线服务地址
        /// </summary>
        public static string ServiceUrl
        {
            get
            {
                return CoreIni.Rini("ServiceUrl");
            }
            set
            {
                CoreIni.Wini("ServiceUrl", value);
            }
        }

        /// <summary>
        /// 在线http地址
        /// </summary>
        public static string HttpUrl
        {
            get
            {
                return CoreIni.Rini("HttpUrl");
            }
            set
            {
                CoreIni.Wini("HttpUrl", value);
            }
        }

        /// <summary>
        /// 离线服务端口号
        /// </summary>
        public static int Port
        {
            get
            {
                return CoreIni.Rini<int>("Port");
            }
            set
            {
                CoreIni.Wini("Port", value);
            }
        }
    }
}