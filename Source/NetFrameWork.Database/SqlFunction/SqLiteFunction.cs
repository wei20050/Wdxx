namespace NetFrameWork.Database.SqlFunction
{
    internal class SqLiteFunction : SqlFunBase
    {
        public override string Now()
        {
            return "DATE('NOW') || ' ' || TIME('NOW', 'LOCALTIME')";
        }

    }
}