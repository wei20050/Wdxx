using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Tset.Entity;

namespace HttpService
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        [WebGet]
        void Test();

        [OperationContract]
        [WebGet]
        DateTime GetTime();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int Insert(user u);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int Delete(int id);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int Update(user u);

        [OperationContract]
        [WebGet]
        user Select(int id,string name);

        [OperationContract]
        [WebGet]
        List<user> SelectAll();
    }
}
