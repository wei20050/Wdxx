using System.Collections.Generic;
using MydbEntity;
using Wdxx.Database;

namespace WcfService
{
    public class Service : IService
    {
        public void Test()
        {

        }

        public Service()
        {
            _db.SqlLog = true;
        }

        private readonly DbHelper _db = new DbHelper();
        public string Get(int id, string name)
        {
            return $"id:{id} name:{name}";
        }

        public user GetUser(int id, string name)
        {
            return new user {id = id + 111, name = $"GetName:{name}"};
        }

        public string Post(int id, string name)
        {
            return $"id:{id} name:{name}";
        }

        public user PostUser(int id, string name)
        {
            return new user { id = id + 222, name = $"GetName:{name}" };
        }

        public int Insert(user u)
        {
            return _db.Insert(u);
        }

        public int Delete(int id)
        {
            return _db.Delete<user>(new SqlTextHelper().Add("id").Equal(id));
        }

        public int Update(user u)
        {
            return _db.Update(u);
        }

        public user Select(int id,string name)
        {
            return _db.Select<user>(new SqlTextHelper().Add("id").Equal(id).Or("name").Equal(name));
        }

        public List<user> SelectAll()
        {
            return _db.SelectAll<user>();
        }

    }
}
