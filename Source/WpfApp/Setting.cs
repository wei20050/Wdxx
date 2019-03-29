using Wdxx.Core;

namespace Client
{
    public class Setting
    {

        /// <summary>
        /// 在线服务地址
        /// </summary>
        public static string ServiceUrl
        {
            get => CoreIni.Rini(nameof(ServiceUrl));
            set => CoreIni.Wini(nameof(ServiceUrl), value);
        }

        /// <summary>
        /// 在线http地址
        /// </summary>
        public static string HttpUrl
        {
            get => CoreIni.Rini(nameof(HttpUrl));
            set => CoreIni.Wini(nameof(HttpUrl), value);
        }

        /// <summary>
        /// 离线服务端口号
        /// </summary>
        public static int Port
        {
            get => CoreIni.Rini<int>(nameof(Port));
            set => CoreIni.Wini(nameof(Port),value);
        }

    }
}