using System;
using System.IO;
using System.Reflection;
using Client.WcfServiceReference;

namespace Client
{
    public static class GlobalVar
    {

        /// <summary>
        /// 系统资源路径
        /// </summary>
        public static string ResPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{Assembly.GetExecutingAssembly().GetName().Name}\\F213BCE5-9EC2-4C29-945B-F62C3EC00161\\");
        

        /// <summary>
        /// 服务对象
        /// </summary>
        public static ServiceClient Service;

      

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
        public static string AppDbName = Path.Combine(ResPath, DbName);

        /// <summary>
        /// 离线应用数据库版本文件路径(使用的文件)
        /// </summary>
        public static string AppDbVer = Path.Combine(ResPath, DbVer);

        /// <summary>
        /// 离线应用数据库连接字符串
        /// </summary>
        public static string DbContext = $"data source={AppDbName}";

    }
}
