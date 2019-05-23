using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Wdxx.Core;

namespace Client.Service
{
    public static class LocalDatabaseHelp
    {
        /// <summary>
        /// 设置数据库
        /// </summary>
        public static void SetDatabase()
        {
            //开始创建离线库文件 不存在库的情况
            if (!File.Exists(GlobalVar.AppDbName))
            {
                if (!Directory.Exists(GlobalVar.AppDbDirectory))
                {
                    Directory.CreateDirectory(GlobalVar.AppDbDirectory);
                }
                CoreLog.Info("复制离线数据库从{" + GlobalVar.DbName + "} => {" + GlobalVar.AppDbName + "}");
                File.Copy(GlobalVar.DbName, GlobalVar.AppDbName);
                File.Copy(GlobalVar.DbVer, GlobalVar.AppDbVer);
            }
            //判断离线库是否有更新 若有更新离线数据库
            var dbVer = Convert.ToInt32(File.ReadAllText(GlobalVar.DbVer));
            if (!File.Exists(GlobalVar.AppDbVer) || Convert.ToInt32(File.ReadAllText(GlobalVar.AppDbVer)) < dbVer)
            {
                //更新版本文件
                File.WriteAllText(GlobalVar.AppDbVer, dbVer.ToString());
                //备份数据库
                File.Copy(GlobalVar.AppDbName, GlobalVar.AppDbName + DateTime.Now.ToString("yyyyMMddHHmm"));
                //更新数据库
                File.Copy(GlobalVar.DbName, GlobalVar.AppDbName, true);
                //获取数据库文件夹中的文件集合
                var files = Directory.GetFiles(GlobalVar.AppDbDirectory).ToList();
                //对获取到的文件名进行排序按照名称排序后 创建时间早的数据库排在前面
                files.Sort();
                //集合中排除版本文件
                files.Remove(GlobalVar.AppDbVer);
                //集合中排除更新后的数据库文件
                files.Remove(GlobalVar.AppDbName);
                //保留的备份数
                const int filen = 5;
                //当备份超过5个 删除第六个(因每次备份肯定是一个 所以清理备份也清理一个)
                if (files.Count > filen)
                {
                    File.Delete(files[0]);
                }
            }
            //设置数据库连接字符串
            var clientConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            clientConfig.ConnectionStrings.ConnectionStrings["DbContext"].ConnectionString = GlobalVar.DbContext;
            clientConfig.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");

        }
    }
}
