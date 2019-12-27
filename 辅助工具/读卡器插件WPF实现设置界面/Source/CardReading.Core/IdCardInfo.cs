namespace CardReading.Core
{
    /// <summary>
    /// 身份证信息
    /// </summary>
    public class IdCardInfo
    {
        /// <summary>
        /// 住址
        /// </summary>
        public string Addr { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birth { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 发卡机关
        /// </summary>
        public string Issue { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 有效期开始时间
        /// </summary>
        public string ValidStart { get; set; }

        /// <summary>
        /// 有效期结束时间
        /// </summary>
        public string ValidEnd { get; set; }

    }
}