using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NetFrameWork.Database.Expression
{
    /// <summary>
    /// Where类 用于组装参数化条件语句
    /// </summary>
    public class Where
    {

        private static readonly FastEvaluator FastEvaluator = new FastEvaluator();

        #region 构造函数

        /// <summary>
        /// 构造函数 初始化sql语句与参数
        /// </summary>
        public Where()
        {
            SqlText = new StringBuilder();
            ParamDict = new Dictionary<string, object>();
        }

        #endregion

        #region 重写ToString

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>当前拼接好的sql</returns>
        public override string ToString()
        {
            return SqlText.ToString();
        }

        #endregion

        #region 字段

        /// <summary>
        /// 上个参数的位置(默认0)
        /// </summary>
        private static int _lastIndex;

        private static int GetIndex()
        {
            return _lastIndex++;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 返回当前对象所有产品参数,key:参数名,value:参数值
        /// </summary>
        public Dictionary<string, object> ParamDict { get; }

        /// <summary>
        /// 返回当前生成Sql文本
        /// </summary>
        private StringBuilder SqlText { get; }

        #endregion

        #region 公共方法

        /// <summary>
        /// 拉姆达转换Sql
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Sql ToSql<T>(Expression<Func<T, bool>> e)
        {
            var w = ToWhereChild(e.Body);
            var s = new Sql();
            s.Add(w.ToString());
            foreach (var p in w.ParamDict)
            {
                s.ParamDict.Add(p.Key, p.Value);
            }
            return s;
        }

        private static Where ToWhere(Where wLeft, Where wRight, bool isAnd = true)
        {
            var andOr = isAnd ? " AND " : " OR ";
            var w = new Where();
            w.SqlText.AppendFormat("({0} {1} {2})", wLeft, andOr, wRight);
            foreach (var l in wLeft.ParamDict)
            {
                w.ParamDict.Add(l.Key, l.Value);
            }
            foreach (var r in wRight.ParamDict)
            {
                w.ParamDict.Add(r.Key, r.Value);
            }
            return w;
        }

        private static Where ToWhereChild(System.Linq.Expressions.Expression e)
        {
            if (e is BinaryExpression expression)
            {
                return ConvertBinary(expression);
            }
            throw new Exception("暂时不支持的Where条件Lambda表达式写法！请使用经典写法！");
        }

        private static Where ConvertBinary(BinaryExpression be)
        {
            switch (be.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    return LeftAndRight(be);
                case ExpressionType.AndAlso:
                    return ToWhere(ToWhereChild(be.Left), ToWhereChild(be.Right));
                case ExpressionType.OrElse:
                    return ToWhere(ToWhereChild(be.Left), ToWhereChild(be.Right), false);
                default:
                    throw new Exception("暂时不支持的Where条件(" + be.NodeType + ")Lambda表达式写法！请使用经典写法！");
            }
        }

        private static Where LeftAndRight(BinaryExpression be)
        {
            var expLeft = be.Left;
            var expRight = be.Right;
            if (expLeft.NodeType != ExpressionType.MemberAccess)
                throw new Exception("暂时不支持的Where条件(" + be.NodeType + ")Lambda表达式写法！请使用经典写法！");
            var left = (MemberExpression)expLeft;
            var w = new Where();
            var value = $"WhereParam{GetIndex()}";
            w.SqlText.AppendFormat(" `{0}` {1} {2} ", left.Member.Name, GetOperator(be.NodeType), value);
            var obj = FastEvaluator.Eval(expRight);
            w.ParamDict.Add(value, obj);
            return w;
        }

        private static string GetOperator(ExpressionType t)
        {
            switch (t)
            {
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.NotEqual:
                    return " <> ";
                default:
                    return string.Empty;
            }

        }

        #endregion

    }
}
