using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NetFrameWork.Database.WhereExpression
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
        private int _lastIndex;

        private int GetIndex()
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
        /// 拉姆达主体转Where
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Where ToWhere(Expression e)
        {
            switch (e)
            {
                case BinaryExpression binaryExpression:
                    return ConvertBinary(binaryExpression);
                case MethodCallExpression methodCallExpression:
                    return MethodCall(methodCallExpression);
                default:
                    throw new Exception("不支持的Where条件,Lambda表达式写法！请使用经典写法！");
            }
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

        private Where ConvertBinary(BinaryExpression be)
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
                    return ToWhere(ToWhere(be.Left), ToWhere(be.Right));
                case ExpressionType.OrElse:
                    return ToWhere(ToWhere(be.Left), ToWhere(be.Right), false);
                case ExpressionType.Add:
                    break;
                case ExpressionType.AddChecked:
                    break;
                case ExpressionType.And:
                    break;
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.Call:
                    break;
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    break;
                case ExpressionType.Convert:
                    break;
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.Divide:
                    break;
                case ExpressionType.ExclusiveOr:
                    break;
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.Lambda:
                    break;
                case ExpressionType.LeftShift:
                    break;
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.MemberAccess:
                    break;
                case ExpressionType.MemberInit:
                    break;
                case ExpressionType.Modulo:
                    break;
                case ExpressionType.Multiply:
                    break;
                case ExpressionType.MultiplyChecked:
                    break;
                case ExpressionType.Negate:
                    break;
                case ExpressionType.UnaryPlus:
                    break;
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    break;
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.Not:
                    break;
                case ExpressionType.Or:
                    break;
                case ExpressionType.Parameter:
                    break;
                case ExpressionType.Power:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    break;
                case ExpressionType.Subtract:
                    break;
                case ExpressionType.SubtractChecked:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeIs:
                    break;
                case ExpressionType.Assign:
                    break;
                case ExpressionType.Block:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Decrement:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Default:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    break;
                case ExpressionType.Increment:
                    break;
                case ExpressionType.Index:
                    break;
                case ExpressionType.Label:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Loop:
                    break;
                case ExpressionType.Switch:
                    break;
                case ExpressionType.Throw:
                    break;
                case ExpressionType.Try:
                    break;
                case ExpressionType.Unbox:
                    break;
                case ExpressionType.AddAssign:
                    break;
                case ExpressionType.AndAssign:
                    break;
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.AddAssignChecked:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.PostDecrementAssign:
                    break;
                case ExpressionType.TypeEqual:
                    break;
                case ExpressionType.OnesComplement:
                    break;
                case ExpressionType.IsTrue:
                    break;
                case ExpressionType.IsFalse:
                    break;
                default:
                    throw new Exception($"不支持的Where条件({be.NodeType}),Lambda表达式写法！请使用经典写法！");
            }
            throw new Exception($"不支持的Where条件({be.NodeType}),Lambda表达式写法！请使用经典写法！");
        }

        private Where MethodCall(MethodCallExpression mce)
        {
            if (mce.Method.Name != "StartsWith" && mce.Method.Name != "EndsWith" && mce.Method.Name != "Contains")
            {
                throw new Exception($"不支持的Where条件,方法({mce.Method.Name})Lambda表达式写法！请使用经典写法！");
            }
            if (mce.Object == null)
            {
                throw new Exception("不支持的Where条件,Lambda表达式写法！请使用经典写法！");
            }
            var objStr = mce.Object.ToString();
            var value = ((ConstantExpression)mce.Arguments[0]).Value;
            switch (mce.Method.Name)
            {
                case "StartsWith":
                    value += "%";
                    break;
                case "EndsWith":
                    value = $"%{value}";
                    break;
                case "Contains":
                    value = $"%{value}%";
                    break;
                default:
                    throw new Exception($"不支持的Where条件,方法({mce.Method.Name})Lambda表达式写法！请使用经典写法！");
            }
            var name = objStr.Substring(objStr.IndexOf('.') + 1);
            var w = new Where();
            var param = $"Where{GetIndex()}Param";
            w.SqlText.AppendFormat(" `{0}` like {1} ", name, param);
            w.ParamDict.Add(param, value);
            return w;
        }

        private Where LeftAndRight(BinaryExpression be)
        {
            var expLeft = be.Left;
            var expRight = be.Right;
            if (expLeft.NodeType != ExpressionType.MemberAccess)
                throw new Exception("不支持的Where条件(" + be.NodeType + ")Lambda表达式写法！请使用经典写法！");
            var left = (MemberExpression)expLeft;
            var w = new Where();
            var obj = FastEvaluator.Eval(expRight);
            if (obj == null)
            {
                switch (be.NodeType)
                {
                    case ExpressionType.Equal:
                        w.SqlText.AppendFormat(" `{0}` is null ", left.Member.Name);
                        break;
                    case ExpressionType.NotEqual:
                        w.SqlText.AppendFormat(" `{0}` is not null ", left.Member.Name);
                        break;
                    default:
                        throw new Exception("不支持的Where条件(" + be.NodeType + ") 比较值不能为 null");
                }
            }
            else
            {
                var value = $"WhereParam{GetIndex()}";
                w.SqlText.AppendFormat(" `{0}` {1} {2} ", left.Member.Name, GetOperator(be.NodeType), value);
                w.ParamDict.Add(value, obj);
            }
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
                case ExpressionType.Add:
                    break;
                case ExpressionType.AddChecked:
                    break;
                case ExpressionType.And:
                    break;
                case ExpressionType.AndAlso:
                    break;
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.Call:
                    break;
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    break;
                case ExpressionType.Convert:
                    break;
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.Divide:
                    break;
                case ExpressionType.ExclusiveOr:
                    break;
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.Lambda:
                    break;
                case ExpressionType.LeftShift:
                    break;
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.MemberAccess:
                    break;
                case ExpressionType.MemberInit:
                    break;
                case ExpressionType.Modulo:
                    break;
                case ExpressionType.Multiply:
                    break;
                case ExpressionType.MultiplyChecked:
                    break;
                case ExpressionType.Negate:
                    break;
                case ExpressionType.UnaryPlus:
                    break;
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    break;
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.Not:
                    break;
                case ExpressionType.Or:
                    break;
                case ExpressionType.OrElse:
                    break;
                case ExpressionType.Parameter:
                    break;
                case ExpressionType.Power:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    break;
                case ExpressionType.Subtract:
                    break;
                case ExpressionType.SubtractChecked:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeIs:
                    break;
                case ExpressionType.Assign:
                    break;
                case ExpressionType.Block:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Decrement:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Default:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    break;
                case ExpressionType.Increment:
                    break;
                case ExpressionType.Index:
                    break;
                case ExpressionType.Label:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Loop:
                    break;
                case ExpressionType.Switch:
                    break;
                case ExpressionType.Throw:
                    break;
                case ExpressionType.Try:
                    break;
                case ExpressionType.Unbox:
                    break;
                case ExpressionType.AddAssign:
                    break;
                case ExpressionType.AndAssign:
                    break;
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.AddAssignChecked:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.PostDecrementAssign:
                    break;
                case ExpressionType.TypeEqual:
                    break;
                case ExpressionType.OnesComplement:
                    break;
                case ExpressionType.IsTrue:
                    break;
                case ExpressionType.IsFalse:
                    break;
                default:
                    throw new Exception("不支持的Where条件(" + t + ")Lambda表达式写法！请使用经典写法！");
            }
            throw new Exception("不支持的Where条件(" + t + ")Lambda表达式写法！请使用经典写法！");
        }

        #endregion

    }
}
