using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Model生成器.DBUtil;

namespace Model生成器.DAL
{
    /// <inheritdoc />
    /// <summary>
    /// Oracle数据库DAL
    /// </summary>
    public class OracleDal : IDal
    {
        #region 获取所有表信息
        /// <inheritdoc />
        /// <summary>
        /// 获取所有表信息
        /// </summary>
        public List<Dictionary<string, string>> GetAllTables()
        {
            var dbHelper = new OracleHelper();
            var dt = dbHelper.Query(@"
                select a.TABLE_NAME,b.COMMENTS 
                from user_tables a,user_tab_comments b 
                WHERE a.TABLE_NAME=b.TABLE_NAME");
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
            var connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
            var start = connectionString.IndexOf("User Id=", StringComparison.Ordinal) + 8;
            var end = connectionString.IndexOf("Password=", StringComparison.Ordinal);
            var owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            var dbHelper = new OracleHelper();
            var dt = dbHelper.Query($@"
                select a.*,b.COMMENTS
                from user_tab_columns a, user_col_comments b
                where a.TABLE_NAME=b.TABLE_NAME and a.COLUMN_NAME=b.COLUMN_NAME 
                and a.TABLE_NAME='{tableName}'
                order by column_id");
            var result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                var dic = new Dictionary<string, string>
                {
                    {"columns_name", dr["COLUMN_NAME"].ToString()},
                    {"notnull", dr["NULLABLE"].ToString() == "N" ? "1" : "0"},
                    {"comments", dr["COMMENTS"].ToString()},
                    {"data_type", dr["DATA_TYPE"].ToString()},
                    {"data_scale", dr["DATA_SCALE"].ToString()},
                    {"data_precision", dr["DATA_PRECISION"].ToString()}
                };

                var dt2 = dbHelper.Query(string.Format(@"
                    select *
                    from user_cons_columns c,user_constraints d
                    where c.owner='{2}' and c.constraint_name=d.constraint_name
                    and c.TABLE_NAME='{0}' and c.COLUMN_NAME='{1}'", tableName, dr["COLUMN_NAME"].ToString(), owner));
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        if (dr2["CONSTRAINT_TYPE"].ToString() == "P")
                        {
                            dic.Add("constraint_type", dr2["CONSTRAINT_TYPE"].ToString());
                        }
                    }
                }
                if (!dic.ContainsKey("constraint_type"))
                {
                    dic.Add("constraint_type", "");
                }
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
                case "NUMBER":
                    if (column["data_scale"].Trim() == "0")
                    {
                        if (column["data_precision"].Trim() != "" && int.Parse(column["data_precision"].Trim()) > 9)
                        {
                            //dataType = column["notnull"] == "1" ? "long" : "long?";
                            dataType = "long?";
                        }
                        else
                        {
                            //dataType = column["notnull"] == "1" ? "int" : "int?";
                            dataType = "int?";
                        }
                    }
                    else
                    {
                        //if (column["notnull"] == "1")
                        //{
                        //    dataType = "decimal";
                        //}
                        //else
                        //{
                        //    dataType = "decimal?";
                        //}
                        dataType = "decimal?";
                    }
                    break;
                case "LONG":
                    //dataType = column["notnull"] == "1" ? "long" : "long?";
                    dataType = "long?";
                    break;
                case "VARCHAR2":
                    dataType = "string";
                    break;
                case "NVARCHAR2":
                    dataType = "string";
                    break;
                case "CHAR":
                    dataType = "string";
                    break;
                case "DATE":
                    //dataType = column["notnull"] == "1" ? "DateTime" : "DateTime?";
                    dataType = "DateTime?";
                    break;
                case "CLOB":
                    dataType = "string";
                    break;
                default:
                    throw new Exception("Model生成器未实现数据库字段类型" + column["data_type"] + "的转换");
            }
            return dataType;
        }
        #endregion

    }
}
