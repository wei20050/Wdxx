namespace CardReading.Core
{
    /// <summary>
    ///     读卡器接口
    /// </summary>
    public interface IReadCard
    {
        /// <summary>
        ///     Com 端口
        /// </summary>
        string ComPort { get; set; }

        /// <summary>
        ///     读社保卡
        /// </summary>
        /// <returns>社保卡信息</returns>
        SsCardInfo ReadSsCardInfo();

        /// <summary>
        ///     读身份证
        /// </summary>
        /// <returns>身份证信息</returns>
        IdCardInfo ReadIdCardInfo();

        /// <summary>
        ///     初始化读卡器
        /// </summary>
        void Ini();
        
    }
}