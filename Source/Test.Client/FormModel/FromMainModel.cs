using System;
using System.Collections.Generic;
using NetFrameWork.Core2;
using Test.Client.Service;
using Test.Entity;

namespace Test.Client.FormModel
{
    public class FromMainModel : FormBaseModel
    {
        public string Url { get; set; } = "http://localhost:64276";
        public string Id { get; set; }
        public string Xxk { get; set; }
        public IEnumerable<user> Users { get; set; }

        public void ServiceIni()
        {
            ServiceHelp.ServiceIni(Url);
        }

        public void Qk()
        {
            Xxk = null;
        }
        public void GetTest()
        {
            var ret = Api.GetTest();
            Xxk = ret + Environment.NewLine + Xxk;
        }
        public void GetTime()
        {
            var ret = Api.GetTime();
            Xxk = ret.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + Xxk;
        }

        public void GetUser()
        {
            var ret = Api.GetUser();
            Users = ret;
        }

        public void GetUser1()
        {
            var ret = Api.GetUser(Id);
            Users = new List<user> { ret };
        }

        public void PostUser()
        {
            var ret = Api.PostUser(new user
            {
                id = Id,
                name = "张三"
            });
            Xxk = ret + Environment.NewLine + Xxk;
        }
        public void PostUser2()
        {
            var ret = Api.PostUser(new user
            {
                id = Math.Abs(CorePublic.GenerateId()).ToString(),
                name = "李四"
            });
            Xxk = ret + Environment.NewLine + Xxk;
        }
        public void PutUser()
        {
            var ret = Api.PutUser(new user
            {
                id = Id,
                name = "张修改"
            });
            Xxk = ret + Environment.NewLine + Xxk;
        }
        public void PutUser2()
        {
            var ret = Api.PutUser(new user
            {
                id = Id,
                name = "李修改"
            });
            Xxk = ret + Environment.NewLine + Xxk;
        }

        public void DeleteUser()
        {
            var ret = Api.DeleteUser(Id);
            Xxk = ret + Environment.NewLine + Xxk;
        }

    }
}
