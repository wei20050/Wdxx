﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Client.WcfServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="user", Namespace="http://schemas.datacontract.org/2004/07/MydbEntity")]
    [System.SerializableAttribute()]
    public partial class user : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string nameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> id {
            get {
                return this.idField;
            }
            set {
                if ((this.idField.Equals(value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                if ((object.ReferenceEquals(this.nameField, value) != true)) {
                    this.nameField = value;
                    this.RaisePropertyChanged("name");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WcfServiceReference.IService")]
    public interface IService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/Test", ReplyAction="http://tempuri.org/IService/TestResponse")]
        void Test();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/TestStr", ReplyAction="http://tempuri.org/IService/TestStrResponse")]
        string TestStr();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/Get", ReplyAction="http://tempuri.org/IService/GetResponse")]
        string Get(int id, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetUser", ReplyAction="http://tempuri.org/IService/GetUserResponse")]
        Client.WcfServiceReference.user GetUser(int id, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/Post", ReplyAction="http://tempuri.org/IService/PostResponse")]
        string Post(int id, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/PostUser", ReplyAction="http://tempuri.org/IService/PostUserResponse")]
        Client.WcfServiceReference.user PostUser(int id, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/Insert", ReplyAction="http://tempuri.org/IService/InsertResponse")]
        int Insert(Client.WcfServiceReference.user u);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/Delete", ReplyAction="http://tempuri.org/IService/DeleteResponse")]
        int Delete(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/Update", ReplyAction="http://tempuri.org/IService/UpdateResponse")]
        int Update(Client.WcfServiceReference.user u);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/Select", ReplyAction="http://tempuri.org/IService/SelectResponse")]
        Client.WcfServiceReference.user Select(int id, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/SelectAll", ReplyAction="http://tempuri.org/IService/SelectAllResponse")]
        System.Collections.Generic.List<Client.WcfServiceReference.user> SelectAll();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : Client.WcfServiceReference.IService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceClient : System.ServiceModel.ClientBase<Client.WcfServiceReference.IService>, Client.WcfServiceReference.IService {
        
        public ServiceClient() {
        }
        
        public ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void Test() {
            base.Channel.Test();
        }
        
        public string TestStr() {
            return base.Channel.TestStr();
        }
        
        public string Get(int id, string name) {
            return base.Channel.Get(id, name);
        }
        
        public Client.WcfServiceReference.user GetUser(int id, string name) {
            return base.Channel.GetUser(id, name);
        }
        
        public string Post(int id, string name) {
            return base.Channel.Post(id, name);
        }
        
        public Client.WcfServiceReference.user PostUser(int id, string name) {
            return base.Channel.PostUser(id, name);
        }
        
        public int Insert(Client.WcfServiceReference.user u) {
            return base.Channel.Insert(u);
        }
        
        public int Delete(int id) {
            return base.Channel.Delete(id);
        }
        
        public int Update(Client.WcfServiceReference.user u) {
            return base.Channel.Update(u);
        }
        
        public Client.WcfServiceReference.user Select(int id, string name) {
            return base.Channel.Select(id, name);
        }
        
        public System.Collections.Generic.List<Client.WcfServiceReference.user> SelectAll() {
            return base.Channel.SelectAll();
        }
    }
}
