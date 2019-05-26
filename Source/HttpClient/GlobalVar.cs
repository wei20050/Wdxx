using System.IO;
using Wdxx.Core;

namespace Client
{
    public static class GlobalVar
    {
        
        /// <summary>
        /// 离线应用文件夹路径
        /// </summary>
        public static string AppDbDirectory = Path.Combine(CorePublic.AppPath, "Data\\");

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
        public static string AppDbName = Path.Combine(CorePublic.AppPath, DbName);

        /// <summary>
        /// 离线应用数据库版本文件路径(使用的文件)
        /// </summary>
        public static string AppDbVer = Path.Combine(CorePublic.AppPath, DbVer);

        /// <summary>
        /// 离线应用数据库连接字符串
        /// </summary>
        public static string DbContext = "data source=" + AppDbName;

    }
}
