using Wdxx.Database;

namespace HttpService
{
    public partial class Service : IService
    {

        /// <summary>
        /// 数据库操作对象
        /// </summary>
        private readonly DbHelper _db = new DbHelper();

        /// <summary>
        /// 构造
        /// </summary>
        public Service()
        {
            _db.SqlLog = true;
        }

        /// <summary>
        /// 测试服务是否正常
        /// </summary>
        public void Test()
        {

        }

    }
}
