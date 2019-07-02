using System.Collections.Generic;
using System.Web.Services;
using Tset.Entity;
using Wdxx.Database;

namespace Test.Service
{
    public partial class Ws
    {

        [WebMethod]
        public int Insert(user u)
        {
            return _db.Insert(u);
        }

        [WebMethod]
        public int Delete(int id)
        {
            return _db.Delete<user>(new Sql().AddField("id").Equal(id));
        }

        [WebMethod]
        public int Update(user u)
        {
            return _db.Update(u);
        }

        [WebMethod]
        public List<user> Select(int id, string name)
        {
            return _db.SelectAll<user>(new Sql().AddField("id").Equal(id).Or("name").Equal(name));
        }

        [WebMethod]
        public List<user> SelectAll()
        {
            return _db.SelectAll<user>();
        }
    }
}