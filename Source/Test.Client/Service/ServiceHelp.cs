﻿using System;
using System.ServiceModel;
using System.Windows.Forms;
using NetFrameWork.Core;
using Test.Client.ServiceReference1;

namespace Test.Client.Service
{
    public static class ServiceHelp
    {
        /// <summary>
        /// 是否在线
        /// </summary>
        public static bool IsOnLine = true;

        /// <summary>
        /// 服务器初始化确定在线离线
        /// </summary>
        /// <param name="url">在线服务地址</param>
        /// <returns>返回是否初始化成功</returns>
        public static bool ServiceIni(string url)
        {
            try
            {
                //检测服务是否正常连接  若无法连接 开启离线模式
                GlobalVar.TestService.Endpoint.Address = new EndpointAddress(url);
                GlobalVar.TestService.Test(TestEnum.a,null);
                IsOnLine = true;
            }
            catch (Exception ex)
            {
                CoreLog.Error(ex);
                //离线服务开启
                if (DialogResult.Yes != MessageBox.Show(@"离线中！是否还要继续？", @"提示", MessageBoxButtons.YesNo)) return false;
                IsOnLine = false;
                var httpUrl = new CoreHostWebService(typeof(Test.Service.Ws)).Open();
                GlobalVar.TestService = new WsSoapClient();
                GlobalVar.TestService.Endpoint.Address = new EndpointAddress(httpUrl);
                //设置本地数据库
                LocalDatabaseHelp.SetDatabase();
            }
            return true;
        }

    }
}
