﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Model生成器.DBUtil;

namespace Model生成器.DAL
{
    /// <summary>
    /// MySql数据库DAL
    /// </summary>
    public class MySqlDal : IDal
    {
        #region 获取所有表信息
        /// <summary>
        /// 获取所有表信息
        /// </summary>
        public List<Dictionary<string, string>> GetAllTables()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DbContext"].ToString();
            var start = connectionString.IndexOf("database=", StringComparison.Ordinal) + 9;
            var end = connectionString.IndexOf("user id=", StringComparison.Ordinal);
            var owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            var dbHelper = new MySqlHelper();
            var dt = dbHelper.Query($@"
                SELECT TABLE_NAME as TABLE_NAME,TABLE_COMMENT as COMMENTS 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = '{owner}'");
            return (from DataRow dr in dt.Rows
                select new Dictionary<string, string>
                {
                    {"table_name", dr["TABLE_NAME"].ToString()},
                    {"comments", dr["COMMENTS"].ToString()}
                }).ToList();
        }
        #endregion

        #region 获取表的所有字段名及字段类型
        /// <summary>
        /// 获取表的所有字段名及字段类型
        /// </summary>
        public List<Dictionary<string, string>> GetAllColumns(string tableName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DbContext"].ToString();
            var start = connectionString.IndexOf("database=", StringComparison.Ordinal) + 9;
            var end = connectionString.IndexOf("user id=", StringComparison.Ordinal);
            var owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            var dbHelper = new MySqlHelper();
            var dt = dbHelper.Query($@"
                select * 
                from INFORMATION_SCHEMA.Columns 
                where table_name='{tableName}' 
                and table_schema='{owner}'");
            var result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                var dic = new Dictionary<string, string>
                {
                    {"columns_name", dr["COLUMN_NAME"].ToString()},
                    {"notnull", dr["IS_NULLABLE"].ToString() == "NO" ? "1" : "0"},
                    {"comments", dr["COLUMN_COMMENT"].ToString()}
                };
                var dataType = dr["COLUMN_TYPE"].ToString();
                var pos = dataType.IndexOf("(", StringComparison.Ordinal);
                if (pos != -1) dataType = dataType.Substring(0, pos);
                dic.Add("data_type", dataType);
                dic.Add("data_scale", dr["CHARACTER_MAXIMUM_LENGTH"].ToString());
                dic.Add("data_precision", dr["NUMERIC_SCALE"].ToString());

                dic.Add("constraint_type", dr["COLUMN_KEY"].ToString() == "PRI" ? "P" : "");
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
                case "int":
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
                case "float":
                    //dataType = column["notnull"] == "1" ? "DateTime" : "DateTime?";
                    dataType = "float?";
                    break;
                default:
                    throw new Exception("Model生成器未实现数据库字段类型" + column["data_type"] + "的转换");
            }
            return dataType;
        }
        #endregion

    }
}
