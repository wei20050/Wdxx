using System;
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
            var result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                var dic = new Dictionary<string, string>
                {
                    {"columns_name", dr["name"].ToString()},
                    {"notnull", dr["notnull"].ToString() == "NO" ? "1" : "0"},
                    {"comments", ""}
                };
                var dataType = dr["type"].ToString();
                var pos = dataType.IndexOf("(", StringComparison.Ordinal);
                if (pos != -1) dataType = dataType.Substring(0, pos);
                dic.Add("data_type", dataType);
                dic.Add("data_scale", "");
                dic.Add("data_precision", "");

                dic.Add("constraint_type", dr["pk"].ToString() != "0" ? "P" : "");
                result.Add(dic);
            }
            return result;
        }
        #endregion

        #region 类型转换
        /// <inheritdoc />
        /// <summary>
        /// 类型转换
        /// </summary>
        public string ConvertDataType(Dictionary<string, string> column)
        {
            string dataType;
            switch (column["data_type"])
            {
                case "TEXT":
                    //dataType = column["notnull"] == "1" ? "int" : "int?";
                    dataType = "string";
                    break;
                case "INTEGER":
                    //dataType = column["notnull"] == "1" ? "int" : "int?";
                    dataType = "int?";
                    break;
                case "bigint":
                    //dataType = column["notnull"] == "1" ? "long" : "long?";
                    dataType = "long?";
                    break;
                case "decimal":
                    //dataType = column["notnull"] == "1" ? "decimal" : "decimal?";
                    dataType = "decimal?";
                    break;
                case "nvarchar":
                    dataType = "string";
                    break;
                case "varchar":
                    dataType = "string";
                    break;
                case "text":
                    dataType = "string";
                    break;
                case "ntext":
                    dataType = "string";
                    break;
                case "date":
                    dataType = "System.DateTime?";
                    break;
                case "datetime":
                    //dataType = column["notnull"] == "1" ? "DateTime" : "DateTime?";
                    dataType = "System.DateTime?";
                    break;
                default:
                    throw new Exception("Model生成器未实现数据库字段类型" + column["data_type"] + "的转换");
            }
            return dataType;
        }
        #endregion

    }
}
