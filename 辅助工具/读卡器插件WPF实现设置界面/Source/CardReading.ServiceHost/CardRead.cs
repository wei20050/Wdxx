using CardReading.Core;
using System;

namespace CardReading.ServiceHost
{
    public class CardRead
    {
        private readonly IReadCard _irc;

        public CardRead()
        {
            _irc = CardReaderFactory.GetCardReader();
        }
        public string IdCardRead()
        {
            try
            {
                var ret = _irc.ReadIdCardInfo();
                return ret == null
                    ? "{\"Addr\":\"\",\"Birth\":\"\",\"Gender\":\"\",\"Issue\":\"\",\"Name\":\"\",\"Nation\":\"\",\"Pid\":\"\",\"ValidEnd\":\"\",\"ValidStart\":\"\"}"
                    : "{\"Addr\":\"" + ret.Addr + "\",\"Birth\":\"" + ret.Birth + "\",\"Gender\":\"" +
                      ret.Gender + "\",\"Issue\":\"" + ret.Issue + "\",\"Name\":\"" + ret.Name +
                      "\",\"Nation\":\"" + ret.Nation + "\",\"Pid\":\"" + ret.Pid +
                      "\",\"ValidEnd\":\"" + ret.ValidEnd + "\",\"ValidStart\":\"" + ret.ValidStart +
                      "\"}";
            }
            catch (Exception e)
            {
                Common.Error(e.Message);
                return
                    "{\"Addr\":\"\",\"Birth\":\"\",\"Gender\":\"\",\"Issue\":\"\",\"Name\":\"\",\"Nation\":\"\",\"Pid\":\"\",\"ValidEnd\":\"\",\"ValidStart\":\"\"}";
            }
        }
        public string SsCardRead()
        {
            try
            {
                var ret = _irc.ReadSsCardInfo();
                return ret == null
                    ? "{\"CardNo\":\"\",\"Name\":\"\",\"Pid\":\"\",\"PostCode\":\"\"}"
                    : "{\"CardNo\":\"" + ret.CardNo + "\",\"Name\":\"" + ret.Name + "\",\"Pid\":\"" + ret.Pid +
                      "\",\"PostCode\":\"" + ret.PostCode + "\"}";
            }
            catch (Exception e)
            {
                Common.Error(e.Message);
                return "{\"CardNo\":\"\",\"Name\":\"\",\"Pid\":\"\",\"PostCode\":\"\"}";
            }
        }
    }
}
