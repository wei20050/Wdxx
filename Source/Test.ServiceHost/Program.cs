using System;
using Wdxx.Core;

namespace Test.ServiceHost
{
    internal class Program
    {
        private static void Main()
        {
            var chh = new CoreHost(typeof(HttpService.IService), typeof(HttpService.Service), "http://127.0.0.1:8080/Test");
            chh.Opened += (o,e) =>
            {
                Console.WriteLine("服务已开启,地址   " + chh.OpenHost());
            };
            Console.Read();
        }
    }
}
