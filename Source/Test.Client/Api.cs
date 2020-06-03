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
            return CoreWebApi.Send<string>(Url,null,null,1);
        }

        public static DateTime GetTime()
        {
            return CoreWebApi.Send<DateTime>(Url);
        }

        public static IEnumerable<user> GetUser()
        {
            return CoreWebApi.Send<IEnumerable<user>>(Url);
        }

        public static user GetUser(string id)
        {
            return CoreWebApi.Send<user>(Url, id);
        }

        public static bool PostUser(user u)
        {
            return CoreWebApi.Send<bool>(Url, null, u);
        }

        public static bool DeleteUser(string id)
        {
            return CoreWebApi.Send<bool>(Url, id);
        }

        public static bool PutUser(user u)
        {
            return CoreWebApi.Send<bool>(Url, null, u);
        }
    }
}
