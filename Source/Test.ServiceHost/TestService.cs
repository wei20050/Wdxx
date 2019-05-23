using System;
using System.Collections.Generic;
using Tset.Entity;
using Wdxx.Database;

namespace Test.ServiceHost
{
    public class TestService
    {
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        private readonly DbHelper _db = new DbHelper();

        public TestService()
        {
            //这里设置数据库记录日志开启(一般这个配置放到配置文件)
            _db.SqlLog = true;
        }
        public DateTime TestGet()
        {
            return DateTime.Now;
        }
        public string TestGet(int id, string msg)
        {
            return "get id :" + id + " msg:" + msg;
        }

        public DateTime TestPost()
        {
            return DateTime.Now;
        }

        public string TestPost( int id,string msg)
        {
            return "post id :" + id + " msg:" + msg;
        }

        public int UserPost(user u)
        {
            return _db.Insert(u);
        }

        public int UserPut(user u)
        {
            return _db.Update(u);
        }

        public int UserDelete(int id)
        {
            return _db.Delete<user>(new Sql().AddField("id").Equal(id));
        }

        public user UserGet(int id)
        {
            return _db.Select<user>(new Sql().AddField("id").Equal(id));
        }

        public user UserGet(int id,string name)
        {
            return _db.Select<user>(new Sql().AddField("id").Equal(id).And("name").Equal(name));
        }

        public List<user> UserGet()
        {
            return _db.SelectAll<user>();
        }

    }
}
