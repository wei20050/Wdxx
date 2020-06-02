using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Test.Client.Service
{
    public static class LocalDatabaseHelp
    {

        /// <summary>
        /// 离线数据库文件路径(生成的源文件)
        /// </summary>
        // ReSharper disable once StringLiteralTypo
        public static string DbName = "Data\\mydb.db";

        /// <summary>
        /// 离线应用数据库文件路径(使用的文件)
        /// </summary>
        public static string AppDbName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);

        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        public static void SetDatabase()
        {
            typeof(ConfigurationElementCollection)
                .GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(ConfigurationManager.ConnectionStrings, false);
            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings
            {
                ConnectionString = "1",
                Name = "SqlLog"
            });
            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings
            {
                ConnectionString = $"Data Source={AppDbName};",
                ProviderName = "System.Data.SQLite",
                Name = "DbContext"
            });
        }
    }
}