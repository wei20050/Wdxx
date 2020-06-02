using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core2
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
            return Md5(AesEncrypt(Md5(strText), Md5("252819980E064F819D3C2860C65F7B05"))).Substring(8, 18);
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
        /// AES加密(加密模式ECB,填充模式pk-cs5padding,数据块128位,偏移量无,输出16进制,字符集UTF8)
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="key">加密的key(必须是16的整数倍)</param>
        /// <returns></returns>
        public static string AesEncrypt(string text, string key)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Encoding.UTF8.GetBytes(text);
            var rDel = new RijndaelManaged { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return BytesToHex(resultArray);
        }

        /// <summary>
        /// AES解密(加密模式ECB,填充模式pk-cs5padding,数据块128位,偏移量无,输出16进制,字符集UTF8)
        /// </summary>
        /// <param name="text">解密字符</param>
        /// <param name="key">解密的key(必须是16的整数倍)</param>
        /// <returns></returns>
        public static string AesDecrypt(string text, string key)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = HexToBytes(text);
            var rDel = new RijndaelManaged { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
            var cTransform = rDel.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// RSA公钥私钥类
        /// </summary>
        public struct RsaSecretKey
        {
            /// <summary>
            /// RSA公钥私钥类构造
            /// </summary>
            /// <param name="privateKey"></param>
            /// <param name="publicKey"></param>
            public RsaSecretKey(string privateKey, string publicKey)
            {
                PrivateKey = privateKey;
                PublicKey = publicKey;
            }
            /// <summary>
            /// 公钥
            /// </summary>
            public string PublicKey { get; set; }
            /// <summary>
            /// 私钥
            /// </summary>
            public string PrivateKey { get; set; }
        }

        /// <summary>
        /// 获取RSA公钥私钥
        /// </summary>
        /// <param name="keySize">the size of the key,must from 384 bits to 16384 bits in increments of 8 </param>
        /// <returns></returns>
        public static RsaSecretKey GenerateRsaSecretKey(int keySize)
        {
            var rsaKey = new RsaSecretKey();
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                rsaKey.PrivateKey = rsa.ToXmlString(true);
                rsaKey.PublicKey = rsa.ToXmlString(false);
            }
            return rsaKey;
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RsaEncrypt(string xmlPublicKey, string content)
        {
            string encryptedContent;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);
                var encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
                encryptedContent = Convert.ToBase64String(encryptedData);
            }
            return encryptedContent;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RsaDecrypt(string xmlPrivateKey, string content)
        {
            string decryptedContent;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPrivateKey);
                var decryptedData = rsa.Decrypt(Convert.FromBase64String(content), false);
                decryptedContent = Encoding.GetEncoding("utf-8").GetString(decryptedData);
            }
            return decryptedContent;
        }

        /// <summary>
        /// 16进制字符串转Byte数组
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private static byte[] HexToBytes(string hex)
        {
            var num = (int)Math.Round((double)hex.Length / 2);
            var buffer = new byte[num];
            var num3 = num - 1;
            for (var i = 0; i <= num3; i++)
            {
                var s = hex.Substring(i * 2, 2);
                buffer[i] = (byte)int.Parse(s, NumberStyles.HexNumber);
            }
            return buffer;
        }

        /// <summary>
        /// Byte数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string BytesToHex(IList<byte> bytes)
        {
            var builder = new StringBuilder();
            var num2 = bytes.Count - 1;
            for (var i = 0; i <= num2; i++)
            {
                builder.AppendFormat("{0:X2}", bytes[i]);
            }
            return builder.ToString();
        }

    }
}