using System;
using System.Configuration;
using System.IO;

namespace Test.Client.Service
{
    public static class LocalDatabaseHelp
    {

        /// <summary>
        /// 离线数据库文件路径(生成的源文件)
        /// </summary>
        public static string DbName = "Data\\mydb.db";

        /// <summary>
        /// 离线应用数据库文件路径(使用的文件)
        /// </summary>
        public static string AppDbName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);

        /// <summary>
        /// 离线应用数据库连接字符串
        /// </summary>
        public static string DbContext = "data source=" + AppDbName;

        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        public static void SetDatabase()
        {
            var clientConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            clientConfig.ConnectionStrings.ConnectionStrings["DbContext"].ConnectionString = DbContext;
            clientConfig.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

    }
}
