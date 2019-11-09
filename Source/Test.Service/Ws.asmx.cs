using System;
using System.Collections.Generic;
using System.Web.Services;
using Test.Service.Bll;
using Test.Service.Dto.ForClient;
using Test.Service.Entity;

namespace Test.Service
{
    [WebService]
    public class Ws : WebService
    {

        [WebMethod]
        public string Test()
        {
            return "Success";
        }

        [WebMethod]
        public UserInfo GetUserInfo(UserInfo userInfo)
        {
            return new UserInfo
            {
                UserName = userInfo.UserName,
                Token = WsAuth.CreateToken(userInfo)
            };
        }

        [WebMethod]
        public string TestStr(int id, string name)
        {
            return $"id = {id}, name = {name}";
        }

        [WebMethod]
        public DateTime GetTime()
        {
            return DateTime.Now;
        }

        [WebMethod]
        public bool Insert(user u)
        {
            return UserBll.GetInstance().Insert(u);
        }

        [WebMethod]
        public bool Delete(int id)
        {
            return UserBll.GetInstance().Delete(id);
        }

        [WebMethod]
        public bool Update(user u)
        {
            return UserBll.GetInstance().Update(u);
        }

        [WebMethod]
        public List<user> Select(int id, string name)
        {
            return UserBll.GetInstance().Select(id, name);
        }

        [WebMethod]
        public List<user> SelectAll()
        {
            return UserBll.GetInstance().SelectAll();
        }

    }
}
