﻿using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using CardReading.Core;

namespace CardReading.LiangBiao
{
    [CardReaderInfo(
        "良标读卡器",
        "良标读卡器 型号 DTI-23Y",
        true)]
    public class CardReadingLiangBiao : IReadCard
    {

        private int _handle;

        private string _comPort;

        public void Ini()
        {
            _comPort = Settings.CardReaderComPort;
            const string filePath = @"C:\SendRcv.ini";
            IniHelper.CheckIni("TCPIP", "127.0.0.1", filePath, "Host");
            IniHelper.CheckIni("TimeOut", "1000", filePath, "System");
            IniHelper.Write("ComPort", _comPort, filePath, "System");
            if (!File.Exists("C://license.dat"))
            {
                File.WriteAllText("C://license.dat", @"f900f562e8df9030ecd4f576f932ebb12c449255bc67f40f2d77dd4dc6eeed0046e4eba0d3335c7133da54063e9f8d0393f9b6278b2c0d896bb63021deea30c9a2dbe808817b8dc68304866162977da93e23b6a692899fd967fea9c5c7b057ae75ac94a1dbff4ab4441b308f4b7de42b938d39dab9c6d2888716f0316b8888f227209973c906bcc5cfe8fd99e399e912fcfee8cce92acd1d35c8ac9ab0b9ebdd20ff9c8634da0a80b93092861032b5e5e03d6b8308feb920d8da85dc808daaf9c81148e302c1306d3dbe9a5ebc926f2ca7136907ff8abd19d715250dc24aeace881d42911d04dd15d359bb29ceff4070b8488a61b71602ba9c4131355e39d6e2c6ed35986bebb1ffe1c89df02ec250377da477981731b0108e26545c3ba293c0eb47c701d6bd02b3d1d4e9bbc423c60c5defd8b4064eee1c158b348b43cd4ced60f4a0d345c4f7ba094547907869ca74c0f054be6e89e43f993a79ca5d0127f28cbc69169f46b99bf3e35e7932de86f90ea394bd36e4bb0082f9cc1f71884c58d666cad2ce8c6fdef575d77ddac81f22b03740b946a79be73991d59468a8e4a7a8bc6a10b9de420c793259a45870c0770c0dffba861aab7d24ca3b3028aa17656984f2d848835aace6998df7941e8f818bba79c9de841449ac6ba6fb9ad60c1c8087093063c5de46a4ba1b7d761ab4c7950557ed362ffdd4393ed3fea673ec53578d5822f2eaf7631bf0aa3fe7ab58d293b7412f76618fa434092e4035cbde93555e87b5dd3bcc8ab302e243cd1aa328eb98de958663f24303959bc92d240b63e2c13d310c008643d4ba6d9312b0bc530770d72a4e7d4d3910aaada278c79b4b667f22df68814c16e6def0379cb2ccdb4d07d3f3b6f6c80cc20f0cd2fcfcb5d34960df86d705478c51371635556bfa4627257bfaa7188d46808eca61a30a8184f42c1cef42d69e2c7d8d849723226f1efc9177460729c16eb4ed5458533b28e5cdab8020923a7a7ed2a8e565f01ecdea34146edfc0728f0dc5f58cbef6d5d27f3da5b423ad7a030bb94fdfa5a3a8cc33377609ceb83b1daa1b6eb29b7321a6d8ade160007dde605a984c1a61240805806c7fef1ad9cc93cd1e206df8b267317a83282bbde9adbaf4d8653fa05b8624593df6c8cb096c1aff36d366dc9bef6fa82942bd64d831385fe263f66b306a524611a53eea3264dac6cb4e454f1600b2b705f7bffc34b003251d0de6c98bfbb6d05152f77d3bfdfaad455ab1590be3c162810fd98ee47342cdf02db8c25482797e63c69b8f0c7ca2390cbf520362e1c2760351b221d0311ee0c589145e7347c2d9b0c9d3258d2cfcf48845d1540340de79f586f3bde0e5bfe503eba0a6d092ba68a0234749a6542d6521b3d655d6483cf5be76436afc234c64111907a153ab34b39c9b9e023f3b601440d3080cc4430571");
            }
        }

        public IdCardInfo ReadIdCardInfo()
        {
            _handle = ICC_Reader_Open(_comPort);
            ReadIdcInfo(_handle, out var idcardinfo);
            var idCradinfo = new IdCardInfo
            {
                Name = idcardinfo.name.Trim(),
                Gender = idcardinfo.sex.Trim().Equals("1") ? "男" : "女",
                Birth = idcardinfo.birth.Trim(),
                Addr = idcardinfo.addr.Trim(),
                Pid = idcardinfo.pid.Trim(),
                Issue = idcardinfo.issue.Trim(),
                ValidStart = idcardinfo.valid_start.Trim(),
                ValidEnd = idcardinfo.valid_end.Trim()
            };
            var i = ParseToInt(idcardinfo.nation, -1);
            if (i > 0)
            {
                idCradinfo.Nation = NationalHelp.ParseNational(i);
            }
            ICC_Reader_Close(_handle);
            _handle = 0;
            return idCradinfo;
        }
        private static int ParseToInt(string val, int defaultVal = 0)
        {
            return !int.TryParse(val, out var i) ? defaultVal : i;
        }
        public SsCardInfo ReadSsCardInfo()
        {
            var bPara = new byte[1024];
            int strLength;
            long result = iReadCardBas(3, ref bPara[0]);
            for (strLength = 0; strLength < 1024; strLength++)
            {
                if (bPara[strLength] == 0)
                {
                    break;

                }
            }
            var ssCradinfoStr = Encoding.GetEncoding("gbk").GetString(bPara, 0, strLength);

            if (result < 0)
            {
                return null;
            }
            var ssCradinfos = ssCradinfoStr.Split('|');
            var ssCradinfo = new SsCardInfo
            {
                CardNo = ssCradinfos[2],
                Name = ssCradinfos[4],
                Pid = ssCradinfos[1],
                PostCode = ssCradinfos[0]
            };
            return ssCradinfo;
        }
        [DllImport(@"YiBao.dll")]
        private static extern int ICC_Reader_Open(string srcImage0Path);

        [DllImport(@"YiBao.dll")]
        private static extern int ICC_Reader_Close(int readerHandle);

        [DllImport(@"ICCDLL.dll")]
        private static extern void ReadIdcInfo(int handle, out IdcInfo tIdCardInfo);

        [DllImport(@"SSCardDriver.dll", EntryPoint = "iReadCardBas", CallingConvention = CallingConvention.Winapi)]
        private static extern int iReadCardBas(int iType, ref byte pOutInfo);
    }
}