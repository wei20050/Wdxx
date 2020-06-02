// ReSharper disable UnusedMember.Global
namespace NetFrameWork.Database.SqlFunction
{
    /// <summary>
    /// 基于Mysql的sql函数基类
    /// </summary>
    public class SqlFunBase
    {

        /// <summary>
        /// 返回字符串截取函数
        /// </summary>
        /// <returns></returns>
        public virtual string Substr(string str, int start, int length)
        {
            return $"SUBSTR({str},{start},{length}))";
        }
        
        /// <summary>
        /// 返回当前时间函数
        /// </summary>
        /// <returns></returns>
        public virtual string Now()
        {
            return "NOW()";
        }

    }
}
