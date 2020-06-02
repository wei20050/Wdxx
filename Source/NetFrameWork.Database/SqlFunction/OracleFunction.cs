namespace NetFrameWork.Database.SqlFunction
{
    internal class OracleFunction : SqlFunBase
    {
        public override string Now()
        {
            return "SYSDATE()";
        }
        
    }
}