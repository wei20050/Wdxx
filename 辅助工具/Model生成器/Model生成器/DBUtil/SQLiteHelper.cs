using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace Model生成器.DBUtil
{
    /// <summary>
    /// SQLite操作类
    /// </summary>
    public class SqLiteHelper
    {
        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbContext"].ToString();
        #endregion

        #region Exists
        public bool Exists(string sqlString)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        var obj = cmd.ExecuteScalar();
                        return !Equals(obj, null) && !Equals(obj, DBNull.Value);
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
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
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sqlString)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        var rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region 执行查询语句，返回DataTable
        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="sqlString">查询语句</param>
        /// <returns>DataTable</returns>
        public DataTable Query(string sqlString)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    var command = new SQLiteDataAdapter(sqlString, connection);
                    var ds = new DataSet();
                    command.Fill(ds, "ds");
                    return ds.Tables[0];
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        #endregion

    }
}
