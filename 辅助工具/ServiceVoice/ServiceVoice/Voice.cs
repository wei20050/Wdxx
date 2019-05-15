using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ServiceVoice
{
    [ServiceContract]
    public class Voice
    {
        //语音队列
        private static readonly Queue<string> QueueVoice = new Queue<string>();
        private static SpeechSynthesizer _speech;
        private static Thread _t;
        /// <summary>
        /// 语音阅读
        /// </summary>
        public Voice()
        {
            try
            {
                if (_speech == null)
                {
                    _speech = new SpeechSynthesizer
                    {
                        Rate = 0,
                        Volume = 100
                    };
                }
                if (_t != null) return;
                _t = new Thread(Call) { IsBackground = true };
                _t.Start();
            }
            catch (Exception e)
            {
                Common.Log("语音阅读异常:" + e);
            }
        }
        private static void Call()
        {
            while (true)
            {
                string voice;
                if (QueueVoice != null && QueueVoice.Count != 0 &&
                   !string.IsNullOrEmpty(voice = QueueVoice.Dequeue()))
                {
                    Thread.Sleep(300);
                    _speech.Speak(voice);
                    Thread.Sleep(100);
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// 语音叫号执行
        /// </summary>
        /// <param name="message">语音信息</param>
        /// <param name="callNum">播报次数</param>
        [OperationContract]
        [WebGet(UriTemplate = "Add/{message}/{callNum}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string Add(string message, string callNum)
        {
            try
            {
                Common.Log("语音叫号被调用！");
                int cNum;
                try
                {
                    cNum = Convert.ToInt32(callNum);
                }
                catch
                {
                    cNum = 1;
                }
                for (var i = 0; i < cNum; i++)
                {
                    QueueVoice.Enqueue(message);
                }
            }
            catch (Exception e)
            {
                Common.Log("语音叫号执行异常:" + e);
                return "语音叫号执行异常:" + e;
            }
            return "语音叫号执行成功";
        }
    }
}
