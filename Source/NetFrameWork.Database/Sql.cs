using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using NetFrameWork.Database.WhereExpression;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Database
{
    /// <summary>
    /// SqlTextHelper类 用于组装参数化查询语句
    /// </summary>
    public class Sql
    {
        #region 构造函数

        /// <summary>
        /// 构造函数 初始化sql语句与参数
        /// </summary>
        public Sql()
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
        private int _lastParamIndex;

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
        /// 增加查询参数,并输出参数名,参数名会根据LastParamIndex递增
        /// </summary>
        /// <param name="v">参数值</param>
        /// <returns>返回参数名</returns>
        private string AddParam(object v)
        {
            //这里用完 _lastParamIndex 立即加一 防止下个参数错误
            var paramName = $"Sql{_lastParamIndex++}Param";
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

        /// <summary>
        /// 拉姆达转换Sql
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Sql ToSql<T>(Expression<Func<T, bool>> e)
        {
            var w = new Where().ToWhere(e.Body);
            var s = new Sql();
            s.Add(w.ToString());
            foreach (var p in w.ParamDict)
            {
                s.ParamDict.Add(p.Key, p.Value);
            }

            return s;
        }

        #endregion

        #region 查询组装

        /// <summary>
        /// 当前Sql文本增加字符前后不带空格, {0}
        /// </summary>
        public virtual Sql Add(string str)
        {
            SqlText.Append(str);
            return this;
        }

        /// <summary>
        /// 当前Sql文本增加字符前后带空格, {0}
        /// </summary>
        public virtual Sql AddBs(string str)
        {
            SqlText.AppendFormat(" {0} ", str);
            return this;
        }

        /// <summary>
        /// 当前Sql文本增加表名与*, table.*
        /// </summary>
        public virtual Sql AddTf(string table)
        {
            SqlText.AppendFormat(" {0}.* ", table);
            return this;
        }

        /// <summary>
        /// 当前Sql文本增加表名与字段, table.field
        /// </summary>
        public virtual Sql AddTf(string table, string field)
        {
            SqlText.AppendFormat(" {0}.{1} ", table, field);
            return this;
        }

        /// <summary>
        /// 当前Sql文本增加字段, `{0}`
        /// </summary>
        public virtual Sql AddField(string str)
        {
            SqlText.AppendFormat(" `{0}` ", str);
            return this;
        }

        /// <summary>
        /// 增加一个select,等于 "select"
        /// </summary>
        public virtual Sql Select()
        {
            SqlText.AppendFormat(" SELECT ");
            return this;
        }

        /// <summary>
        /// 增加一个*带前后空格,等于 " * "
        /// </summary>
        public virtual Sql All()
        {
            SqlText.AppendFormat(" * ");
            return this;
        }

        /// <summary>
        /// 增加一个from,等于 "from"
        /// </summary>
        public virtual Sql From()
        {
            SqlText.AppendFormat(" FROM ");
            return this;
        }

        /// <summary>
        /// 增加一个join,等于 "join"
        /// </summary>
        public virtual Sql Join()
        {
            SqlText.AppendFormat(" JOIN ");
            return this;
        }

        /// <summary>
        /// 增加一个inner join,等于 "inner join"
        /// </summary>
        public virtual Sql InnerJoin()
        {
            SqlText.AppendFormat(" INNER JOIN ");
            return this;
        }

        /// <summary>
        /// 增加一个left join,等于 "left join"
        /// </summary>
        public virtual Sql LeftJoin()
        {
            SqlText.AppendFormat(" LEFT JOIN ");
            return this;
        }

        /// <summary>
        /// 增加一个right join,等于 "right join"
        /// </summary>
        public virtual Sql RightJoin()
        {
            SqlText.AppendFormat(" RIGHT JOIN ");
            return this;
        }

        /// <summary>
        /// 增加一个on,等于 "on"
        /// </summary>
        public virtual Sql On()
        {
            SqlText.AppendFormat(" ON ");
            return this;
        }

        /// <summary>
        /// 增加一个where,等于 "where"
        /// </summary>
        public virtual Sql Where()
        {
            SqlText.AppendFormat(" WHERE ");
            return this;
        }

        /// <summary>
        /// 比较符,等于
        /// </summary>
        public virtual Sql Equal()
        {
            SqlText.AppendFormat(" = ");
            return this;
        }

        /// <summary>
        /// 比较符,等于 链接一个字段
        /// </summary>
        public virtual Sql Equal(object v)
        {
            SqlText.AppendFormat(" = {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,不等于
        /// </summary>
        public virtual Sql EqualNot()
        {
            SqlText.AppendFormat(" <> ");
            return this;
        }

        /// <summary>
        /// 比较符,不等于 链接一个字段
        /// </summary>
        public virtual Sql EqualNot(object v)
        {
            SqlText.AppendFormat(" <> {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,大于
        /// </summary>
        public virtual Sql GreaterThan()
        {
            SqlText.AppendFormat(" > ");
            return this;
        }

        /// <summary>
        /// 比较符,大于 链接一个字段
        /// </summary>
        public virtual Sql GreaterThan(object v)
        {
            SqlText.AppendFormat(" > {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,小于
        /// </summary>
        public virtual Sql LessThan()
        {
            SqlText.AppendFormat(" < ");
            return this;
        }

        /// <summary>
        /// 比较符,小于 链接一个字段
        /// </summary>
        public virtual Sql LessThan(object v)
        {
            SqlText.AppendFormat(" < {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,大于等于
        /// </summary>
        public virtual Sql GreaterThenEqual()
        {
            SqlText.AppendFormat(" >= ");
            return this;
        }

        /// <summary>
        /// 比较符,大于等于 链接一个字段
        /// </summary>
        public virtual Sql GreaterThenEqual(object v)
        {
            SqlText.AppendFormat(" >= {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,小于等于
        /// </summary>
        public virtual Sql LessThanEqual()
        {
            SqlText.AppendFormat(" <= ");
            return this;
        }

        /// <summary>
        /// 比较符,小于等于 链接一个字段
        /// </summary>
        public virtual Sql LessThanEqual(object v)
        {
            SqlText.AppendFormat(" <= {0}", AddParam(v));
            return this;
        }

        /// <summary>
        /// 比较符,In,参数为某范围内的比较值
        /// </summary>
        public virtual Sql In(params object[] inArgs)
        {
            SqlText.Append(" IN (");
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
        public virtual Sql NotIn(params object[] inArgs)
        {
            SqlText.Append(" NOT IN (");
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
        public virtual Sql LikeLeft(string v)
        {
            var addParam = AddParam("%" + v);
            SqlText.AppendFormat(" LIKE {0}", addParam);
            return this;
        }

        /// <summary>
        /// 比较符 right like {0}%
        /// </summary>
        public virtual Sql LikeRight(string v)
        {
            var addParam = AddParam(v + "%");
            SqlText.AppendFormat(" LIKE {0}", addParam);
            return this;
        }

        /// <summary>
        /// 比较符 full like %{0}%
        /// </summary>
        public virtual Sql Like(string v)
        {
            var addParam = AddParam("%" + v + "%");
            SqlText.AppendFormat(" LIKE {0}", addParam);
            return this;
        }

        /// <summary>
        /// 增加一个左括号 "("
        /// </summary>
        public virtual Sql BracketLeft()
        {
            SqlText.AppendFormat("(");
            return this;
        }

        /// <summary>
        /// 增加一个左括号"(",再加一个字段)
        /// </summary>
        public virtual Sql BracketLeft(string str)
        {
            SqlText.AppendFormat("(`{0}`", str);
            return this;
        }

        /// <summary>
        /// 增加一个右括号 ")"
        /// </summary>
        public virtual Sql BracketRight()
        {
            SqlText.Append(")");
            return this;
        }

        /// <summary>
        /// 增加连接符 "and"
        /// </summary>
        public virtual Sql And()
        {
            And(string.Empty);
            return this;
        }

        /// <summary>
        /// 增加连接符 "and",再加一个字段
        /// </summary>
        public virtual Sql And(string str)
        {
            if (SqlText.Length > 0)
            {
                SqlText.Append(" AND ");
            }

            if (!string.IsNullOrEmpty(str))
            {
                SqlText.AppendFormat("`{0}`", str);
            }

            return this;
        }

        /// <summary>
        /// 增加连接符 "or"
        /// </summary>
        public virtual Sql Or()
        {
            Or(string.Empty);
            return this;
        }

        /// <summary>
        /// 增加连接符"or",再加一个字段
        /// </summary>
        public virtual Sql Or(string str)
        {
            if (SqlText.Length > 0)
            {
                SqlText.Append(" OR ");
            }

            if (!string.IsNullOrEmpty(str))
            {
                SqlText.AppendFormat("`{0}`", str);
            }

            return this;
        }

        /// <summary>
        /// 增加连接符","
        /// </summary>
        public virtual Sql Comma()
        {
            SqlText.Append(",");
            return this;
        }

        /// <summary>
        /// 多表联合查询表暂存
        /// </summary>
        private readonly List<JoinTable> _multiTables = new List<JoinTable>();


        /// <summary>
        /// 多表联合查询
        /// </summary>
        /// <param name="mainType"></param>
        /// <param name="fromType"></param>
        /// <param name="mainField"></param>
        /// <param name="fromField"></param>
        /// <param name="joInType"></param>
        /// <param name="comparisonOperator"></param>
        /// <param name="joinFields"></param>
        /// <returns></returns>
        public virtual Sql MultiTableQuery(Type mainType,Type fromType ,string mainField , string fromField, JoinTypeEnum joInType = JoinTypeEnum.Join, ComparisonOperatorEnum comparisonOperator = ComparisonOperatorEnum.Equal , List<JoinField> joinFields = null)
        {
            _multiTables.Add(new JoinTable
            {
                MainType = mainType,
                FromType= fromType,
                MainField = mainField,
                FromField = fromField,
                JoInType = joInType,
                ComparisonOperator = comparisonOperator,
                JoinFields = joinFields
            });
            if (_multiTables.Count <= 0) return this;
            Clear();
            Select();
            var startMainType = _multiTables[0].MainType;
            var startMainTableName = startMainType.Name;
            var startMainProperties = startMainType.GetProperties();
            foreach (var p in startMainProperties)
            {
                var asName = $"{startMainTableName}_{p.Name}";
                AddTf(startMainTableName, p.Name).AddBs("AS").AddBs(asName).Comma();
            }
            foreach (var multiTable in _multiTables)
            {
                var fromTableName = multiTable.FromType.Name;
                var fromProperties = multiTable.FromType.GetProperties();
                foreach (var p in fromProperties)
                {
                    var asName = $"{fromTableName}_{p.Name}";
                    AddTf(fromTableName, p.Name).AddBs("AS").AddBs(asName).Comma();
                }
            }
            SqlText.Remove(SqlText.Length - 1, 1);
            From().Add(startMainTableName);
            foreach (var multiTable in _multiTables)
            {
                switch (multiTable.JoInType)
                {
                    case JoinTypeEnum.Join:
                        Join();
                        break;
                    case JoinTypeEnum.InnerJoin:
                        InnerJoin();
                        break;
                    case JoinTypeEnum.LeftJoin:
                        LeftJoin();
                        break;
                    case JoinTypeEnum.RightJoin:
                        RightJoin();
                        break;
                    default:
                        Join();
                        break;
                }
                var fromTableName = multiTable.FromType.Name;
                Add(fromTableName).On();
                if (multiTable.JoinFields == null || multiTable.JoinFields.Count == 0)
                {
                    AddTf(multiTable.MainType.Name, multiTable.MainField);
                    switch (multiTable.ComparisonOperator)
                    {
                        case ComparisonOperatorEnum.Equal:
                            Equal();
                            break;
                        case ComparisonOperatorEnum.EqualNot:
                            EqualNot();
                            break;
                        default:
                            Equal();
                            break;
                    }
                    AddTf(fromTableName, multiTable.FromField);
                }
                else
                {
                    foreach (var f in multiTable.JoinFields)
                    {

                        switch (f.LogicalOperator)
                        {
                            case LogicalOperatorEnum.And:
                                And();
                                break;
                            case LogicalOperatorEnum.Or:
                                Or();
                                break;
                        }
                    }
                }
            }
            return this;
        }


        #endregion

        #region 排序

        /// <summary>
        /// 单个字段排序
        /// </summary>
        /// <param name="lsPram">排序参数</param>
        /// <returns></returns>
        public virtual Sql Sort(SortPram lsPram)
        {
            return Sort(new List<SortPram> { lsPram });
        }

        /// <summary>
        /// 多个字段排序
        /// </summary>
        /// <param name="lsPrams">排序参数</param>
        /// <returns></returns>
        public virtual Sql Sort(List<SortPram> lsPrams)
        {
            var str = new StringBuilder();
            foreach (var p in lsPrams)
            {
                var sort = p.SortType.ToString().ToUpper();
                var t = string.IsNullOrEmpty(p.TableName) ? string.Empty : $"{p.TableName}.";
                str.AppendFormat("{0}`{1}` {2},", t, p.ColumnName, sort);
            }

            str.Remove(str.Length - 1, 1);
            SqlText.AppendFormat(" ORDER BY {0}", str);
            return this;
        }

        #endregion

        /// <summary>
        /// 排序参数
        /// </summary>
        public struct SortPram
        {

            /// <summary>
            /// 表名
            /// </summary>
            public string TableName { get; set; }

            /// <summary>
            /// 列名
            /// </summary>
            public string ColumnName { get; set; }

            /// <summary>
            /// 排序类型
            /// </summary>
            public SortTypeEnum SortType { get; set; }

        }

        /// <summary>
        /// 排序枚举
        /// </summary>
        public enum SortTypeEnum
        {
            /// <summary>
            /// 升序
            /// </summary>
            Asc = 0,

            /// <summary>
            /// 降序
            /// </summary>
            Desc = 1
        }

    }

    /// <summary>
    /// 关联表类
    /// </summary>
    public class JoinTable
    {
        /// <summary>
        /// 关联主表类型
        /// </summary>
        public Type MainType { get; set; }

        /// <summary>
        /// 关联从表类型
        /// </summary>
        public Type FromType { get; set; }

        /// <summary>
        /// 主表关联字段
        /// </summary>
        public string MainField { get; set; }

        /// <summary>
        /// 从表关联字段
        /// </summary>
        public string FromField { get; set; }

        /// <summary>
        /// 比较运算符
        /// </summary>
        public ComparisonOperatorEnum ComparisonOperator { get; set; }

        /// <summary>
        /// 关联字段组(on 多个条件用 优先使用)
        /// </summary>
        public List<JoinField> JoinFields { get; set; }

        /// <summary>
        /// 关联类型
        /// </summary>
        public JoinTypeEnum JoInType { get; set; }

    }

    /// <summary>
    /// 关联类型枚举
    /// </summary>
    public enum JoinTypeEnum
    {
        /// <summary>
        /// Join
        /// </summary>
        Join = 0,
        /// <summary>
        /// InnerJoin
        /// </summary>
        InnerJoin = 1,
        /// <summary>
        /// LeftJoin
        /// </summary>
        LeftJoin = 2,
        /// <summary>
        /// RightJoin
        /// </summary>
        RightJoin = 3
    }

    /// <summary>
    /// 逻辑运算符枚举
    /// </summary>
    public enum LogicalOperatorEnum
    {
        /// <summary>
        /// And
        /// </summary>
        And = 0,
        /// <summary>
        /// Or
        /// </summary>
        Or = 1
    }

    /// <summary>
    /// 运算符枚举
    /// </summary>
    public enum ComparisonOperatorEnum
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal = 0,
        /// <summary>
        /// 不等于
        /// </summary>
        EqualNot = 1
    }

    /// <summary>
    /// 关联字段类
    /// </summary>
    public class JoinField
    {
        /// <summary>
        /// 主表关联字段
        /// </summary>
        public string MainField { get; set; }

        /// <summary>
        /// 从表关联字段
        /// </summary>
        public string FromField { get; set; }

        /// <summary>
        /// 比较运算符
        /// </summary>
        public ComparisonOperatorEnum ComparisonOperator { get; set; }

        /// <summary>
        /// 逻辑运算符(on 多个条件用 优先使用)
        /// </summary>
        public LogicalOperatorEnum LogicalOperator { get; set; }

    }

}