using CardReading.Core;

namespace CardReading.LiangBiao
{
    [CardReaderInfo(
        "良标2合一读卡器",
        "型号 DTI-23Y 不能读身份证 不能读三代社保卡",
        true)]
    public class CardReadingLiangBiao2 : LiangBiaoBase
    {
    }
}