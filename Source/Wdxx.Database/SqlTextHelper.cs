using System.Collections.Generic;
using System.Text;

namespace Wdxx.Database
{
    
    /// <summary>
    /// SqlTextHelper类 用于组装参数化查询语句
    /// </summary>
    public class SqlTextHelper
    {
        #region 构造函数

        /// <summary>
        /// 构造函数 初始化sql语句与参数
        /// </summary>
        public SqlTextHelper()
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
        /// 上个参数的位置
        /// </summary>
        private int _lastParamIndex = -1;

        #endregion

        #region 属性

        /// <summary>
        /// 返回当前对象所有产品参数,key:参数数名(包含@),value:参数值
        /// </summary>
        public Dictionary<string, object> ParamDict { get; }

        /// <summary>
        /// 返回当前生成Sql文本
        /// </summary>
        private StringBuilder SqlText { get; }

        #endregion

        #region 公共方法

        /// <summary>
        /// 增加查询参数,并输出参数名,参数名会根据LastParamIndex递增
        /// </summary>
        /// <param name="v">参数值</param>
        /// <returns>返回参数名</returns>
        private string AddParam(object v)
        {
            _lastParamIndex++;
            var paramName = $"SqlTextP_{_lastParamIndex}";
            ParamDict.Add(paramName, v);
            return paramName;
        }

        /// <summary>
        /// 清除文本内容和参数内容
        /// </summary>
        public void Clear()
        {
            ParamDict.Clear();
            SqlText.Remove(0, SqlText.Length);
        }

        #endregion

        #region 查询组装

        /// <summary>
        /// 当前Sql文本增加字符, {0}
        /// </summary>
        public virtual SqlTextHelper Add(string str)
        {
            SqlText.AppendFormat(" {0} ", str);
            return this;
        }

        /// <summary>
        /// 增加一个select,等于 "select"
        /// </summary>
        public virtual SqlTextHelper Select()
        {
            SqlText.AppendFormat(" select ");
            return this;
        }

        /// <summary>
        /// 增加一个*,等于 "*"
        /// </summary>
        public virtual SqlTextHelper All()
        {
            SqlText.AppendFormat(" * ");
            return this;
        }

        /// <summary>
        /// 增加一个from,等于 "from"
        /// </summary>
        public virtual SqlTextHelper From()
        {
            SqlText.AppendFormat(" from ");
            return this;
        }

        /// <summary>
        /// 增加一个inner join,等于 "inner join"
        /// </summary>
        public virtual SqlTextHelper InnerJoin()
        {
            SqlText.AppendFormat(" inner join ");
            return this;
        }

        /// <summary>
        /// 增加一个left join,等于 "left join"
        /// </summary>
        public virtual SqlTextHelper LeftJoin()
        {
            SqlText.AppendFormat(" left join ");
            return this;
        }

        /// <summary>
        /// 增加一个right join,等于 "right join"
        /// </summary>
        public virtual SqlTextHelper RightJoin()
        {
            SqlText.AppendFormat(" right join ");
            return this;
        }

        /// <summary>
        /// 增加一个on,等于 "on"
        /// </summary>
        public virtual SqlTextHelper On()
        {
            SqlText.AppendFormat(" on ");
            return this;
        }

        /// <summary>
        /// 增加一个where,等于 "where"
        /// </summary>
        public virtual SqlTextHelper Where()
        {
            SqlText.AppendFormat(" where ");
            return this;
        }

