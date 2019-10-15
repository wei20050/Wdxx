using System.Collections.Generic;
using System.Linq.Expressions;

namespace NetFrameWork.Database.Expression
{
    /// <summary>
    /// 
    /// </summary>
    public class ConstantExtractor : ExpressionVisitor
    {
        private List<object> _mConstants;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public List<object> Extract(System.Linq.Expressions.Expression exp)
        {
            _mConstants = new List<object>();
            Visit(exp);
            return _mConstants;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
        {
            _mConstants.Add(c.Value);
            return c;
        }
    }        
}
