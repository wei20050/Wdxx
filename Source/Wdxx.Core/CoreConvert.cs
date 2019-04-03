using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;

namespace Wdxx.Core
{
    /// <summary>
    /// 转换核心
    /// </summary>
    public static class CoreConvert
    {

        /// <summary>
        /// 将JSON数据转化为对应的类型  
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static T JsonToObj<T>(string jsonStr)
        {
            if (typeof(T) == typeof(DateTime))
            {
                return (T) (object) Convert.ToDateTime(jsonStr);
            }
            return string.IsNullOrEmpty(jsonStr) ? default(T) : new JavaScriptSerializer().Deserialize<T>(jsonStr);
        }

        /// <summary>
        /// 将对应的类型转化为JSON字符串
        /// </summary>
        /// <param name="jsonObject">要转换的类型</param>
        /// <returns>json字符串</returns>
        public static string ObjToJson(object jsonObject)
        {
            //单独的时间格式不支持反序列化 这里直接转string
            if (jsonObject is DateTime)
            {
                return jsonObject.ToString();
            }
            return new JavaScriptSerializer().Serialize(jsonObject);
        }

        /// <summary>
        /// 实体集合转DataTable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">要转换的集合</param>
        /// <param name="propertyName">需要的字段 (若没有此参数 转换所有字段)</param>
        /// <returns></returns>
        public static DataTable ToTable<T>(IList<T> list, params string[] propertyName)
        {
            var propertyNameList = new List<string>();
            if (propertyName != null)
            {
                propertyNameList.AddRange(propertyName);
            }
            var result = new DataTable();
            if (list.Count > 0)
            {
                var propertys = list[0].GetType().GetProperties();
                foreach (var pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        var colType = pi.PropertyType;
                        if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        result.Columns.Add(pi.Name, colType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                        {
                            var colType = pi.PropertyType;
                            if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                colType = colType.GetGenericArguments()[0];
                            }
                            result.Columns.Add(pi.Name, colType);
                        }
                    }
                }
                foreach (var t in list)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            var obj = pi.GetValue(t, null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (!propertyNameList.Contains(pi.Name)) continue;
                            var obj = pi.GetValue(t, null);
                            tempList.Add(obj);
                        }
                    }
                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        /// DataTable转实体集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="table">datatable对象</param>
        /// <returns></returns>
        public static List<T> DataTableTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }
            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }
            return ConvertTo<T>(rows);
        }

        //DataRow集合转实体集合
        private static List<T> ConvertTo<T>(IList<DataRow> rows)
        {
            List<T> list = null;
            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }
            return list;
        }
        //DataRow转实体
        private static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row == null) return obj;
            obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in row.Table.Columns)
            {
                var prop = obj.GetType().GetProperty(column.ColumnName);
                try
                {
                    var value = row[column.ColumnName];
                    if (prop != null) prop.SetValue(obj, value, null);
                }
                catch(Exception ex)
                {
                    CoreLog.Error(column.ColumnName + " 转换失败:" + ex);
                }
            }
            return obj;
        }

        /// <summary>
        /// 文件转Base64字符串
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>Base64字符串</returns>
        public static string FileToBase64(string fileName)
        {
            var fs = File.OpenRead(fileName);
            var br = new BinaryReader(fs);
            var bt = br.ReadBytes(Convert.ToInt32(fs.Length));
            fs.Close();
            fs.Dispose();
            return Convert.ToBase64String(bt);
        }

        /// <summary>
        /// Base64字符串转文件
        /// </summary>
        /// <param name="base64Str">Base64字符串</param>
        /// <param name="fileName">文件路径</param>
        public static void Base64ToFile(string base64Str, string fileName)
        {
            var contents = Convert.FromBase64String(base64Str);
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(contents, 0, contents.Length);
                fs.Flush();
            }
        }
    }
}
