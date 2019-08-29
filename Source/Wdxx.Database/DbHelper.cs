using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Objects.DataClasses;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable 618

namespace Wdxx.Database
{

    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class DbHelper
    {
        #region 构造

        /// <inheritdoc />
        /// <summary>
        /// 无参构造(默认数据库连接字符串名称:DbContext)
        /// </summary>
        public DbHelper() : this("DbContext") { }

        /// <summary>
        /// 带连接字符串名称构造
        /// </summary>
        /// <param name="dbConnectionName">数据库连接字符串Name</param>
        public DbHelper(string dbConnectionName)
        {
            var sqlLogCm = ConfigurationManager.ConnectionStrings["SqlLog"];
            if (sqlLogCm == null)
            {
                Error("没有找到名为:SqlLog 的连接字符串,默认不记录Sql日志");
            }
            else
            {
                _sqlLog = sqlLogCm.ConnectionString == "1";
            }
            var cm = ConfigurationManager.ConnectionStrings[dbConnectionName];
            if (cm == null)
            {
                Error("没有找到名为:" + dbConnectionName + " 的连接字符串");
                throw new Exception("没有找到名为:" + dbConnectionName + " 的连接字符串");
            }
            var pn = cm.ProviderName;
            if (pn.Contains("System.Data.SQLite"))
            {
                var sqlItePath = AppDomain.CurrentDomain.BaseDirectory + "System.Data.SQLite.dll";
                if (File.Exists(sqlItePath))
                {
                    _sqliteAss = Assembly.LoadFrom(sqlItePath);
                    MDbType = DbTypeEnum.Sqlite;
                }
                else
                {
                    Error("System.Data.SQLite.dll 文件不存在 无法支持SQLite");
                    throw new Exception("未能加载：" + sqlItePath + " 请确认此插件是否存在！");
                }
            }
            else if (pn.Contains("MySql.Data.MySqlClient"))
            {
                MDbType = DbTypeEnum.Mysql;
            }
            else if (pn.Contains("System.Data.SqlClient"))
            {
                MDbType = DbTypeEnum.Mssql;
            }
            else if (pn.Contains("System.Data.OracleClient"))
            {
                MDbType = DbTypeEnum.Oracle;
            }
            else
            {
                MDbType = DbTypeEnum.None;
                Error("没有在连接字符串中检测到 providerName 无法判断数据库类型!");
                throw new Exception("没有在连接字符串中检测到 providerName 无法判断数据库类型!");
            }
            _mConnectionString = cm.ToString();
            _mParameterMark = GetParameterMark();
        }

        #endregion

        #region 数据库类型枚举

        /// <summary>
        /// 数据库类型枚举
        /// </summary>
        public enum DbTypeEnum
        {
            /// <summary>
            /// Sqlite数据库
            /// </summary>
            Sqlite = 0,
            /// <summary>
            /// Mysql数据库
            /// </summary>
            Mysql = 1,
            /// <summary>
            /// Mssql数据库
            /// </summary>
            Mssql = 2,
            /// <summary>
            /// Oracle数据库
            /// </summary>
            Oracle = 3,
            /// <summary>
            /// 未知数据库类型
            /// </summary>
            None = 4
        }

        #endregion

        #region 变量

        /// <summary>
        /// Sqlite程序集
        /// </summary>
        private readonly Assembly _sqliteAss;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public readonly DbTypeEnum MDbType;

        /// <summary>
        /// 数据库sql操作日志(1:开启日志  0:关闭日志)
        /// </summary>
        private static bool _sqlLog;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private readonly string _mConnectionString;

        /// <summary>
        /// 事务
        /// </summary>
        [ThreadStatic]
        private static DbTransaction _mTran;

        /// <summary>
        /// 带参数的SQL插入和修改语句中，参数前面的符号
        /// </summary>
        private readonly string _mParameterMark;

        #endregion

        #region 生成变量

        #region 生成 IDbCommand

        /// <summary>
        /// 生成 IDbCommand
        /// </summary>
        private DbCommand GetCommand()
        {
            DbCommand command;
            switch (MDbType)
            {
                case DbTypeEnum.Sqlite:
                    var obj = _sqliteAss.CreateInstance("System.Data.SQLite.SQLiteCommand");
                    command = (DbCommand)obj;
                    break;
                case DbTypeEnum.Mysql:
                    command = new MySqlCommand();
                    break;
                case DbTypeEnum.Mssql:
                    command = new SqlCommand();
                    break;
                case DbTypeEnum.Oracle:
                    command = new OracleCommand();
                    break;
                case DbTypeEnum.None:
                    command = null;
                    break;
                default:
                    command = null;
                    break;
            }
            return command;
        }

        /// <summary>
        /// 生成 IDbCommand
        /// </summary>
        private DbCommand GetCommand(string sql, DbConnection conn)
        {
            DbCommand command;
            switch (MDbType)
            {
                case DbTypeEnum.Sqlite:
                    var parameters = new object[1];
                    parameters[0] = sql;
                    var obj = _sqliteAss.CreateInstance("System.Data.SQLite.SQLiteCommand", true, BindingFlags.Default, null, parameters, null, null);
                    command = (DbCommand)obj;
                    break;
                case DbTypeEnum.Mysql:
                    command = new MySqlCommand(sql);
                    break;
                case DbTypeEnum.Mssql:
                    command = new SqlCommand(sql);
                    break;
                case DbTypeEnum.Oracle:
                    command = new OracleCommand(sql);
                    break;
                case DbTypeEnum.None:
                    return null;
                default:
                    return null;
            }
            if (command == null)
            {
                return null;
            }

            command.Connection = conn;
            return command;
        }

        #endregion

        #region 生成 IDbConnection

        /// <summary>
        /// 生成 IDbConnection
        /// </summary>
        private DbConnection GetConnection()
        {
            DbConnection conn;
            try
            {
                switch (MDbType)
                {
                    case DbTypeEnum.Sqlite:
                        var parameters = new object[1];
                        parameters[0] = _mConnectionString;
                        var obj = _sqliteAss.CreateInstance("System.Data.SQLite.SQLiteConnection", true, BindingFlags.Default, null, parameters, null, null);
                        conn = (DbConnection)obj;
                        break;
                    case DbTypeEnum.Mysql:
                        conn = new MySqlConnection(_mConnectionString);
                        break;
                    case DbTypeEnum.Mssql:
                        conn = new SqlConnection(_mConnectionString);
                        break;
                    case DbTypeEnum.Oracle:
                        conn = new OracleConnection(_mConnectionString);
                        break;
                    case DbTypeEnum.None:
                        conn = null;
                        break;
                    default:
                        conn = null;
                        break;
                }
            }
            catch (Exception e)
            {
                conn = null;
                Error("生成 IDbConnection异常 请检查数据库连接字符串:" + e);
            }
            return conn;
        }

        #endregion

        #region 生成 IDbDataAdapter

        /// <summary>
        /// 生成 IDbDataAdapter
        /// </summary>
        private DbDataAdapter GetDataAdapter(DbCommand cmd)
        {
            DbDataAdapter dataAdapter;

            switch (MDbType)
            {
                case DbTypeEnum.Sqlite:
                    var obj = _sqliteAss.CreateInstance("System.Data.SQLite.SQLiteDataAdapter");
                    dataAdapter = (DbDataAdapter)obj;
                    break;
                case DbTypeEnum.Mysql:
                    dataAdapter = new MySqlDataAdapter();
                    break;
                case DbTypeEnum.Mssql:
                    dataAdapter = new SqlDataAdapter();
                    break;
                case DbTypeEnum.Oracle:
                    dataAdapter = new OracleDataAdapter();
                    break;
                case DbTypeEnum.None:
                    return null;
                default:
                    return null;
            }
            if (dataAdapter == null)
            {
                return null;
            }

            dataAdapter.SelectCommand = cmd;
            return dataAdapter;
        }

        #endregion

        #region 生成 DbParameter

        /// <summary>
        /// 生成 DbParameter
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="vallue">参数值</param>
        /// <returns></returns>
        private DbParameter GetDbParameter(string name, object vallue)
        {
            DbParameter dbParameter;

            switch (MDbType)
            {
                case DbTypeEnum.Sqlite:
                    var parameters = new object[2];
                    parameters[0] = name;
                    parameters[1] = vallue;
                    var obj = _sqliteAss.CreateInstance("System.Data.SQLite.SQLiteParameter", true, BindingFlags.Default, null, parameters, null, null);
                    dbParameter = (DbParameter)obj;
                    break;
                case DbTypeEnum.Mysql:
                    dbParameter = new MySqlParameter(name, vallue);
                    break;
                case DbTypeEnum.Mssql:
                    dbParameter = new SqlParameter(name, vallue);
                    break;
                case DbTypeEnum.Oracle:
                    dbParameter = new OracleParameter(name, vallue);
                    break;
                case DbTypeEnum.None:
                    dbParameter = null;
                    break;
                default:
                    dbParameter = null;
                    break;
            }
            return dbParameter;
        }

        #endregion

        #region 获取 ParameterMark

        /// <summary>
        /// 获取 ParameterMark(参数化查询前面的标识符)
        /// </summary>
        private string GetParameterMark()
        {
            switch (MDbType)
            {
                case DbTypeEnum.Sqlite:
                    return ":";
                case DbTypeEnum.Mysql:
                    return "@";
                case DbTypeEnum.Mssql:
                    return "@";
                case DbTypeEnum.Oracle:
                    return ":";
                case DbTypeEnum.None:
                    return ":";
                default:
                    return ":";
            }
        }

        #endregion

        #endregion 生成变量

        #region 基础方法

        #region SQL过滤，防注入

        /// <summary>
        /// SQL过滤，防注入
        /// </summary>
        /// <param name="sql">sql文本</param>
        public void SqlFilter(ref string sql)
        {
            var keywordList = new List<string>
            {
                "net localgroup",
                "net user",
                "xp_cmdshell",
                "exec",
                "execute",
                "truncate",
                "drop",
                "restore",
                "create",
                "alter",
                "rename",
                "insert",
                "update",
                "delete",
                "select"};
            var ignore = string.Empty;
            var upperSql = sql.ToUpper().Trim();
            foreach (var keyword in keywordList)
            {
                if (upperSql.IndexOf(keyword.ToUpper(), StringComparison.Ordinal) == 0)
                {
                    ignore = keyword;
                }
            }
            sql = (from keyword in keywordList where !string.Equals(ignore, keyword, StringComparison.CurrentCultureIgnoreCase) select new Regex(keyword, RegexOptions.IgnoreCase)).Aggregate(sql, (current, regex) => regex.Replace(current, string.Empty));
        }

        #endregion

        #region 执行sql 返回是否成功

        /// <summary>
        /// 执行sql 返回是否成功
        /// </summary>
        /// <param name="sqlString">sql文本</param>
        /// <returns></returns>
        public bool Exists(string sqlString)
        {
            SqlFilter(ref sqlString);
            LogInfo(sqlString);
            using (var conn = GetConnection())
            {
                using (var cmd = GetCommand(sqlString, conn))
                {
                    try
                    {
                        conn.Open();
                        var obj = cmd.ExecuteScalar();
                        return !Equals(obj, null) && !Equals(obj, DBNull.Value);
                    }
                    catch (Exception ex)
                    {
                        Error(sqlString);
                        Error(ex);
                        return false;
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
        }

        #endregion

        #region 执行一条计算查询结果语句，返回查询结果

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）
        /// </summary>
        /// <param name="sqlString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string sqlString)
        {
            SqlFilter(ref sqlString);
            LogInfo(sqlString);
            using (var conn = GetConnection())
            {
                using (var cmd = GetCommand(sqlString, conn))
                {
                    try
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }

                        var obj = cmd.ExecuteScalar();
                        if (Equals(obj, null) || Equals(obj, DBNull.Value))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (Exception ex)
                    {
                        Error(sqlString);
                        Error(ex);
                        return false;
                    }
                    finally
                    {
                        cmd.Dispose();
                    }
                }
            }
        }

        #endregion

        #region 执行SQL语句，返回影响的记录数

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sqlString, params DbParameter[] cmdParms)
        {
            LogInfo(sqlString, cmdParms);
            var conn = _mTran == null ? GetConnection() : _mTran.Connection;
            using (var cmd = GetCommand())
            {
                try
                {
                    PrepareCommand(cmd, conn, _mTran, sqlString, cmdParms);
                    var rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (Exception ex)
                {
                    LogErr(sqlString, cmdParms);
                    Error(ex);
                    return 0;
                }
                finally
                {
                    cmd.Dispose();
                    if (_mTran == null)
                    {
                        conn.Close();
                    }
                }
            }
        }

        #endregion

        #region 执行查询语句，返回IDataReader

        /// <summary>
        /// 执行查询语句，返回IDataReader ( 注意：调用该方法后，一定要对IDataReader进行Close )
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>IDataReader</returns>
        public DbDataReader ExecuteReader(string sqlString, params DbParameter[] cmdParms)
        {
            LogInfo(sqlString, cmdParms);
            var conn = GetConnection();
            var cmd = GetCommand();
            try
            {
                PrepareCommand(cmd, conn, null, sqlString, cmdParms);
                var myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (Exception ex)
            {
                LogErr(sqlString, cmdParms);
                Error(ex);
                return null;
            }

        }

        #endregion

        #region 执行查询语句，返回DataSet

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteSet(string sqlString, params DbParameter[] cmdParms)
        {
            LogInfo(sqlString, cmdParms);
            var conn = GetConnection();
            var cmd = GetCommand();
            PrepareCommand(cmd, conn, null, sqlString, cmdParms);
            using (var da = GetDataAdapter(cmd))
            {
                var ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    LogErr(sqlString, cmdParms);
                    Error(ex);
                    return null;
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
                return ds;
            }
        }

        #endregion

        #region 准备命令
        /// <summary>
        /// 包装数据库对象
        /// </summary>
        /// <param name="cmd">命令对象</param>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="trans">事物对象</param>
        /// <param name="cmdText">运行的文本</param>
        /// <param name="cmdParms">DbCommand参数</param>
        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, string cmdText, DbParameter[] cmdParms)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                cmd.CommandType = CommandType.Text;
                if (cmdParms == null)
                {
                    return;
                }

                foreach (var parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
            catch (Exception e)
            {
                Error("打开数据库异常,请检查数据库连接配置:" + e);
            }
        }

        /// <summary>
        /// 从sql中 获取sqlstr与DbParameter[]
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlstr"></param>
        /// <param name="parms"></param>
        private void GetSqlAndParms(Sql sql, out string sqlstr, out DbParameter[] parms)
        {
            sqlstr = sql.ToString();
            parms = new DbParameter[sql.ParamDict.Count];
            var i = 0;
            foreach (var p in sql.ParamDict)
            {
                //这里参数化为了匹配不同的数据库 在这里补充下参数化的参数前缀
                var parmsKey = GetParameterMark() + p.Key;
                sqlstr = sqlstr.Replace(p.Key, parmsKey);
                parms[i] = GetDbParameter(parmsKey, p.Value);
                i++;
            }
        }

        #endregion

        #endregion 基础方法

        #region 增删改查

        #region 添加

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj">要添加的实体对象</param>
        /// <returns>返回影响的行数 成功为1 失败为0</returns>
        public bool Insert(object obj)
        {
            var strSql = new StringBuilder();
            var type = obj.GetType();
            strSql.AppendFormat("insert into {0}(", type.Name);
            var propertyInfoList = GetEntityProperties(type);
            var propertyNameList = new List<string>();
            var savedCount = 0;
            foreach (var propertyInfo in propertyInfoList)
            {
                var val = propertyInfo.GetValue(obj, null);
                if (val == null)
                {
                    continue;
                }
                propertyNameList.Add(propertyInfo.Name);
                savedCount++;
            }

            strSql.AppendFormat("{0}) values ({1})", string.Join(",", propertyNameList.ConvertAll(a => "`" + a + "`").ToArray()), string.Join(",", propertyNameList.ConvertAll(a => _mParameterMark + a).ToArray()));
            var parameters = new DbParameter[savedCount];
            var k = 0;
            for (var i = 0; i < propertyInfoList.Length && savedCount > 0; i++)
            {
                var propertyInfo = propertyInfoList[i];
                var val = propertyInfo.GetValue(obj, null);
                if (val == null)
                {
                    continue;
                }

                var param = GetDbParameter(_mParameterMark + propertyInfo.Name, val);
                parameters[k++] = param;
            }
            return ExecuteSql(strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="objList">要批量添加的实体集合</param>
        /// <returns>返回影响的行数 成功为1 失败为0</returns>
        public bool Inserts<T>(List<T> objList) where T : new()
        {
            try
            {
                BeginTransaction();
                foreach (var obj in objList)
                {
                    InsertOrUpdate(obj);
                }
                CommitTransaction();
                return true;
            }
            catch (Exception e)
            {
                Error("批量添加发生异常:" + e);
                RollbackTransaction();
                return false;
            }
        }

        /// <summary>
        /// 添加或修改
        /// </summary>
        /// <param name="obj">要添加的实体对象</param>
        /// <returns>返回影响的行数 成功为1 失败为0</returns>
        public bool InsertOrUpdate(object obj)
        {
            var type = obj.GetType();
            var sql = $"select count(*) from {type.Name} where ";
            var propertyInfoList = GetEntityProperties(type);
            foreach (var propertyInfo in propertyInfoList)
            {
                var attrs = (EdmScalarPropertyAttribute[])propertyInfo.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), true);
                if (attrs.Length < 1 || !attrs[0].EntityKeyProperty)
                {
                    continue;
                }

                var val = propertyInfo.GetValue(obj, null);
                sql += $"`{propertyInfo.Name}`={val} OR ";
            }
            sql = sql.Substring(0, sql.Length - 3);
            var rd = GetSingle(sql);
            return rd.ToString() == "0" ? Insert(obj) : Update(obj);
        }

        #endregion

        #region 删除

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="T">要删除的实体类型</typeparam>
        /// <param name="conditions">删除条件(从where后面开始组装)</param>
        /// <returns></returns>
        public bool Delete<T>(string conditions)
        {
            var st = new Sql();
            st.Add(conditions);
            var ret = Delete<T>(st);
            st.Clear();
            return ret;
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="T">要删除的实体类型</typeparam>
        /// <param name="sql">sql对象(从where后面开始组装)</param>
        /// <returns></returns>
        public bool Delete<T>(Sql sql)
        {
            GetSqlAndParms(sql, out var conditions, out var parameters);
            if (string.IsNullOrEmpty(conditions))
            {
                return false;
            }

            var type = typeof(T);
            var sbSql = new StringBuilder();
            SqlFilter(ref conditions);
            sbSql.AppendFormat("delete from {0} where {1}", type.Name, conditions);
            return ExecuteSql(sbSql.ToString(), parameters) > 0;
        }

        #endregion

        #region 修改

        /// <summary>
        /// 根据实体第一个字段修改(适用于第一个字段是主键的实体)
        /// </summary>
        /// <param name="obj">要修改的实体</param>
        /// <returns></returns>
        public bool Update(object obj)
        {
            var type = obj.GetType();
            var propertyInfoList = GetEntityProperties(type);
            if (propertyInfoList.Length == 0)
            {
                return false;
            }

            var pk = propertyInfoList[0];
            var conditions = pk.Name + "='" + pk.GetValue(obj, null) + "'";
            return Update(obj, conditions);
        }

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <param name="obj">要修改的实体</param>
        /// <param name="conditions">修改的条件(从where后面开始组装)</param>
        /// <returns></returns>
        public bool Update(object obj, string conditions)
        {
            var st = new Sql();
            st.Add(conditions);
            var ret = Update(obj, st);
            st.Clear();
            return ret;
        }

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <param name="obj">要修改的实体</param>
        /// <param name="sql">sql对象(从where后面开始组装)</param>
        /// <returns></returns>
        public bool Update(object obj, Sql sql)
        {
            var strSql = new StringBuilder();
            var type = obj.GetType();
            strSql.AppendFormat("update {0} ", type.Name);
            //获取要修改的字段数
            var propertyInfoList = GetEntityProperties(type);
            var savedCount = propertyInfoList.Select(propertyInfo => propertyInfo.GetValue(obj, null))
                .Count(val => val != null);
            //这里定义参数化数组 长度加上where中的参数
            var parameters = new DbParameter[savedCount + sql.ParamDict.Count];
            //开始拼接写入字段
            strSql.Append(" set ");
            //SQL参数下标
            var k = 0;
            //写入字段拼接字符串
            var sbPros = new StringBuilder();
            for (var i = 0; i < propertyInfoList.Length && savedCount > 0; i++)
            {
                var propertyInfo = propertyInfoList[i];
                var val = propertyInfo.GetValue(obj, null);
                if (string.IsNullOrEmpty(val?.ToString()))
                {
                    continue;
                }

                sbPros.Append(string.Format(" `{0}`={1}{0},", propertyInfo.Name, _mParameterMark));
                var param = GetDbParameter(_mParameterMark + propertyInfo.Name, val);
                parameters[k++] = param;
            }
            //写入字段存在 去掉最后的逗号加到sql中
            if (sbPros.Length > 0)
            {
                strSql.Append(sbPros.ToString(0, sbPros.Length - 1));
            }
            var conditions = sql.ToString();
            //添加where中的参数
            foreach (var p in sql.ParamDict)
            {
                //这里参数化为了匹配不同的数据库 在这里补充下参数化的参数前缀
                var parmsKey = GetParameterMark() + p.Key;
                conditions = conditions.Replace(p.Key, parmsKey);
                parameters[k++] = GetDbParameter(GetParameterMark() + p.Key, p.Value);
            }
            //拼接上where语句
            strSql.AppendFormat(" where {0}", conditions);
            //执行
            return savedCount > 0 && ExecuteSql(strSql.ToString(), parameters) > 0;
        }

        #endregion

        #region 查询

        /// <summary>
        /// 根据条件查询单条数据
        /// </summary>
        /// <param name="conditions">查询条件(从where后面开始组装)</param>
        /// <returns></returns>
        public T Select<T>(string conditions) where T : new()
        {
            var st = new Sql();
            st.Add(conditions);
            var ret = Select<T>(st);
            st.Clear();
            return ret;
        }

        /// <summary>
        /// 根据条件查询单条数据
        /// </summary>
        /// <param name="sql">sql对象(从where后面开始组装)</param>
        /// <returns></returns>
        public T Select<T>(Sql sql) where T : new()
        {
            GetSqlAndParms(sql, out var conditions, out var parameters);
            var type = typeof(T);
            var sbSql = new StringBuilder();
            SqlFilter(ref conditions);
            sbSql.AppendFormat("select * from {0} where {1}", type.Name, conditions);
            return Find<T>(sbSql.ToString(), parameters);
        }

        /// <summary>
        /// 根据条件查询列表
        /// </summary>
        /// <param name="conditions">修改的条件(从where后面开始组装)</param>
        /// <returns></returns>
        public List<T> SelectAll<T>(string conditions) where T : new()
        {
            var st = new Sql();
            st.Add(conditions == "" ? "1=1" : conditions);
            var ret = SelectAll<T>(st);
            st.Clear();
            return ret;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public List<T> SelectAll<T>() where T : new()
        {
            var type = typeof(T);
            var sbSql = new StringBuilder();
            sbSql.AppendFormat("select * from {0}", type.Name);
            return FindList<T>(sbSql.ToString());
        }

        /// <summary>
        /// 根据条件查询列表
        /// </summary>
        /// <param name="sql">sql对象(从where后面开始组装)</param>
        /// <returns></returns>
        public List<T> SelectAll<T>(Sql sql) where T : new()
        {
            GetSqlAndParms(sql, out var conditions, out var parameters);
            var type = typeof(T);
            var sbSql = new StringBuilder();
            SqlFilter(ref conditions);
            var where = conditions == "" ? "" : " where " + conditions;
            sbSql.AppendFormat("select * from {0} {1}", type.Name, where);
            return FindList<T>(sbSql.ToString(), parameters);
        }

        #endregion

        #region 获取实体

        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        private T Find<T>(string sqlString, params DbParameter[] cmdParms) where T : new()
        {
            var type = typeof(T);
            var result = (T)Activator.CreateInstance(type);
            IDataReader rd = null;

            try
            {
                rd = ExecuteReader(sqlString, cmdParms);

                var propertyInfoList = GetEntityProperties(type);

                var fcnt = rd.FieldCount;
                var fileds = new List<string>();
                for (var i = 0; i < fcnt; i++)
                {
                    fileds.Add(rd.GetName(i).ToUpper());
                }

                while (rd.Read())
                {
                    IDataRecord record = rd;

                    foreach (var pro in propertyInfoList)
                    {
                        if (!fileds.Contains(pro.Name.ToUpper()) || record[pro.Name] == DBNull.Value)
                        {
                            continue;
                        }

                        pro.SetValue(result, record[pro.Name] == DBNull.Value ? null : GetReaderValue(record[pro.Name], pro.PropertyType), null);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        /// <typeparam name="T">要查询的实体类型</typeparam>
        /// <param name="sql">SQL语句(全SQL)</param>
        /// <returns></returns>
        public T FindBySql<T>(string sql) where T : new()
        {
            return Find<T>(sql);
        }

        /// <summary>
        /// 根据sql获取实体(参数化)
        /// </summary>
        /// <typeparam name="T">要查询的实体类型</typeparam>
        /// <param name="sql">sql对象(全SQL)</param>
        /// <returns></returns>
        public T FindBySql<T>(Sql sql) where T : new()
        {
            var conditions = sql.ToString();
            var parameters = new DbParameter[sql.ParamDict.Count];
            var i = 0;
            foreach (var p in sql.ParamDict)
            {
                parameters[i] = GetDbParameter(p.Key, p.Value);
                i++;
            }
            return Find<T>(conditions, parameters);
        }

        #endregion

        #region 获取实体集合

        /// <summary>
        /// 根据参数化sql获取列表
        /// </summary>
        private List<T> FindList<T>(string sqlString, params DbParameter[] cmdParms) where T : new()
        {
            var list = new List<T>();
            IDataReader rd = null;

            try
            {
                rd = ExecuteReader(sqlString, cmdParms);

                if (typeof(T) == typeof(int))
                {
                    while (rd.Read())
                    {
                        list.Add((T)rd[0]);
                    }
                }
                else if (typeof(T) == typeof(string))
                {
                    while (rd.Read())
                    {
                        list.Add((T)rd[0]);
                    }
                }
                else
                {
                    var propertyInfoList = typeof(T).GetProperties();

                    var fcnt = rd.FieldCount;
                    var fileds = new List<string>();
                    for (var i = 0; i < fcnt; i++)
                    {
                        fileds.Add(rd.GetName(i).ToUpper());
                    }

                    while (rd.Read())
                    {
                        IDataRecord record = rd;
                        object obj = new T();


                        foreach (var pro in propertyInfoList)
                        {
                            if (!fileds.Contains(pro.Name.ToUpper()) || record[pro.Name] == DBNull.Value)
                            {
                                continue;
                            }

                            pro.SetValue(obj, record[pro.Name] == DBNull.Value ? null : GetReaderValue(record[pro.Name], pro.PropertyType), null);
                        }
                        list.Add((T)obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
            finally
            {
                if (rd != null && !rd.IsClosed)
                {
                    rd.Close();
                    rd.Dispose();
                }
            }
            return list;
        }

        /// <summary>
        /// 根据sql获取列表
        /// </summary>
        /// <typeparam name="T">要查询的列表实体类型</typeparam>
        /// <param name="sql">SQL语句(全SQL)</param>
        /// <returns></returns>
        public List<T> FindListBySql<T>(string sql) where T : new()
        {
            return FindList<T>(sql);
        }

        /// <summary>
        /// 根据sql获取实体
        /// </summary>
        /// <typeparam name="T">要查询的实体类型</typeparam>
        /// <param name="sql">sql对象(全SQL)</param>
        /// <returns></returns>
        public List<T> FindListBySql<T>(Sql sql) where T : new()
        {
            var conditions = sql.ToString();
            var parameters = new DbParameter[sql.ParamDict.Count];
            var i = 0;
            foreach (var p in sql.ParamDict)
            {
                parameters[i] = GetDbParameter(p.Key, p.Value);
                i++;
            }
            return FindList<T>(conditions, parameters);
        }

        #endregion

        #endregion 增删改查

        #region 单表分页查询

        #region 分页获取列表

        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        /// <returns></returns>
        public List<T> FindPageBySql<T>(string sql, string orderby, int pageSize, int currentPage, out int rows, params DbParameter[] cmdParms) where T : new()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var commandText = "select count(*) from (" + sql + ") T";
                IDbCommand cmd = GetCommand(commandText, connection);
                rows = int.Parse(cmd.ExecuteScalar().ToString());
                return FindList<T>(GetPageSql(sql, orderby, pageSize, currentPage), cmdParms);
            }
        }

        #endregion

        #region 分页获取DataSet

        /// <summary>
        /// 分页(任意entity，尽量少的字段)
        /// </summary>
        public DataSet FindPageBySql(string sql, string orderby, int pageSize, int currentPage, out int totalCount, params DbParameter[] cmdParms)
        {
            DataSet ds;
            using (var connection = GetConnection())
            {
                connection.Open();
                var commandText = "select count(*) from (" + sql + ") T";
                IDbCommand cmd = GetCommand(commandText, connection);
                totalCount = int.Parse(cmd.ExecuteScalar().ToString());
                ds = ExecuteSet(GetPageSql(sql, orderby, pageSize, currentPage), cmdParms);
            }
            return ds;
        }

        #endregion

        #region 分页语句拼接
        private string GetPageSql(string sql, string orderby, int pageSize, int currentPage)
        {
            var sb = new StringBuilder();
            int startRow;
            int endRow;
            switch (MDbType)
            {
                case DbTypeEnum.Oracle:

                    #region 分页查询语句

                    startRow = pageSize * (currentPage - 1);
                    endRow = startRow + pageSize;

                    sb.Append("select * from ( select row_limit.*, rownum rownum_ from (");
                    sb.Append(sql);
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        sb.Append(" ");
                        sb.Append(orderby);
                    }
                    sb.Append(" ) row_limit where rownum <= ");
                    sb.Append(endRow);
                    sb.Append(" ) where rownum_ >");
                    sb.Append(startRow);

                    #endregion

                    break;
                case DbTypeEnum.Mssql:

                    #region 分页查询语句

                    startRow = pageSize * (currentPage - 1) + 1;
                    endRow = startRow + pageSize - 1;

                    sb.AppendFormat(@"
                            select * from
                            (select ROW_NUMBER() over({1}) as rowNumber, t.* from ({0}) t) tempTable
                            where rowNumber between {2} and {3} ", sql, orderby, startRow, endRow);

                    #endregion

                    break;
                case DbTypeEnum.Mysql:

                    #region 分页查询语句

                    startRow = pageSize * (currentPage - 1);

                    sb.Append(sql);
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        sb.Append(" ");
                        sb.Append(orderby);
                    }
                    sb.AppendFormat(" limit {0},{1}", startRow, pageSize);

                    #endregion

                    break;
                case DbTypeEnum.Sqlite:
                    #region 分页查询语句

                    startRow = pageSize * (currentPage - 1);

                    sb.Append(sql);
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        sb.Append(" ");
                        sb.Append(orderby);
                    }
                    sb.AppendFormat(" limit {0} offset {1}", pageSize, startRow);

                    #endregion
                    break;
                default:
                    #region 分页查询语句

                    startRow = pageSize * (currentPage - 1);

                    sb.Append(sql);
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        sb.Append(" ");
                        sb.Append(orderby);
                    }
                    sb.AppendFormat(" limit {0} offset {1}", pageSize, startRow);

                    #endregion
                    break;
            }
            return sb.ToString();
        }

        #endregion

        #region getReaderValue 转换数据

        /// <summary>
        /// 转换数据
        /// </summary>
        private static object GetReaderValue(object rdValue, Type ptype)
        {
            if (ptype == typeof(double))
            {
                return Convert.ToDouble(rdValue);
            }

            if (ptype == typeof(decimal))
            {
                return Convert.ToDecimal(rdValue);
            }

            if (ptype == typeof(int))
            {
                return Convert.ToInt32(rdValue);
            }

            if (ptype == typeof(long))
            {
                return Convert.ToInt64(rdValue);
            }

            if (ptype == typeof(DateTime))
            {
                return Convert.ToDateTime(rdValue);
            }

            if (ptype == typeof(double?))
            {
                return Convert.ToDouble(rdValue);
            }

            if (ptype == typeof(decimal?))
            {
                return Convert.ToDecimal(rdValue);
            }

            if (ptype == typeof(int?))
            {
                return Convert.ToInt32(rdValue);
            }

            if (ptype == typeof(long?))
            {
                return Convert.ToInt64(rdValue);
            }

            return ptype == typeof(DateTime?) ? Convert.ToDateTime(rdValue) : rdValue;
        }

        #endregion

        #region 获取实体类属性

        /// <summary>
        /// 获取实体类属性
        /// </summary>
        private static PropertyInfo[] GetEntityProperties(Type type)
        {
            var propertyInfoList = type.GetProperties();
            return propertyInfoList.Where(propertyInfo => propertyInfo.GetCustomAttributes(typeof(EdmRelationshipNavigationPropertyAttribute), false).Length == 0 && propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), false).Length == 0).ToArray();
        }

        #endregion

        #region 获取基类

        /// <summary>
        /// 获取基类
        /// </summary>
        public static Type GetBaseType(Type type)
        {
            while (type.BaseType != null && type.BaseType.Name != typeof(object).Name)
            {
                type = type.BaseType;
            }
            return type;
        }

        #endregion

        #endregion 分页

        #region 事务

        #region 开始事务

        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTransaction()
        {
            if (_sqlLog)
            {
                Info("开始事务");
            }

            var conn = GetConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            _mTran = conn.BeginTransaction();
        }

        #endregion

        #region 提交事务

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            //防止重复提交
            if (_mTran == null)
            {
                return;
            }

            if (_sqlLog)
            {
                Info("提交事务");
            }

            var conn = _mTran.Connection;
            try
            {
                _mTran.Commit();
            }
            catch (Exception ex)
            {
                _mTran.Rollback();
                Error("提交事务:" + ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                _mTran.Dispose();
                _mTran = null;
            }
        }

        #endregion

        #region 回滚事务(出错时调用该方法回滚)

        /// <summary>
        /// 回滚事务(出错时调用该方法回滚)
        /// </summary>
        public void RollbackTransaction()
        {
            //防止重复回滚
            if (_mTran == null)
            {
                return;
            }

            if (_sqlLog)
            {
                Info("回滚事务");
            }

            var conn = _mTran.Connection;
            try
            {
                _mTran.Rollback();
            }
            catch (Exception ex)
            {
                Error("回滚事务:" + ex);
            }
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        #endregion

        #endregion

        #region SQL日志

        /// <summary>
        /// 获取具体执行的SQL
        /// </summary>
        /// <returns></returns>
        private static string GetSql(string sqlString, params DbParameter[] cmdParms)
        {
            var sqlStr = sqlString;
            foreach (var p in cmdParms)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (p.DbType)
                {
                    case DbType.AnsiString:

                    case DbType.Binary:

                    case DbType.Date:

                    case DbType.DateTime:

                    case DbType.Guid:

                    case DbType.Object:

                    case DbType.String:

                    case DbType.Time:

                    case DbType.AnsiStringFixedLength:

                    case DbType.StringFixedLength:

                    case DbType.Xml:

                    case DbType.DateTime2:

                    case DbType.DateTimeOffset:
                        sqlStr = sqlStr.Replace(p.ParameterName, "'" + p.Value + "'");
                        break;
                    default:
                        sqlStr = sqlStr.Replace(p.ParameterName, p.Value.ToString());
                        break;
                }
            }
            return sqlStr;
        }

        /// <summary>
        /// SQL记录日志
        /// </summary>
        private static void LogInfo(string sqlString, params DbParameter[] cmdParms)
        {
            if (!_sqlLog)
            {
                return;
            }

            Info("原查询:" + sqlString + string.Concat(cmdParms.Select(m => " " + m.ParameterName + ":" + m.Value)));
            Info("调试查询:" + GetSql(sqlString, cmdParms));
        }

        /// <summary>
        /// SQL错误日志
        /// </summary>
        private static void LogErr(string sqlString, params DbParameter[] cmdParms)
        {
            Error("原查询:" + sqlString + string.Concat(cmdParms.Select(m => " " + m.ParameterName + ":" + m.Value)));
            Error("调试查询:" + GetSql(sqlString, cmdParms));
        }

        private static void Info(object o)
        {
            Log.Info(o, "DB_");
        }

        private static void Error(object o)
        {
            Log.Error(o, "DB_");
        }

        #endregion
    }

}
