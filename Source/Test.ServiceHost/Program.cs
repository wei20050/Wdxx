using System;
using Wdxx.Core;

namespace Test.ServiceHost
{
    internal class Program
    {
        private static void Main()
        {
            string ConfigPath = Environment.CurrentDirectory + "\\config.ini";
            var port = Ini.Rini("webconfig", "port", ConfigPath);
            if (string.IsNullOrEmpty(port))
            {
                port = "80";
                Ini.Wini("webconfig", "port", "80", ConfigPath);
            }
            var chh = new CoreHttpHost(typeof(TestService), int.Parse(port));
            Console.WriteLine("服务已开启,地址   " + chh.Open());
            Console.Read();
        }
    }
}
