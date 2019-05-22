using System;
using Wdxx.Core;

namespace Test.ServiceHost
{
    internal class Program
    {
        private static void Main()
        {
            var chh = new CoreHttpHost(typeof(TestService),888);
            Console.WriteLine("服务已开启,地址   " + chh.Open());
            Console.Read();
        }
    }
}
