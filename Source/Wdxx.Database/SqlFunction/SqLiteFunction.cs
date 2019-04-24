namespace Wdxx.Database.SqlFunction
{
    internal class SqLiteFunction : SqlFunBase
    {
        
        internal new string Now()
        {
            return "date('now') || ' ' || time('now', 'localtime')";
        }

    }
}