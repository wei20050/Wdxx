using System;
using System.Collections.Generic;
using Tset.Entity;
using Wdxx.Core;
using Wdxx.Database;

namespace HttpService
{
    public class Service : IService
    {
        public void Test()
        {

        }
        public DateTime TestStr(DateTime dt)
        {
            return DateTime.Now;
        }

        public Service()
        {
            _db.SqlLog = true;
        }

        private readonly DbHelper _db = new DbHelper();
        public string Get(int id, string name)
        {
            return "{id:" + id + " name:" + name + "}";
        }

        public user GetUser(int id, string name)
        {
            return new user {id = id + 111, name = "GetName:" + name};
        }

        public string Post(int id, string name)
        {
            return "id:" + id + " name:" + name;
        }

        public user PostUser(int id, string name)
        {
            return new user { id = id + 111, name = "GetName:" + name };
        }

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

        public user Select(int id,string name)
        {
            return _db.Select<user>(new Sql().AddField("id").Equal(id).Or("name").Equal(name));
        }

        public List<user> SelectAll()
        {
            return _db.SelectAll<user>();
        }

    }
}
