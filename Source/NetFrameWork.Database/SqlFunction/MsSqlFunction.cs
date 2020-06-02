namespace NetFrameWork.Database.SqlFunction
{
    internal class MsSqlFunction : SqlFunBase
    {
        public override string Substr(string str, int start, int length)
        {
            return $"SUBSTRING({str},{start},{length}))";
        }

        public override string Now()
        {
            // ReSharper disable once StringLiteralTypo
            return "GETDATE()";
        }

    }
}
