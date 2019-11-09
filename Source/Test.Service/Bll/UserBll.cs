using System.Collections.Generic;
using Test.Service.Entity;

namespace Test.Service.Bll
{
    public class UserBll
    {
        private static UserBll _bll;

        public static UserBll GetInstance(bool isAuth = true)
        {
            WsAuth.Authenticate(isAuth);
            return _bll ??= new UserBll();
        }

        public bool Insert(user u)
        {
            return DbHelper.Db.Insert(u);
        }

        public bool Delete(int id)
        {
            return DbHelper.Db.Delete<user>(p => p.id == id);
        }

        public bool Update(user u)
        {
            return DbHelper.Db.Update(u);
        }

        public List<user> Select(int id, string name)
        {
            return DbHelper.Db.SelectAll<user>(p => p.id == id || p.name == name);
        }

        public List<user> SelectAll()
        {
            return DbHelper.Db.SelectAll<user>();
        }
    }
}