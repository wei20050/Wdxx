using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using CardReading.Core;

namespace ServiceCardRead
{
    [ServiceContract]
    public class CardRead
    {
        private readonly IReadCard _irc;
       
        private CardRead()
        {
            _irc = CardReaderFactory.GetCardReader();
        }
        [OperationContract]
        [WebGet]
        public string IdCardRead()
        {
            try
            {
                Common.Log("身份证读卡被调用！");
                var ret = _irc.ReadIdCardInfo();
                if (ret == null)
                {
                    return Ini.ObjToJson(new IdCardInfo());
                }
                return Ini.ObjToJson(ret);
            }
            catch(Exception e)
            {
                Common.Log(e.Message);
                return Ini.ObjToJson(new IdCardInfo());
            }
        }
        [OperationContract]
        [WebGet]
        public string SsCardRead()
        {
            try
            {
                Common.Log("社保卡读卡被调用！");
                var ret = Ini.ObjToJson(_irc.ReadSsCardInfo());
                if (ret == null)
                {
                    return Ini.ObjToJson(new SsCardInfo());
                }
                return ret;
            }
            catch (Exception e)
            {
                Common.Log(e.Message);
                return Ini.ObjToJson(new SsCardInfo());
            }
        }
    }
}
