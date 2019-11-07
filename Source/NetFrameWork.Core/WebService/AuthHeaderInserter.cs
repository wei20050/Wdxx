using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace NetFrameWork.Core.WebService
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthHeaderInserter : IClientMessageInspector
    {
        /// <summary>
        /// 
        /// </summary>
        public string Authorization { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // 很诡异的问题，用visual studio启动或者attach的时候一切正常
            // 但是编译出来的exe报错->没有httpRequest属性
            // 猜测原因是vs会在http请求里面注入VsDebuggerCausalityData头信息，此时有httpRequest属性
            // 但是exe直接起的情况下，运行至此的时候还没有httpRequest
            if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                request.Properties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());
            }

            //Get the HttpRequestMessage property from the message
            if (!(request.Properties[HttpRequestMessageProperty.Name] is HttpRequestMessageProperty httpReq))
            {
                httpReq = new HttpRequestMessageProperty();
                request.Properties.Add(HttpRequestMessageProperty.Name, httpReq);
            }
            httpReq.Headers["Authorization"] = Authorization;
            return null;
        }

    }
}