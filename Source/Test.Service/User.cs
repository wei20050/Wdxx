using System.Collections.Generic;
using System.Web.Services;
using Test.Service.Entity;

namespace Test.Service
{
    public partial class Ws
    {

        [WebMethod]
        public bool Insert(User u)
        {
            return _db.Insert(u);
        }

        [WebMethod]
        public bool Delete(int id)
        {
            return _db.Delete<User>(p => p.id == id);
        }

        [WebMethod]
        public bool Update(User u)
        {
            return _db.Update(u);
        }

        [WebMethod]
        public List<User> Select(int id, string name)
        {
            return _db.SelectAll<User>(p => p.id == id || p.name == name);
        }

        [WebMethod]
        public List<User> SelectAll()
        {
            return _db.SelectAll<User>();
        }
    }
}