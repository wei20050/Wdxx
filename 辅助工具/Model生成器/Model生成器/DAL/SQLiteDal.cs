using System.Collections.Generic;
using System.Data;
using System.Linq;
using Model生成器.DBUtil;

namespace Model生成器.DAL
{
    /// <inheritdoc />
    /// <summary>
    /// SQLite数据库DAL
    /// </summary>
    public class SqLiteDal : IDal
    {
        #region 获取所有表名
        /// <inheritdoc />
        /// <summary>
        /// 获取数据库名
        /// </summary>
        public List<Dictionary<string, string>> GetAllTables()
        {
            var sqliteHelper = new SqLiteHelper();
            var dt = sqliteHelper.Query("select tbl_name from sqlite_master where type='table'");
            return (from DataRow dr in dt.Rows select new Dictionary<string, string> {{"table_name", dr["tbl_name"].ToString()}, {"comments", ""}}).ToList();
        }
        #endregion

        #region 获取表的所有字段名及字段类型
        /// <inheritdoc />
        /// <summary>
        /// 获取表的所有字段名及字段类型
        /// </summary>
        public List<Dictionary<string, string>> GetAllColumns(string tableName)
        {
            var sqliteHelper = new SqLiteHelper();
            var dt = sqliteHelper.Query("PRAGMA table_info('" + tableName + "')");
            return (from DataRow dr in dt.Rows
                select new Dictionary<string, string>
                {
                    {"columns_name", dr["name"].ToString()},
                    {"notnull", dr["notnull"].ToString() == "1" ? "1" : "0"},
                    {"comments", ""},
                    {"data_type", "string"},
                    {"data_scale", ""},
                    {"data_precision", ""},
                    {"constraint_type", dr["pk"].ToString() == "1" ? "P" : ""}
                }).ToList();
        }
        #endregion

        #region 类型转换
        /// <inheritdoc />
        /// <summary>
        /// 类型转换
        /// </summary>
        public string ConvertDataType(Dictionary<string, string> column)
        {
            return "string";
        }
        #endregion

    }
}
