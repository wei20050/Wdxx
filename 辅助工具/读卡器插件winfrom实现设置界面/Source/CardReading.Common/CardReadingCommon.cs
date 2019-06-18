using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CardReading.Core;

namespace CardReading.Common
{
    [CardReaderInfo(
        "通用读卡器",
        "通用身份证读卡器,兼容市面上大部分身份证读卡器 需要安装通用驱动",
        false)]
    public class CardReadingCommon : IReadCard
    {
        public string ComPort { get; set; }

        public IdCardInfo ReadIdCardInfo()
        {
            try
            {
                return Read();
            }
            catch (Exception ex)
            {
                //这里不放卡或者卡没有拿起来也报错,吞掉异常
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public SsCardInfo ReadSsCardInfo()
        {
            return null;
        }

        #region 私有全局变量

        private readonly object _readingLocker = new object();
        private int _iRetUsb;
        private int _iRetCom;

        #endregion

        public void Ini()
        {
            try
            {
                for (var port = 1001; port <= 1016; ++port)
                {
                    _iRetUsb = CVR_InitComm(port);
                    if (_iRetUsb == 1)
                        break;
                }
                if (_iRetUsb == 1) return;
                for (var port = 1; port <= 4; ++port)
                {
                    _iRetCom = CVR_InitComm(port);
                    if (_iRetCom == 1)
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        public IdCardInfo Read()
        {
            if (!Monitor.TryEnter(_readingLocker)) return null;
            try
            {
                if (_iRetCom == 1 || _iRetUsb == 1)
                {
                    CVR_Authenticate();
                    return CVR_Read_Content(1) == 1 ? FillData() : null;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("初始化失败！");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                Monitor.Exit(_readingLocker);
            }
        }
        private static IdCardInfo FillData()
        {
            try
            {
                var buffer = new byte[38862];
                var strLen1 = 38862;
                GetBMPData(ref buffer[0], ref strLen1);
                var bytes1 = new byte[30];
                var strLen2 = 30;
                GetPeopleName(ref bytes1[0], ref strLen2);
                var bytes2 = new byte[36];
                var strLen3 = 36;
                GetPeopleIDCode(ref bytes2[0], ref strLen3);
                var bytes3 = new byte[30];
                var strLen4 = 3;
                GetPeopleNation(ref bytes3[0], ref strLen4);
                var bytes4 = new byte[30];
                var strLen5 = 16;
                GetStartDate(ref bytes4[0], ref strLen5);
                var bytes5 = new byte[30];
                var strLen6 = 16;
                GetPeopleBirthday(ref bytes5[0], ref strLen6);
                var bytes6 = new byte[70];
                var strLen7 = 70;
                GetPeopleAddress(ref bytes6[0], ref strLen7);
                var bytes7 = new byte[30];
                var strLen8 = 16;
                GetEndDate(ref bytes7[0], ref strLen8);
                var bytes8 = new byte[70];
                var strLen9 = 70;
                GetDepartment(ref bytes8[0], ref strLen9);
                var bytes9 = new byte[30];
                var strLen10 = 3;
                GetPeopleSex(ref bytes9[0], ref strLen10);
                var bytes10 = new byte[70];
                CVR_GetSAMID(ref bytes10[0]);

                var result = new IdCardInfo
                {
                    Addr = GetReaderString(bytes6),
                    Gender = GetReaderString(bytes9),
                    Birth = GetReaderString(bytes5),
                    Issue = GetReaderString(bytes8),
                    Pid = GetReaderString(bytes2),
                    Name = GetReaderString(bytes1),
                    Nation = GetReaderString(bytes3),
                    ValidStart = GetReaderString(bytes4),
                    ValidEnd = GetReaderString(bytes7)
                };
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private static string GetReaderString(byte[] bytes)
        {
            return Encoding.Unicode.GetString(bytes).Replace("\0", "").Trim();
        }

        [DllImport("termb.dll", CharSet = CharSet.Auto)]
        public static extern int CVR_InitComm(int port);

        [DllImport("termb.dll", CharSet = CharSet.Auto)]
        public static extern int CVR_Authenticate();

        [DllImport("termb.dll", CharSet = CharSet.Auto)]
        public static extern int CVR_Read_Content(int active);

        [DllImport("termb.dll", CharSet = CharSet.Auto)]
        public static extern int CVR_CloseComm();

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetBMPData(ref byte bmpbuffer, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto)]
        public static extern int GetPeopleName(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto)]
        public static extern int GetPeopleNation(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPeopleBirthday(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPeopleAddress(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPeopleIDCode(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetDepartment(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetStartDate(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetEndDate(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPeopleSex(ref byte strTmp, ref int strLen);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CVR_GetSAMID(ref byte strTmp);

        [DllImport("termb.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetManuID(ref byte strTmp);
    }
}
