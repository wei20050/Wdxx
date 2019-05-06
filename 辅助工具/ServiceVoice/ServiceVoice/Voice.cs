using System;
using System.Collections.Generic;
using System.ComponentModel;
using SpeechLib;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ServiceVoice
{
    [ServiceContract]
    public class Voice
    {
        private readonly BackgroundWorker _worker;
        //语音队列
        private readonly Queue<string> _queueVoice = new Queue<string>(); 

        /// <summary>
        /// 语音阅读
        /// </summary>
        public Voice()
        {
            try
            {
                var speech = new SpVoice
                {
                    Rate = 0,
                    Volume = 100
                };
                _worker = new BackgroundWorker();
                _worker.DoWork += (sender, e) =>
                {
                    string voice;
                    while (_queueVoice != null && _queueVoice.Count != 0 &&
                           !string.IsNullOrEmpty(voice = _queueVoice.Dequeue()))
                    {
                        Thread.Sleep(300);
                        //语音阅读方法
                        speech.Speak(voice); 
                        Thread.Sleep(100);
                    }
                };
            }
            catch (Exception e)
            {
               Common.Log("语音阅读异常:"+ e);
            }
        }

        /// <summary>
        /// 语音叫号执行
        /// </summary>
        /// <param name="message">语音信息</param>
        /// <param name="callNum">播报次数</param>
        [OperationContract]
        [WebGet(UriTemplate = "Add/{message}/{callNum}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void Add(string message, string callNum)
        {
            try
            {
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
                    _queueVoice.Enqueue(message);
                }
                if (!_worker.IsBusy)
                {
                    _worker.RunWorkerAsync();
                }
            }
            catch (Exception e)
            {
                Common.Log("语音叫号执行异常:" + e);
            }
        }
    }
}
