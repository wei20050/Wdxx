using System;
using System.Windows.Forms;
using NetFrameWork.Core2;

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
        public static void ServiceIni(string url)
        {
            try
            {
                //设定要连接的服务地址
                Api.Url = url;
                var test = Api.GetTest();
                Console.WriteLine(test);
                IsOnLine = true;
            }
            catch (Exception ex)
            {
                CoreLog.Error("在线异常:" + ex);
                try
                {
                    //离线服务开启
                    if (MessageBox.Show(@"离线中！是否还要继续？", @"提示", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    IsOnLine = false;
                    url = CoreWebApiHost.OpenAsync("Service\\Api\\Test.Api");
                    Api.Url = url;
                    var test = Api.GetTest();
                    Console.WriteLine(test);
                }
                catch (Exception e)
                {
                    CoreLog.Error("离线异常:" + e);
                }
            }

        }

    }
}
