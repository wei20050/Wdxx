using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using MydbEntity;

namespace WcfService
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        [WebGet]
        void Test();

        [OperationContract]
        [WebGet]
        string TestStr();

        [OperationContract]
        [WebGet]
        string Get(int id, string name);

        [OperationContract]
        [WebGet]
        user GetUser(int id, string name);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        string Post(int id, string name);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        user PostUser(int id, string name);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        int Insert(user u);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        int Delete(int id);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        int Update(user u);

        [OperationContract]
        [WebGet]
        user Select(int id,string name);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        List<user> SelectAll();
    }
}
