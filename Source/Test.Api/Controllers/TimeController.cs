using System;
using System.Web.Http;

namespace Test.Api.Controllers
{
    public class TimeController : ApiController
    {
        public DateTime Get()
        {
            return DateTime.Now;
        }
    }
}