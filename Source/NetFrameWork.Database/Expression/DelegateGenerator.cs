using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NetFrameWork.Database.Expression
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateGenerator : ExpressionVisitor
    {
        private static readonly MethodInfo SIndexerInfo = typeof(List<object>).GetMethod("get_Item");

        private int _mParameterCount;
        private ParameterExpression _mParametersExpression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Func<List<object>, object> Generate(System.Linq.Expressions.Expression exp)
        {
            _mParameterCount = 0;
            _mParametersExpression =
                System.Linq.Expressions.Expression.Parameter(typeof(List<object>), "parameters");

            var body = Visit(exp); // normalize
            if (body.Type != typeof(object))
            {
                body = System.Linq.Expressions.Expression.Convert(body, typeof(object));
            }

            var lambda = System.Linq.Expressions.Expression.Lambda<Func<List<object>, object>>(body, _mParametersExpression);
            return lambda.Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
        {
            System.Linq.Expressions.Expression exp = System.Linq.Expressions.Expression.Call(
                _mParametersExpression,
                SIndexerInfo,
                System.Linq.Expressions.Expression.Constant(_mParameterCount++));
            return c.Type == typeof(object) ? exp : System.Linq.Expressions.Expression.Convert(exp, c.Type);
        }
    }
}
