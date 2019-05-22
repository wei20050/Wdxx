using System;
using Tset.Entity;

namespace Test.ServiceHost
{
    public class TestService
    {
        public void TestGet()
        {
        }

        public string TestGet( int id,string msg)
        {
            return "id :" + id + " msg:" + msg;
        }

        public DateTime DateTimeGet()
        {
            return DateTime.Now;
        }

        public string DateTimePost(user u1,user u2)
        {
            return DateTime.Now + "post:u1.name " + u1.name + "post:u2.name " + u2.name;
        }
    }
}
