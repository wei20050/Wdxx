namespace NetFrameWork.Database.SqlFunction
{
    internal class OracleFunction : SqlFunBase
    {

        internal new string Now()
        {
            return "SYSDATE()";
        }
        
    }
}