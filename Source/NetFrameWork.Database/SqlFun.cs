using NetFrameWork.Database.SqlFunction;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Database
{

    /// <summary>
    /// sql函数类(以MySql为基准)
    /// </summary>
    public class SqlFun
    {

        #region 基本

        /// <summary>
        /// 获取当前函数对象
        /// </summary>
        /// <returns></returns>
        public static SqlFunBase GetSqlFun(ORM.DbTypeEnum mDbType = ORM.DbTypeEnum.None)
        {
            SqlFunBase sqlFun;
            switch (mDbType)
            {
                case ORM.DbTypeEnum.Mysql:
                    sqlFun = new SqlFunBase();
                    break;
                case ORM.DbTypeEnum.SqlIte:
                    sqlFun = new SqLiteFunction();
                    break;
                case ORM.DbTypeEnum.Mssql:
                    sqlFun = new MsSqlFunction();
                    break;
                case ORM.DbTypeEnum.Oracle:
                    sqlFun = new OracleFunction();
                    break;
                case ORM.DbTypeEnum.None:
                    sqlFun = new SqlFunBase();
                    break;
                default:
                    sqlFun = new SqlFunBase();
                    break;
            }
            return sqlFun;
        }

        #endregion 基本

    }
}