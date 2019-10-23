using NetFrameWork.Core;
using Test.Client.ServiceReference1;

namespace Test.Client
{
    public class GlobalVar
    {
        //服务对象
        public static WsSoapClient TestService = new WsSoapClient();
    }
}
