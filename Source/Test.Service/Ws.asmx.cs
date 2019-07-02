using System.Web.Services;
using Tset.Entity;
using Wdxx.Database;

namespace Test.Service
{
    [WebService]
    public partial class Ws : WebService
    {
        private readonly DbHelper _db = new DbHelper();

        [WebMethod]
        public string Test()
        {
            return "Success";
        }

        [WebMethod]
        public user TestStr(int id,string name)
        {
            return new user {id = id, name = name};
        }
    }
}
