namespace CardReading.Core
{
    /// <summary>
    ///     读卡器接口
    /// </summary>
    public interface IReadCard
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Ini();

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
        
    }
}