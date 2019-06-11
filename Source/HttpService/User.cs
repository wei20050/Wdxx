using System.Collections.Generic;
using Tset.Entity;
using Wdxx.Database;

namespace HttpService
{
    public partial class Service
    {
        
        public int Insert(user u)
        {
            return _db.Insert(u);
        }

        public int Delete(int id)
        {
            return _db.Delete<user>(new Sql().AddField("id").Equal(id));
        }

        public int Update(user u)
        {
            return _db.Update(u);
        }

        public user Select(int id, string name)
        {
            return _db.Select<user>(new Sql().AddField("id").Equal(id).Or("name").Equal(name));
        }

        public List<user> SelectAll()
        {
            return _db.SelectAll<user>();
        }
    }
}