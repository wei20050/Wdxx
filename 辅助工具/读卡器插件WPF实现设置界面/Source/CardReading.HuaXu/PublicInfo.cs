#region namespace

using System.Runtime.InteropServices;

#endregion

namespace CardReading.HuaXu
{
    /// <summary>
    /// 社保卡开放信息
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct PublicInfo
    {
        /// <summary>
        ///卡号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)] [FieldOffset(0)]
        public string CardNo;
        /// <summary>
        ///姓名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)] [FieldOffset(8)]
        public string Name;
        /// <summary>
        ///性别
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)] [FieldOffset(40)]
        public string Sex;
        /// <summary>
        ///身份证号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 19)] [FieldOffset(40)]
        public string PersonalID;
        /// <summary>
        ///联系电话
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] [FieldOffset(80)]
        public string Phone;
        /// <summary>
        ///通信地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] [FieldOffset(80)]
        public string Address;
        /// <summary>
        ///邮政编码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)] [FieldOffset(80)]
        public string PostCode;
    }
}