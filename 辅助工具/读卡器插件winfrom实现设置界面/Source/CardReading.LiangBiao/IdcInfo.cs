#region namespace

using System.Runtime.InteropServices;

#endregion

namespace CardReading.LiangBiao
{
    /// <summary>
    /// 身份证信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct IdcInfo
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string name;
        /// <summary>
        /// 性别
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string sex;
        /// <summary>
        /// 民族
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public string nation;
        /// <summary>
        /// 生日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string birth;
        /// <summary>
        /// 住址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 71)]
        public string addr;
        /// <summary>
        /// 身份证号码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 19)]
        public string pid;
        /// <summary>
        /// 发卡机关
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string issue;
        /// <summary>
        /// 有效期开始时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string valid_start;
        /// <summary>
        /// 有效期结束时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string valid_end;
    }
}