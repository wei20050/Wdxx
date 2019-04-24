namespace Wdxx.Database.SqlFunction
{
    internal class MsSqlFunction : SqlFunBase
    {
        
        internal new string Substr(string str, int start, int length)
        {
            return string.Format("SUBSTRING({0},{1},{2}))", str, start, length);
        }
        
        internal new string Now()
        {
            return "GETDATE()";
        }

    }
}