        /// <summary>
        /// 比较符,等于 "="
        /// </summary>
        public virtual SqlTextHelper Equal(object v)
        {
            SqlText.AppendFormat(" = {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,不等于 "!="
        /// </summary>
        public virtual SqlTextHelper EqualNot(object v)
        {
            SqlText.AppendFormat(" <> {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,大于 ">"
        /// </summary>
        public virtual SqlTextHelper GreaterThan(object v)
        {
            SqlText.AppendFormat(" > {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,小于
        /// </summary>
        public virtual SqlTextHelper LessThan(object v)
        {
            SqlText.AppendFormat(" < {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,大于等于
        /// </summary>
        public virtual SqlTextHelper GreaterThenEqual(object v)
        {
            SqlText.AppendFormat(" >= {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,小于等于
        /// </summary>
        public virtual SqlTextHelper LessThanEqual(object v)
        {
            SqlText.AppendFormat(" <= {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,In,参数为某范围内的比较值
        /// </summary>
        public virtual SqlTextHelper In(params object[] inArgs)
        {
            SqlText.Append(" in (");
            for (var i = 0; i < inArgs.Length; i++)
            {
                SqlText.Append(AddParam(inArgs[i]));
                if (i < inArgs.Length - 1)
                {
                    SqlText.Append(",");
                }
            }
            SqlText.Append(" )");
            return this;
        }

        /// <summary>
        /// 比较符,NotIn,参数为某范围内的比较值
        /// </summary>
        public virtual SqlTextHelper NotIn(params object[] inArgs)
        {
            SqlText.Append(" not in (");
            for (var i = 0; i < inArgs.Length; i++)
            {
                SqlText.Append(AddParam(inArgs[i]));
                if (i < inArgs.Length - 1)
                {
                    SqlText.Append(",");
                }
            }
            SqlText.Append(" )");
            return this;
        }

        /// <summary>
        /// 比较符 left Like %{0}
        /// </summary>
        public virtual SqlTextHelper LikeLeft(string v)
        {
            var addParam = AddParam($"%{v}");
            SqlText.AppendFormat(" like {0}", addParam);
            return this;
        }

        /// <summary>
        /// 比较符 right like {0}%
        /// </summary>
        public virtual SqlTextHelper LikeRight(string v)
        {
            var addParam = AddParam($"{v}%");
            SqlText.AppendFormat(" like {0}", addParam);
            return this;
        }

        /// <summary>
        /// 比较符 full like %{0}%
        /// </summary>
        public virtual SqlTextHelper Like(string v)
        {
            var addParam = AddParam($"%{v}%");
            SqlText.AppendFormat(" like {0}", addParam);
            return this;
        }

        /// <summary>
        /// 增加一个左括号 "("
        /// </summary>
        public virtual SqlTextHelper BracketLeft()
        {
            SqlText.AppendFormat("(");
            return this;
        }

        /// <summary>
        /// 增加一个左括号"(",再加一个字符串(通常是一个字段)
        /// </summary>
        public virtual SqlTextHelper BracketLeft(string str)
        {
            SqlText.AppendFormat("(");
            SqlText.AppendFormat(str);
            return this;
        }

        /// <summary>
        /// 增加一个右括号 ")"
        /// </summary>
        public virtual SqlTextHelper BracketRight()
        {
            SqlText.Append(")");
            return this;
        }

        /// <summary>
        /// 增加连接符 "and"
        /// </summary>
        public virtual SqlTextHelper And()
        {
            And(string.Empty);
            return this;
        }

        /// <summary>
        /// 增加连接符 "and",再加一个字符串(通常是一个字段)
        /// </summary>
        public virtual SqlTextHelper And(string str)
        {
            if (SqlText.Length > 0)
            {
                SqlText.Append(" and ");
            }
            SqlText.Append(str);
            return this;
        }

        /// <summary>
        /// 增加连接符 "or"
        /// </summary>
        public virtual SqlTextHelper Or()
        {
            Or(string.Empty);
            return this;
        }

        /// <summary>
        /// 增加连接符"or",再加一个字符串(通常是一个字段)
        /// </summary>
        public virtual SqlTextHelper Or(string str)
        {
            if (SqlText.Length > 0)
            {
                SqlText.Append(" or ");
            }
            SqlText.Append(str);
            return this;
        }

        #endregion

        #region 排序

        /// <summary>
        /// 根据单个字段升序排序
        /// </summary>
        public virtual SqlTextHelper Asc(string str)
        {
            SqlText.AppendFormat(" order by {0} asc ", str);
            return this;
        }

        /// <summary>
        /// 根据单个字段降序排序
        /// </summary>
        public virtual SqlTextHelper Desc(string str)
        {
            SqlText.AppendFormat(" order by {0} desc ", str);
            return this;
        }

        /// <summary>
        /// 多个字段排序 Dictionary 排序字段,是否升序
        /// </summary>
        public virtual SqlTextHelper Sort(Dictionary<string,bool> strDic)
        {
            var str = new StringBuilder();
            foreach (var v in strDic)
            {
                var sort= v.Value ? " asc" :" desc";
                str.Append($"{v.Key}{sort},");
            }
            str.Remove(str.Length - 1, 1);
            SqlText.AppendFormat(" order by {0}", str);
            return this;
        }

        #endregion

    }
}