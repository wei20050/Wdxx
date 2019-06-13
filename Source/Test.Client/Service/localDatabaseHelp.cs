using System;
using System.IO;
using System.Linq;
using Wdxx.Core;

namespace Test.Client.Service
{
    public static class LocalDatabaseHelp
    {
        /// <summary>
        /// 离线应用文件夹路径
        /// </summary>
        public static string AppDbDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\");

        /// <summary>
        /// 离线数据库文件路径(生成的源文件)
        /// </summary>
        public static string DbName = "Data\\mydb.db";

        /// <summary>
        /// 离线数据库版本文件路径(生成的源文件)
        /// </summary>
        public static string DbVer = "Data\\dbver.txt";

        /// <summary>
        /// 离线应用数据库文件路径(使用的文件)
        /// </summary>
        public static string AppDbName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);

        /// <summary>
        /// 离线应用数据库版本文件路径(使用的文件)
        /// </summary>
        public static string AppDbVer = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbVer);

        /// <summary>
        /// 离线应用数据库连接字符串
        /// </summary>
        public static string DbContext = "data source=" + AppDbName;
        /// <summary>
        /// 设置数据库
        /// </summary>
        public static void SetDatabase()
        {
            //开始创建离线库文件 不存在库的情况
            if (!File.Exists(AppDbName))
            {
                if (!Directory.Exists(AppDbDirectory))
                {
                    Directory.CreateDirectory(AppDbDirectory);
                }
                CoreLog.Info("复制离线数据库从{" + DbName + "} => {" + AppDbName + "}");
                File.Copy(DbName, AppDbName);
                File.Copy(DbVer, AppDbVer);
            }
            //判断离线库是否有更新 若有更新离线数据库
            var dbVer = Convert.ToInt32(File.ReadAllText(DbVer));
            if (!File.Exists(AppDbVer) || Convert.ToInt32(File.ReadAllText(AppDbVer)) < dbVer)
            {
                //更新版本文件
                File.WriteAllText(AppDbVer, dbVer.ToString());
                //备份数据库
                File.Copy(AppDbName, AppDbName + DateTime.Now.ToString("yyyyMMddHHmm"));
                //更新数据库
                File.Copy(DbName, AppDbName, true);
                //获取数据库文件夹中的文件集合
                var files = Directory.GetFiles(AppDbDirectory).ToList();
                //对获取到的文件名进行排序按照名称排序后 创建时间早的数据库排在前面
                files.Sort();
                //集合中排除版本文件
                files.Remove(AppDbVer);
                //集合中排除更新后的数据库文件
                files.Remove(AppDbName);
                //保留的备份数
                const int filen = 5;
                //当备份超过5个 删除第六个(因每次备份肯定是一个 所以清理备份也清理一个)
                if (files.Count > filen)
                {
                    File.Delete(files[0]);
                }
            }

            //设置数据库连接字符串
            CoreIni.Wini("connectionString", DbContext, AppDomain.CurrentDomain.BaseDirectory + "Database.ini", "DbContext");
            CoreIni.Wini("providerName", "System.Data.SQLite", AppDomain.CurrentDomain.BaseDirectory + "Database.ini", "DbContext");
            CoreIni.Wini("sqlLog", "1", AppDomain.CurrentDomain.BaseDirectory + "Database.ini", "DbContext");
        }
    }
}
