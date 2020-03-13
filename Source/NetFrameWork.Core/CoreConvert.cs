using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core
{
    /// <summary>
    /// 转换核心
    /// </summary>
    public static class CoreConvert
    {
        /// <summary>
        /// 自动转换配置
        /// </summary>
        public class MapConfig
        {
            /// <summary>
            /// 从 字段
            /// </summary>
            public string MapFrom { get; set; }

            /// <summary>
            /// 到 字段
            /// </summary>
            public string MapTo { get; set; }
        }

        /// <summary>
        /// 自动转换类
        /// </summary>
        /// <typeparam name="T">转换后的类型</typeparam>
        /// <param name="objFrom">从 数据源</param>
        /// <param name="isVague">字段是否模糊匹配(不区分大小写去掉下划线)</param>
        /// <param name="mapConfigs">字段转换配置</param>
        /// <returns></returns>
        public static T Map<T>(object objFrom, bool isVague = false, params MapConfig[] mapConfigs) where T : new()
        {
            var jsonStr = ObjToJson(objFrom);
            jsonStr = JsonNull(jsonStr);
            jsonStr = JsonMember(jsonStr, mapConfigs);
            if (isVague)
            {
                var regexs = Regex.Matches(jsonStr, "\"\\w+\":");
                foreach (Match regex in regexs)
                {
                    foreach (var p in typeof(T).GetProperties())
                    {
                        var tmp = $"\"{p.Name}\":";
                        if (regex.Value.ToUpper().Replace("_", string.Empty) != tmp.ToUpper().Replace("_", string.Empty)) continue;
                        jsonStr = jsonStr.Replace(regex.Value, tmp);
                        break;
                    }
                }
            }
            var ret = JsonToObj<T>(jsonStr);
            return ret;
        }

        /// <summary>
        /// 自动转换类
        /// </summary>
        /// <param name="objFrom">从 数据源</param>
        /// <param name="objTo">到 数据源</param>
        /// <param name="isVague">字段是否模糊匹配(不区分大小写去掉下划线)</param>
        /// <param name="mapConfigs">字段转换配置</param>
        /// <returns></returns>
        public static T Map<T>(object objFrom, T objTo, bool isVague = false, params MapConfig[] mapConfigs)
        {
            var typeFrom = objFrom.GetType();
            var typeTo = typeof(T);
            var propertiesFrom = typeFrom.GetProperties();
            var propertiesTo = typeTo.GetProperties();
            foreach (var tFrom in propertiesFrom)
            {
                var tFromValue = tFrom.GetValue(objFrom, null);
                var tFromValueJson = ObjToJson(tFromValue);
                var name = tFrom.Name;
                foreach (var c in mapConfigs)
                {
                    if (c.MapFrom == name)
                    {
                        name = c.MapTo;
                    }
                }
                foreach (var tTo in propertiesTo)
                {
                    if (isVague)
                    {
                        if (name.ToUpper().Replace("_", string.Empty) == tTo.Name.ToUpper().Replace("_", string.Empty))
                        {
                            tTo.SetValue(objTo, JsonToObj(tFromValueJson, tTo.PropertyType), null);
                        }
                    }
                    else
                    {
                        if (name == tTo.Name)
                        {
                            tTo.SetValue(objTo, JsonToObj(tFromValueJson, tTo.PropertyType), null);
                        }
                    }
                }
            }
            return objTo;
        }

        /// <summary>
        /// 自动转换类(排除源数据中Null的值)
        /// </summary>
        /// <param name="objFrom">从 数据源</param>
        /// <param name="objTo">到 数据源</param>
        /// <param name="isVague">字段是否模糊匹配(不区分大小写去掉下划线)</param>
        /// <param name="mapConfigs">字段转换配置</param>
        /// <returns></returns>
        public static T MapNull<T>(object objFrom, T objTo, bool isVague = false, params MapConfig[] mapConfigs)
        {
            var typeFrom = objFrom.GetType();
            var typeTo = typeof(T);
            var propertiesFrom = typeFrom.GetProperties();
            var propertiesTo = typeTo.GetProperties();
            foreach (var tFrom in propertiesFrom)
            {
                var tFromValue = tFrom.GetValue(objFrom, null);
                var tFromValueJson = ObjToJson(tFromValue);
                if (tFromValue == null)
                {
                    continue;
                }
                var name = tFrom.Name;
                foreach (var c in mapConfigs)
                {
                    if (c.MapFrom == name)
                    {
                        name = c.MapTo;
                    }
                }
                foreach (var tTo in propertiesTo)
                {
                    if (isVague)
                    {
                        if (name.ToUpper().Replace("_", string.Empty) == tTo.Name.ToUpper().Replace("_", string.Empty))
                        {
                            tTo.SetValue(objTo, JsonToObj(tFromValueJson, tTo.PropertyType), null);
                        }
                    }
                    else
                    {
                        if (name == tTo.Name)
                        {
                            tTo.SetValue(objTo, JsonToObj(tFromValueJson, tTo.PropertyType), null);
                        }
                    }
                }
            }
            return objTo;
        }

        /// <summary>
        /// 排除json 字段为Null的值
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string JsonNull(string jsonStr)
        {
            var json = Regex.Replace(jsonStr, "\"\\w+\":null,*", match => string.Empty);
            return json.Replace(",}", "}");
        }

        /// <summary>
        /// json 字段转换为对应的字段
        /// </summary>
        /// <returns></returns>
        public static string JsonMember(string jsonStr, params MapConfig[] mapConfigs)
        {
            var json = jsonStr;
            foreach (var m in mapConfigs)
            {
                if (m == null || string.IsNullOrEmpty(m.MapFrom) || string.IsNullOrEmpty(m.MapTo))
                {
                    continue;
                }
                json = jsonStr.Replace($"\"{m.MapFrom}\":", $"\"{m.MapTo}\":");
            }
            return json;
        }

        /// <summary>
        /// XML序列化
        /// </summary>
        public static string ObjToXml(object obj)
        {
            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        public static object XmlToObj(string xmlStr, Type t)
        {
            using (var sr = new StringReader(xmlStr))
            {
                var serializer = new XmlSerializer(t);
                return serializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        public static T XmlToObj<T>(string xmlStr) where T : new()
        {
            using (var sr = new StringReader(xmlStr))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// 将json字符串中的时间戳加上当前时区
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string JsonTimeAddZone(string jsonStr)
        {
            return Regex.Replace(jsonStr, @"\\/Date\((-?\d+)\)\\/", match =>
            {
                var zone = DateTime.Now.ToString("zz00");
                var math = match.Groups[1].Value;
                return @"\/Date(" + math + zone + @")\/";
            });
        }

        /// <summary>
        /// 将json字符串中的事件戳转换成字符串时间格式
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string JsonTime(string jsonStr)
        {
            return Regex.Replace(jsonStr, @"\\/Date\((-?\d+)\)\\/", match =>
            {
                var dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            });
        }

        /// <summary>
        /// 处理掉无法反序列化的构造(wcf自动创建的实体会出现这个问题)
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static string JsonExtension(string jsonStr)
        {
            return jsonStr.Replace("\"ExtensionData\":{},", string.Empty);
        }

        /// <summary>
        /// 将JSON字符串转化为对应类型的对象
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static T JsonToObj<T>(string json)
        {
            if (json == "null")
            {
                return (T)(object)null;
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)json.Substring(1, json.Length - 2);
            }
            if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
            {
                DateTime.TryParse(json, out var ret);
                return (T)(object)ret;
            }
            var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return string.IsNullOrEmpty(json) ? default(T) : js.Deserialize<T>(json);
        }

        /// <summary>
        /// 将JSON字符串转化为对应类型的对象
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="type">要转化的类型</param>
        /// <returns>转换后的对象</returns>
        public static object JsonToObj(string json, Type type)
        {
            if (json == "null")
            {
                return null;
            }
            if (type == typeof(string))
            {
                return json.Substring(1, json.Length - 2);
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                DateTime.TryParse(json, out var ret);
                return ret;
            }
            var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return string.IsNullOrEmpty(json) ? default(object) : js.Deserialize(json, type);
        }

        /// <summary>
        /// 将任意类型对象转化为JSON字符串(排除null字段)
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>json字符串</returns>
        public static string ObjToJsonNull(object obj)
        {
            var jsonStr = ObjToJson(obj);
            return JsonNull(jsonStr);
        }

        /// <summary>
        /// 将任意类型对象转化为JSON字符串(时间处理成普通格式)
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>json字符串</returns>
        public static string ObjToJson(object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            var js = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var jsonStr = js.Serialize(obj);
            if (obj is string)
            {
                return $"\"{obj}\"";
            }
            if (obj is DateTime)
            {
                jsonStr = jsonStr.Substring(1, jsonStr.Length - 2);
            }
            return JsonTime(jsonStr);
        }

        /// <summary>
        /// 将任意类型对象转化为数据JsonData字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>json字符串</returns>
        public static string ObjToJsonData(object obj)
        {
            if (obj == null)
            {
                return "null";
            }
            if (obj is string)
            {
                return $"\"{obj}\"";
            }
            var js = new DataContractJsonSerializer(obj.GetType());
            var msObj = new MemoryStream();
            //将序列化之后的Json格式数据写入流中
            js.WriteObject(msObj, obj);
            msObj.Position = 0;
            //从0这个位置开始读取流中的数据
            var sr = new StreamReader(msObj, Encoding.UTF8);
            var json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            return json;
        }

        /// <summary>
        /// 将JsonData字符串转化为对应类型的对象
        /// </summary>
        /// <param name="jsonData">json字符串</param>
        /// <returns>转换后的对象</returns>
        public static T JsonDataToObj<T>(string jsonData)
        {
            return (T)JsonDataToObj(jsonData, typeof(T));
        }

        /// <summary>
        /// 将JsonData字符串转化为对应类型的对象
        /// </summary>
        /// <param name="jsonData">json字符串</param>
        /// <param name="type"></param>
        /// <returns>转换后的对象</returns>
        public static object JsonDataToObj(string jsonData, Type type)
        {
            if (jsonData == "null")
            {
                return null;
            }
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonData)))
            {
                var deserializer = new DataContractJsonSerializer(type);
                return deserializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// 将DataTable对象转化为实体集合
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> DataTableToListEntity<T>(DataTable table)
        {
            var jsSerializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var parentRow = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            var json = jsSerializer.Serialize(parentRow);
            return jsSerializer.Deserialize<List<T>>(json);
        }

        /// <summary>
        /// 将DataTable对象转化为实体集合
        /// </summary>
        /// <param name="table"></param>
        /// <param name="type">实体类型</param>
        /// <returns></returns>
        public static object DataTableToListEntity(DataTable table, Type type)
        {
            var jsSerializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var parentRow = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            var json = jsSerializer.Serialize(parentRow);
            return jsSerializer.Deserialize(json, type);
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
            br.Dispose();
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
            var directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var contents = Convert.FromBase64String(base64Str);
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(contents, 0, contents.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 文件转字节集
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>Base64字符串</returns>
        public static byte[] FileToBytes(string fileName)
        {
            var fs = File.OpenRead(fileName);
            var br = new BinaryReader(fs);
            var bt = br.ReadBytes(Convert.ToInt32(fs.Length));
            br.Dispose();
            fs.Close();
            fs.Dispose();
            return bt;
        }

        /// <summary>
        /// 字节集转文件
        /// </summary>
        /// <param name="bytes">Base64字符串</param>
        /// <param name="fileName">文件路径</param>
        public static void BytesToFile(byte[] bytes, string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }
        }

    }
}
