using System;

namespace Model生成器.DAL
{
    /// <summary>
    /// 数据库操作工厂类
    /// </summary>
    public class DalFactory
    {
        /// <summary>
        /// 创建Dal
        /// </summary>
        /// <param name="databaseType">数据库类型，如SQLite、MySql</param>
        public static IDal CreateDal(string databaseType)
        {
            switch (databaseType.ToLower())
            {
                case "mysql":
                    return new MySqlDal();
                case "sqlite":
                    return new SqLiteDal();
                case "oracle":
                    return new OracleDal();
                case "mssql":
                    return new MssqlDal();
                default:
                    throw new Exception("数据库类型错误");
            }
        }
    }
}
