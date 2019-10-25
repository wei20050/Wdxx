using System.Collections.Generic;
using System.Web.Services;
using Test.Service.Entity;

namespace Test.Service
{
    public partial class Ws
    {

        [WebMethod]
        public bool Insert(user u)
        {
            return _db.Insert(u);
        }

        [WebMethod]
        public bool Delete(int id)
        {
            return _db.Delete<user>(p => p.id == id);
        }

        [WebMethod]
        public bool Update(user u)
        {
            return _db.Update(u);
        }

        [WebMethod]
        public List<user> Select(int id, string name)
        {
            return _db.SelectAll<user>(p => p.id == id || p.name == name);
        }

        [WebMethod]
        public List<user> SelectAll()
        {
            return _db.SelectAll<user>();
        }
    }
}