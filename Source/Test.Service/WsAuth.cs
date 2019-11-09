using System;
using NetFrameWork.Core;
using NetFrameWork.Core.WebService;
using Test.Service.Dto.ForClient;

namespace Test.Service
{
    public class WsAuth
    {
        public static void Authenticate(bool isAuth)
        {
            if (!isAuth)
            {
                return;
            }
            AuthHelper.AuthIni();
            if (!IsTokenValid())
            {
                throw new Exception("401");
            }
        }

        public static bool IsTokenValid()
        {
            try
            {
                var userInfoJson = CoreEncrypt.AesDecrypt(AuthHelper.GetAuth(), GlobalConst.AesKey);
                var userInfo = CoreConvert.JsonToObj<UserInfo>(userInfoJson);
                var text = CoreEncrypt.AesDecrypt(userInfo.Token, CoreEncrypt.Md5(userInfo.UserName));
                var expiredTime = DateTime.Parse(text);
                return expiredTime >= DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                CoreLog.Error(ex);
                return false;
            }
        }

        public static string CreateToken(UserInfo userInfo)
        {
            var token = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            return CoreEncrypt.AesEncrypt(token, CoreEncrypt.Md5(userInfo.UserName));
        }
    }
}