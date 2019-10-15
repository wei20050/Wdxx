using NetFrameWork.Database.SqlFunction;

namespace NetFrameWork.Database
{

    /// <summary>
    /// sql函数类(以MySql为基准)
    /// </summary>
    public class SqlFun
    {

        #region 基本

        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DbHelper.DbTypeEnum MDbType;

        /// <inheritdoc />
        /// <summary>
        /// 默认构造 Mysql数据库
        /// </summary>
        public SqlFun() : this(DbHelper.DbTypeEnum.Mysql) { }

        /// <summary>
        /// 构造函数 初始化sql语句与参数
        /// </summary>
        public SqlFun(DbHelper.DbTypeEnum dbType)
        {
            MDbType = dbType;
        }

        /// <summary>
        /// 获取当前函数对象
        /// </summary>
        /// <returns></returns>
        internal static SqlFunBase GetSqlFun()
        {
            SqlFunBase sqlFun;
            switch (MDbType)
            {
                case DbHelper.DbTypeEnum.Mysql:
                    sqlFun = new SqlFunBase();
                    break;
                case DbHelper.DbTypeEnum.SqlIte:
                    sqlFun = new SqLiteFunction();
                    break;
                case DbHelper.DbTypeEnum.Mssql:
                    sqlFun = new MsSqlFunction();
                    break;
                case DbHelper.DbTypeEnum.Oracle:
                    sqlFun = new OracleFunction();
                    break;
                case DbHelper.DbTypeEnum.None:
                    sqlFun = new SqlFunBase();
                    break;
                default:
                    sqlFun = new SqlFunBase();
                    break;
            }
            return sqlFun;
        }

        #endregion 基本

        #region 扩展

        /// <summary>
        /// 根据 str 截取从 start 开始 长度 length 的字符串
        /// </summary>
        /// <returns></returns>
        public static string Substr(string str, int start, int length)
        {
            return GetSqlFun().Substr(str,start,length);
        }
        
        /// <summary>
        /// 当前时间
        /// </summary>
        /// <returns></returns>
        public static string Now()
        {
            return GetSqlFun().Now();
        }

        #endregion 扩展

    }
}