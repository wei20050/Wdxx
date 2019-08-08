using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Model生成器.DBUtil;

namespace Model生成器.DAL
{
    /// <summary>
    /// MSSQL数据库DAL
    /// </summary>
    public class MssqlDal : IDal
    {
        #region 获取所有表信息
        /// <summary>
        /// 获取所有表信息
        /// </summary>
        public List<Dictionary<string, string>> GetAllTables()
        {
            var dbHelper = new MssqlHelper();
            var dt = dbHelper.Query(@"
                SELECT tbs.name as TABLE_NAME,ds.value as COMMENTS 
                FROM sys.tables tbs
                left join sys.extended_properties ds on ds.major_id=tbs.object_id 
                Where ds.minor_id=0");
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
            var dbHelper = new MssqlHelper();
            var dt = dbHelper.Query($@"
                select c.name,c.is_nullable,ds.value,ts.name as column_type,c.max_length,c.precision,c.scale
                from sys.columns c
                left join sys.extended_properties ds on ds.major_id=c.object_id and ds.minor_id=c.column_id
                left join sys.types ts on c.system_type_id=ts.system_type_id and ts.user_type_id=c.user_type_id
                left join sys.tables tbs on tbs.object_id=c.object_id
                where tbs.name='{tableName}' 
                order by c.column_id");
            var dtPk = dbHelper.Query($@"
                select b.column_name 
                from  information_schema.table_constraints a
                inner join information_schema.constraint_column_usage b
                on a.constraint_name = b.constraint_name
                where a.constraint_type = 'PRIMARY KEY' 
                and a.table_name = '{tableName}'");
            var strPk = string.Empty;
            if (dtPk.Rows.Count > 0)
            {
                strPk = dtPk.Rows[0]["column_name"].ToString();
            }
            var result = new List<Dictionary<string, string>>();
            foreach (DataRow dr in dt.Rows)
            {
                var dic = new Dictionary<string, string>
                {
                    {"columns_name", dr["name"].ToString()},
                    {"notnull", dr["is_nullable"].ToString() == "False" ? "1" : "0"},
                    {"comments", dr["value"].ToString()}
                };
                var dataType = dr["column_type"].ToString();
                dic.Add("data_type", dataType);
                dic.Add("data_scale", dr["scale"].ToString());
                dic.Add("data_precision", dr["precision"].ToString());

                dic.Add("constraint_type", dr["name"].ToString() == strPk ? "P" : "");
                result.Add(dic);
            }
            return result;
        }
        #endregion

        #region 类型转换
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
