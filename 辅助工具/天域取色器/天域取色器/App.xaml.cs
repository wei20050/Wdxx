﻿using System.Windows;
using System.Windows.Threading;

namespace 天域取色器
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //捕获全局异常
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
