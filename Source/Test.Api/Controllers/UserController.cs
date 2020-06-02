using System.Collections.Generic;
using System.Web.Http;
using Test.Entity;

namespace Test.Api.Controllers
{
    public class UserController : ApiController
    {

        public IEnumerable<user> Get()
        {
            return DbHelper.Db.SelectAll<user>();
        }

        public user Get(string id)
        {
            return DbHelper.Db.Select<user>(p => p.id == id);
        }

        public bool Post(user u)
        {
            return DbHelper.Db.Insert(u);
        }

        public bool Delete(string id)
        {
            return DbHelper.Db.Delete<user>(p => p.id == id);
        }

        public bool Put([FromBody]user u)
        {
            return DbHelper.Db.Update(u);
        }

    }
}