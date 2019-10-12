﻿using NetFrameWork.Core.Mvvm;
using System.Windows;
using System.Windows.Input;

namespace Test.ClientWpf
{
    public class MainWindowModel : ViewModelBase
    {
        public MainWindowModel()
        {
            LabTxt = "初始化完成!";
        }
        public string LabTxt { get; set; }
        public ICommand BtnClick => new DelegateCommand(o =>
        {
            LabTxt = "修改后的显示!";
            OnPropertyChanged(nameof(LabTxt));
            MessageBox.Show("command调用成功!");
        });
    }
}
