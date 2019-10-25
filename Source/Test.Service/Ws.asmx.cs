using System.Web.Services;
using NetFrameWork.Database;

namespace Test.Service
{
    [WebService]
    public partial class Ws : WebService
    {
        private readonly ORM _db = new ORM();

        [WebMethod]
        public void Test()
        {
        }

        [WebMethod]
        public string TestStr(int id, string name)
        {
            return $"id = {id}, name = {name}";
        }
    }

}
