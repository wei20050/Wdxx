using System;
using System.Runtime.InteropServices;
using System.Text;
using CardReading.Core;


namespace CardReading.HuaXu
{
    [CardReaderInfo(
        "(华旭金卡/华大身份证读卡器)",
        "华旭金卡读卡器 兼容华大身份证读卡器",
        false)]
    public class CardReadingHuaXu : IReadCard
    {

        public void Ini()
        {
            SetDllDirectory(AppDomain.CurrentDomain.BaseDirectory + @"IdCardReader\HuaXu");
            //const string filePath = @"C:\SendRcv.ini";
            //IniHelper.CheckIni("TCPIP", "127.0.0.1", filePath, "Host");
            //IniHelper.CheckIni("TimeOut", "1000", filePath, "System");
            //IniHelper.Write("ComPort", Settings.CardReaderComPort, filePath, "System");
        }
        
        public IdCardInfo ReadIdCardInfo()
        {
            var pName = new StringBuilder(100);
            var pSex = new StringBuilder(100);
            var pNation = new StringBuilder(100);
            var pBirth = new StringBuilder(100);
            var pAddress = new StringBuilder(100);
            var pCertNo = new StringBuilder(100);
            var pDepartment = new StringBuilder(100);
            var pEffectData = new StringBuilder(100);
            var pExpire = new StringBuilder(100);
            var pErrMsg = new StringBuilder(100);
            var hReader = ICC_Reader_Open(new StringBuilder("USB1"));
            PICC_Reader_ReadIDMsg(hReader, pName, pSex, pNation, pBirth, pAddress, pCertNo, pDepartment, pEffectData,
                pExpire, pErrMsg);
            ICC_Reader_Close(hReader);
            return new IdCardInfo
            {
                Name = pName.ToString().Trim(),
                Gender = pSex.ToString().Trim(),
                Nation = pNation.ToString().Trim(),
                Birth = pBirth.ToString().Trim(),
                Addr = pAddress.ToString().Trim(),
                Pid = pCertNo.ToString().Trim(),
                Issue = pDepartment.ToString().Trim(),
                ValidStart = pEffectData.ToString().Trim(),
                ValidEnd = pExpire.ToString().Trim()
            };
        }

        public SsCardInfo ReadSsCardInfo()
        {
            var bPara = new byte[1024];
            int strLength;
            var result = iReadCardBas(3, ref bPara[0]);

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
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);
        [DllImport(@"SSSE32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern int ICC_Reader_Open(StringBuilder devName);
        [DllImport(@"SSSE32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern int ICC_Reader_Close(int readerHandle);
        [DllImport(@"SSSE32.dll", CallingConvention = CallingConvention.Winapi,CharSet = CharSet.Ansi)]
        private static extern long PICC_Reader_ReadIDMsg(long readerHandle, StringBuilder pName, StringBuilder pSex, StringBuilder pNation, StringBuilder pBirth, StringBuilder pAddress, StringBuilder pCertNo, StringBuilder pDepartment, StringBuilder pEffectData, StringBuilder pExpire, StringBuilder pErrMsg);
        [DllImport(@"SSCardDriver.dll", EntryPoint = "iReadCardBas", CallingConvention = CallingConvention.Winapi)]
        private static extern int iReadCardBas(int iType, ref byte pOutInfo);
    }
}