using System;
using System.Web.Http;
using NetFrameWork.Database;

namespace Test.Api.Controllers
{
    public class TestController : ApiController
    {
        public string Get()
        {
            var sql = new Sql();
            sql.Select().AddBs(SqlFun.GetSqlFun(DbHelper.Db.MDbType).Now()).AddBs("DateTime");
            var ds = DbHelper.Db.ExecuteSet(sql.ToString());
            var dt = ds.Tables[0].Rows[0]["DateTime"].ToString();
            if (string.IsNullOrEmpty(dt))
            {
                throw new Exception("数据库连接失败,请检查配置.");
            }
            return $"{dt} Success";
        }
    }
}