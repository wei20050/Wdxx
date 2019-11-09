using System.Collections.Generic;
using System.Globalization;
using NetFrameWork.Core.Mvvm;
using System.Windows.Input;
using Test.ClientWpf.Service;
using NetFrameWork.Core;
using System;
using Test.ClientWpf.WsServiceReference;

namespace Test.ClientWpf
{
    public class MainWindowModel : ViewModelBase
    {
        public MainWindowModel()
        {
            // ReSharper disable once StringLiteralTypo
            ServiceUrl = "http://localhost:61070/Ws.asmx";
            Msg = string.Empty;
            UserList = new List<user>();
        }

        #region  属性

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private string _serviceUrl;
        public string ServiceUrl
        {
            get => _serviceUrl;
            set
            {
                _serviceUrl = value;
                OnPropertyChanged(nameof(ServiceUrl));
            }
        }

        private string _msg;
        public string Msg
        {
            get => _msg;
            set
            {
                _msg = string.IsNullOrEmpty(value) ? value :  Environment.NewLine + value;
                OnPropertyChanged(nameof(Msg));
            }
        }

        private List<user> _userList;
        public List<user> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                OnPropertyChanged(nameof(UserList));
            }
        }

        private int? _editId;
        public int? EditId
        {
            get => _editId;
            set
            {
                _editId = value;
                OnPropertyChanged(nameof(EditId));
            }
        }

        #endregion

        public ICommand TestService => new DelegateCommand(o =>
        {
            Text = ServiceHelp.ServiceIni(ServiceUrl) ? @"服务已连接!" : @"等待连接... ...";
        });

        public ICommand Test => new DelegateCommand(o =>
        {
            Msg = App.CreateWsService().Test() + Msg;
        });

        public ICommand TestStr => new DelegateCommand(o =>
        {
            Msg = App.CreateWsService().TestStr(123, "一二三") + Msg;
        });

        public ICommand GetTime => new DelegateCommand(o =>
        {
            Msg = App.CreateWsService().GetTime().ToString(CultureInfo.InstalledUICulture) + Msg;
        });

        public ICommand Insert => new DelegateCommand(o =>
        {
            var user = new user { id = 1, name = "张三" };
            var ret = App.CreateWsService().Insert(user);
            Msg = ret.ToString() + Msg;
        });

        public ICommand InsertEx => new DelegateCommand(o =>
        {
            var user = new user { id = Math.Abs(CorePublic.GenerateId()), name = "李四" };
            var ret = App.CreateWsService().Insert(user);
            Msg = ret.ToString() + Msg;
        });

        public ICommand Update => new DelegateCommand(o =>
        {
            var user = new user { id = 1, name = "张修改" };
            var ret = App.CreateWsService().Update(user);
            Msg = ret.ToString() + Msg;
        });

        public ICommand UpdateEx => new DelegateCommand(o =>
        {
            var user = new user { id = EditId, name = "根修改" };
            var ret = App.CreateWsService().Update(user);
            Msg = ret.ToString() + Msg;
        });

        public ICommand Delete => new DelegateCommand(o =>
        {
            var ret = App.CreateWsService().Delete(1);
            Msg = ret.ToString() + Msg;
        });

        public ICommand Select => new DelegateCommand(o =>
        {
            var ret = App.CreateWsService().Select(1, "");
            UserList = ret;
        });

        public ICommand SelectEx => new DelegateCommand(o =>
        {
            var ret = App.CreateWsService().Select(1, "根修改");
            UserList = ret;
        });

        public ICommand SelectAll => new DelegateCommand(o =>
        {
            var ret = App.CreateWsService().SelectAll();
            UserList = ret;
        });

        public ICommand Test1 => new DelegateCommand(o =>
        {
            Msg = "Test1" + Msg;
        });

        public ICommand Test2 => new DelegateCommand(o =>
        {
            Msg = "Test2" + Msg;
        });

        public ICommand Test3 => new DelegateCommand(o =>
        {
            Msg = "Test3" + Msg;
        });

        public ICommand ClearMsg => new DelegateCommand(o =>
        {
            Msg = string.Empty;
        });


    }
}
