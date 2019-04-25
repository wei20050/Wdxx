namespace Wdxx.Database.SqlFunction
{
    internal class SqLiteFunction : SqlFunBase
    {
        
        internal new string Now()
        {
            return "DATE('NOW') || ' ' || TIME('NOW', 'LOCALTIME')";
        }

    }
}