using System;
using System.Web.Services;

namespace Test.Service
{
    public partial class Ws
    {
        [WebMethod]
        public DateTime GetTime()
        {
            return DateTime.Now;
        }

    }
}