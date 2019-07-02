using System.Collections.Specialized;

namespace 串口测试工具
{

    public class GlobalVar
    {
        public static string ConfigStr
        {
            get { return CoreIni.Rini("ConfigStr"); }
            set
            {
                CoreIni.Wini("ConfigStr", value);
            }
        }
        public static string Com
        {
            get { return CoreIni.Rini("Com"); }
            set
            {
                CoreIni.Wini("Com", value);
            }
        }
        public static string StrText
        {
            get { return CoreIni.Rini("StrText"); }
            set
            {
                CoreIni.Wini("StrText", value);
            }
        }
        public static string ByteText
        {
            get { return CoreIni.Rini("ByteText"); }
            set
            {
                CoreIni.Wini("ByteText", value);
            }
        }
        public static StringCollection Zdy
        {
            get { return CoreIni.Rini<StringCollection>("Zdy"); }
            set
            {
                CoreIni.Wini("Zdy", value);
            }
        }
        public static bool IsHex
        {
            get { return CoreIni.Rini<bool>("IsHex"); }
            set
            {
                CoreIni.Wini("IsHex", value);
            }
        }

    }
}
