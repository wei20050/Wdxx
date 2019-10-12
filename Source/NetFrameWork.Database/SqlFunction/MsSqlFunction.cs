namespace NetFrameWork.Database.SqlFunction
{
    internal class MsSqlFunction : SqlFunBase
    {
        
        internal new string Substr(string str, int start, int length)
        {
            return $"SUBSTRING({str},{start},{length}))";
        }
        
        internal new string Now()
        {
            return "GETDATE()";
        }

    }
}
