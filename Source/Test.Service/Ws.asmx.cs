using System;
using System.Collections.Generic;
using System.Web.Services;
using NetFrameWork.Database;
using Tset.Entity;

namespace Test.Service
{
    [WebService]
    public partial class Ws : WebService
    {
        private readonly ORM _db = new ORM();

        [WebMethod]
        public EpdResult Test(TestEnum te,List<DateTime> lt)
        {
           throw new Exception("服务端异常了"); 
            return new EpdResult
            {
                code = te.ToString(),
                data = new List<user>()
            };
        }

        [WebMethod]
        public user TestStr(int id,string name)
        {
            return new user {id = id, name = name};
        }
    }

    public enum TestEnum
    {
        a=1,
        b=2,
        c=3
    }
    public class EpdResult
    {
        public const string SuccessfulCode = "0";

        public string code { get; set; }
        public string msg { get; set; }
        public List<user> data { get; set; }

        public bool IsSuccessful()
        {
            return code == SuccessfulCode;
        }
    }
}
