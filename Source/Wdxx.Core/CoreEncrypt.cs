using System;
using System.Security.Cryptography;
using System.Text;

namespace Wdxx.Core
{

    /// <summary>
    /// 加密核心
    /// </summary>
    public static class CoreEncrypt
    {

        /// <summary>
        /// 核心加密
        /// </summary>
        /// <param name="strText">待加密的文本</param>
        /// <returns></returns>
        public static string Core(string strText)
        {
            return Md5(AesEncrypt(Md5(strText),Md5("25281998-0E06-4F81-9D3C-2860C65F7B05"))).Substring(8,18);
        }

        /// <summary>
        /// 32位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5(string input)
        {
            var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.Default.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.AppendFormat("{0:X2}", t);
            }
            return sb.ToString();
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="key">加密的key(必须是16的整数倍)</param>
        /// <returns></returns>
        public static string AesEncrypt(string text, string key)
        {
            try
            {
                var keyArray = Encoding.UTF8.GetBytes(key);
                var toEncryptArray = Encoding.UTF8.GetBytes(text);
                var rDel = new RijndaelManaged
                {
                    Key = keyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                var cTransform = rDel.CreateEncryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text">解密字符</param>
        /// <param name="key">解密的key(必须是16的整数倍)</param>
        /// <returns></returns>
        public static string AesDecrypt(string text, string key)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Convert.FromBase64String(text);
            var rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            var cTransform = rDel.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

    }
}