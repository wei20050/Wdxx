using System;
using System.Collections.Generic;
using NetFrameWork.Core2;
using Test.Entity;

namespace Test.Client
{
    public class Api
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public static string Url { private get; set; }

        public static string GetTest()
        {
            return CoreWebApi.Get<string>(Url);
        }

        public static DateTime GetTime()
        {
            return CoreWebApi.Get<DateTime>(Url);
        }

        public static IEnumerable<user> GetUser()
        {
            return CoreWebApi.Get<IEnumerable<user>>(Url);
        }

        public static user GetUser(string id)
        {
            return CoreWebApi.Get<user>(Url, id);
        }

        public static bool PostUser(user u)
        {
            return CoreWebApi.Post<bool>(Url, u);
        }

        public static bool DeleteUser(string id)
        {
            return CoreWebApi.Delete<bool>(Url, id);
        }

        public static bool PutUser(user u)
        {
            return CoreWebApi.Put<bool>(Url, u);
        }
    }
}
