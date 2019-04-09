using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace SqliteChangePwd
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 数据库地址
        /// </summary>
        public static string DataBasePath;
        /// <summary>
        /// 数据库密码
        /// </summary>
        public static string DataBasePasssord;
        /// <summary>
        /// 数据库初始化
        /// </summary>
        /// <param name="errMessage">初始化异常提示</param>
        /// <returns></returns>
        public static bool Init(out string errMessage)
        {
            if (!File.Exists("DBComfig.txt"))
            {
                File.WriteAllText(@"DBComfig.txt", @"dataBasePath=D:\\mydb.db,dataBasePasssord=123456");
                errMessage = "数据库配置不存在,已经自动创建,请配置好数据库后重启服务器 !";
                return false;
            }
            var dbStr = File.ReadAllText("DBComfig.txt");
            var db = dbStr.Split(',');
            if (db.Length != 2)
            {
                errMessage = "数据库配置配置异常,请配置好数据库后重启服务器 !";
                return false;
            }
            var dbPath = db[0].Split('=');
            var dbPwd = db[1].Split('=');
            if (dbPath.Length != 2 || dbPwd.Length != 2)
            {
                errMessage = "数据库配置配置异常,请配置好数据库后重启服务器 !";
                return false;
            }
            DataBasePath = Path.GetFullPath(dbPath[1].Trim());
            DataBasePasssord = dbPwd[1].Trim();
            errMessage = string.Empty;
            return Test();
        }
        /// <summary>
        /// 测试数据库连接情况
        /// </summary>
        private static bool Test()
        {
            try
            {
                Query("select name from sqlite_master");
                return true;
            }
            catch (Exception ex)
            {
                YxLog.Error(ex);
                throw new Exception("连接数据库异常:" + ex.Message);
            }
        }
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet Query(string sql)
        {
            DataSet dsResult;
            using (var conn = GetSqLiteConnection())
            {
                using (var da = new SQLiteDataAdapter(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        dsResult = new DataSet();
                        da.Fill(dsResult, "ds");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("执行查询异常:" + ex.Message);
                    }
                }
            }
            return dsResult;
        }
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        private static SQLiteConnection GetSqLiteConnection()
        {
            SQLiteConnection conn;
            try
            {
                conn = new SQLiteConnection();
                var connStr = new SQLiteConnectionStringBuilder
                {
                    DataSource = DataBasePath,
                    Password = DataBasePasssord
                };
                conn.ConnectionString = connStr.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("连接数据库异常:" + ex.Message);
            }
            return conn;
        }
        /// <summary>
        /// 修改数据库密码
        /// </summary>
        /// <param name="pwd">不给此参数是删除密码</param>
        /// <returns></returns>
        public static bool ChangePwd(string pwd = "")
        {
            using (var conn = GetSqLiteConnection())
            {
                try
                {
                    conn.Open();
                    conn.ChangePassword(pwd);
                }
                catch (Exception ex)
                {
                    YxLog.Error(ex);
                    throw new Exception("连接数据库异常:" + ex.Message);
                }
            }
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Init(out var err); if (err != string.Empty)
                {
                    MessageBox.Show(err);
                }
                else
                {
                    if (!ChangePwd()) return;
                    MessageBox.Show(@"密码删除成功!");
                    YxLog.Info(@"数据库密码删除成功");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                YxLog.Error(exception);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show(@"新密码不能为空!");
                return;
            }
            try
            {
                Init(out var err);
                if (err != string.Empty)
                {
                    MessageBox.Show(err);
                }
                else
                {
                    if (ChangePwd(textBox1.Text))
                    {
                        MessageBox.Show(@"密码修改成功!");
                    }
                    YxLog.Info($@"修改数据库密码为:{textBox1.Text}");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                YxLog.Error(exception);
            }
        }
    }
}
