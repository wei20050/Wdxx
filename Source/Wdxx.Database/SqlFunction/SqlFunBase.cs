namespace Wdxx.Database.SqlFunction
{
    /// <summary>
    /// 基于Mysql的sql函数基类
    /// </summary>
    internal class SqlFunBase
    {
        
        internal string Substr(string str, int start, int length)
        {
            return $"SUBSTR({str},{start},{length}))";
        }
        
        internal string Now()
        {
            return "NOW()";
        }

    }
}
