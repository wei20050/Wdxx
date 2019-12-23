using System.Collections.Generic;
using System;
using Test.ClientWpf.WsServiceReference;

namespace Test.ClientWpf
{
    public class MainWindowModel : Panuon.UI.Silver.Core.PropertyChangedBase
    {
        public MainWindowModel()
        {
            ServiceUrl = "http://localhost:61070/Ws.asmx";
            Msg = string.Empty;
            UserList = new List<user>();
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                NotifyPropertyChanged(nameof(Text));
            }
        }

        private string _serviceUrl;
        public string ServiceUrl
        {
            get => _serviceUrl;
            set
            {
                _serviceUrl = value;
                NotifyPropertyChanged(nameof(ServiceUrl));
            }
        }

        private string _msg;
        public string Msg
        {
            get => _msg;
            set
            {
                _msg = string.IsNullOrEmpty(value) ? value : value + Environment.NewLine + _msg;
                NotifyPropertyChanged(nameof(Msg));
            }
        }

        private List<user> _userList;
        public List<user> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                NotifyPropertyChanged(nameof(UserList));
            }
        }

        private int? _editId;
        public int? EditId
        {
            get => _editId;
            set
            {
                _editId = value;
                NotifyPropertyChanged(nameof(EditId));
            }
        }

    }
}
