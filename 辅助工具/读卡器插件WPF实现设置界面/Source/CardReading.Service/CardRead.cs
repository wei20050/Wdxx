using CardReading.Core;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace CardReading.Service
{
    [ServiceContract]
    public class CardRead
    {
        private readonly IReadCard _irc;

        public CardRead()
        {
            _irc = CardReaderFactory.GetCardReader();
        }
        [OperationContract]
        [WebGet]
        public IdCardInfo IdCardRead()
        {
            try
            {
                var idCardInfo = _irc.ReadIdCardInfo();
                return idCardInfo ?? new IdCardInfo();
            }
            catch(Exception e)
            {
                Common.Error(e.Message);
                return new IdCardInfo();
            }
        }
        [OperationContract]
        [WebGet]
        public SsCardInfo SsCardRead()
        {
            try
            {
                var ret = _irc.ReadSsCardInfo();
                return ret ?? new SsCardInfo();
            }
            catch (Exception e)
            {
                Common.Error(e.Message);
                return new SsCardInfo();
            }
        }
    }
}
