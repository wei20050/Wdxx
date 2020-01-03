using System.Windows;

namespace 天域取色器
{
    public class GlobalVar
    {
        private static double? _w;
        public static double? W => _w ?? (_w = SystemParameters.PrimaryScreenWidth);
        private static double? _h;
        public static double? H => _h ?? (_h = SystemParameters.PrimaryScreenHeight);
    }
}
