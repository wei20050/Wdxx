﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace NetFrameWork.Database.Expression
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpressionComparer : IComparer<System.Linq.Expressions.Expression>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="result"></param>
        /// <returns>can stop comparing or not</returns>
        protected bool CompareNull<T>(T x, T y, out int result) where T : class
        {
            if (x == null && y == null)
            {
                result = 0;
                return true;
            }

            if (x == null || y == null)
            {
                result = x == null ? -1 : 1;
                return true;
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareType(Type x, Type y)
        {
            if (x == y) return 0;

            if (CompareNull(x, y, out var result)) return result;

            result = x.GetHashCode() - y.GetHashCode();
            if (result != 0) return result;

            result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (result != 0) return result;
            if (x.AssemblyQualifiedName == null || y.AssemblyQualifiedName == null)
            {
                return 0;
            }
            return string.Compare(x.AssemblyQualifiedName, y.AssemblyQualifiedName, StringComparison.Ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareMemberInfo(MemberInfo x, MemberInfo y)
        {
            if (x == y) return 0;

            if (CompareNull(x, y, out var result)) return result;

            result = x.GetHashCode() - y.GetHashCode();
            if (result != 0) return result;

            result = x.MemberType - y.MemberType;
            if (result != 0) return result;

            result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            result = CompareType(x.DeclaringType, y.DeclaringType);
            if (result != 0) return result;

            return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual int Compare(System.Linq.Expressions.Expression x, System.Linq.Expressions.Expression y)
        {
            if (CompareNull(x, y, out var result)) return result;
            if (x==null || y==null)
            {
                return 0;
            }
            result = CompareType(x.GetType(), y.GetType());
            if (result != 0) return result;
                
            result = x.NodeType - y.NodeType;
            if (result != 0) return result;

            result = CompareType(x.Type, y.Type);
            if (result != 0) return result;

            switch (x.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return CompareUnary((UnaryExpression)x, (UnaryExpression)y);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return CompareBinary((BinaryExpression)x, (BinaryExpression)y);
                case ExpressionType.TypeIs:
                    return CompareTypeIs((TypeBinaryExpression)x, (TypeBinaryExpression)y);
                case ExpressionType.Conditional:
                    return CompareConditional((ConditionalExpression)x, (ConditionalExpression)y);
                case ExpressionType.Constant:
                    return CompareConstant((ConstantExpression)x, (ConstantExpression)y);
                case ExpressionType.Parameter:
                    return CompareParameter((ParameterExpression)x, (ParameterExpression)y);
                case ExpressionType.MemberAccess:
                    return CompareMemberAccess((MemberExpression)x, (MemberExpression)y);
                case ExpressionType.Call:
                    return CompareMethodCall((MethodCallExpression)x, (MethodCallExpression)y);
                case ExpressionType.Lambda:
                    return CompareLambda((LambdaExpression)x, (LambdaExpression)y);
                case ExpressionType.New:
                    return CompareNew((NewExpression)x, (NewExpression)y);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return CompareNewArray((NewArrayExpression)x, (NewArrayExpression)y);
                case ExpressionType.Invoke:
                    return CompareInvocation((InvocationExpression)x, (InvocationExpression)y);
                case ExpressionType.MemberInit:
                    return CompareMemberInit((MemberInitExpression)x, (MemberInitExpression)y);
                case ExpressionType.ListInit:
                    return CompareListInit((ListInitExpression)x, (ListInitExpression)y);
                default:
                    throw new Exception($"Unhandled expression type: '{x.NodeType}'");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareListInit(ListInitExpression x, ListInitExpression y)
        {
            int result = CompareElementInitializerList(x.Initializers, y.Initializers);
            if (result != 0) return result;

            return CompareNew(x.NewExpression, y.NewExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected int CompareElementInitializerList(ReadOnlyCollection<ElementInit> x, ReadOnlyCollection<ElementInit> y)
        {
            int result = x.Count - y.Count;
            if (result != 0) return result;

            for (int i = 0; i < x.Count; i++)
            {
                result = CompareElementInitializer(x[i], y[i]);
                if (result != 0) return result;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareElementInitializer(ElementInit x, ElementInit y)
        {
            int result = CompareMemberInfo(x.AddMethod, y.AddMethod);
            if (result != 0) return result;

            return CompareExpressionList(x.Arguments, y.Arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareMemberInit(MemberInitExpression x, MemberInitExpression y)
        {
            int result = CompareNew(x.NewExpression, y.NewExpression);
            if (result != 0) return result;

            return CompareBindingList(x.Bindings, y.Bindings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareBindingList(ReadOnlyCollection<MemberBinding> x, ReadOnlyCollection<MemberBinding> y)
        {
            int result = x.Count - y.Count;
            if (result != 0) return result;

            for (int i = 0; i < x.Count; i++)
            {
                result = CompareBinding(x[i], y[i]);
                if (result != 0) return result;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareBinding(MemberBinding x, MemberBinding y)
        {
            int result = x.BindingType - y.BindingType;
            if (result != 0) return result;

            return CompareMemberInfo(x.Member, y.Member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareInvocation(InvocationExpression x, InvocationExpression y)
        {
            int result = CompareExpressionList(x.Arguments, y.Arguments);
            if (result != 0) return result;

            return Compare(x.Expression, y.Expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareNewArray(NewArrayExpression x, NewArrayExpression y)
        {
            return CompareExpressionList(x.Expressions, y.Expressions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareNew(NewExpression x, NewExpression y)
        {
            if (CompareNull(x.Members, y.Members, out var result)) return result;

            result = CompareMemberInfo(x.Constructor, y.Constructor);
            if (result != 0) return result;

            for (int i = 0; i < x.Members.Count; i++)
            {
                result = CompareMemberInfo(x.Members[i], y.Members[i]);
                if (result != 0) return result;
            }

            return CompareExpressionList(x.Arguments, y.Arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareLambda(LambdaExpression x, LambdaExpression y)
        {
            int result = x.Parameters.Count - y.Parameters.Count;
            if (result != 0) return result;

            for (int i = 0; i < x.Parameters.Count; i++)
            {
                result = CompareParameter(x.Parameters[i], y.Parameters[i]);
                if (result != 0) return result;
            }

            return Compare(x.Body, y.Body);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareMethodCall(MethodCallExpression x, MethodCallExpression y)
        {
            int result = CompareMemberInfo(x.Method, y.Method);
            if (result != 0) return result;

            result = CompareExpressionList(x.Arguments, y.Arguments);
            if (result != 0) return result;

            return Compare(x.Object, y.Object);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareExpressionList(ReadOnlyCollection<System.Linq.Expressions.Expression> x, ReadOnlyCollection<System.Linq.Expressions.Expression> y)
        {
            int result = x.Count - y.Count;
            if (result != 0) return result;

            for (int i = 0; i < x.Count; i++)
            {
                result = Compare(x[i], y[i]);
                if (result != 0) return result;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareMemberAccess(MemberExpression x, MemberExpression y)
        {
            int result = CompareMemberInfo(x.Member, y.Member);
            if (result != 0) return result;

            return Compare(x.Expression, y.Expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareParameter(ParameterExpression x, ParameterExpression y)
        {
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareConstant(ConstantExpression x, ConstantExpression y)
        {
            return Comparer.Default.Compare(x.Value, y.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareConditional(ConditionalExpression x, ConditionalExpression y)
        {
            int result = Compare(x.Test, y.Test);
            if (result != 0) return result;

            result = Compare(x.IfTrue, y.IfTrue);
            if (result != 0) return result;

            return Compare(x.IfFalse, y.IfFalse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareTypeIs(TypeBinaryExpression x, TypeBinaryExpression y)
        {
            int result = CompareType(x.TypeOperand, y.TypeOperand);
            if (result != 0) return result;

            return Compare(x.Expression, y.Expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareBinary(BinaryExpression x, BinaryExpression y)
        {
            int result = x.IsLifted.CompareTo(y.IsLifted);
            if (result != 0) return result;

            result = x.IsLiftedToNull.CompareTo(y.IsLiftedToNull);
            if (result != 0) return result;

            result = CompareMemberInfo(x.Method, y.Method);
            if (result != 0) return result;

            result = Compare(x.Left, y.Left);
            if (result != 0) return result;

            result = Compare(x.Right, y.Right);
            if (result != 0) return result;

            return Compare(x.Conversion, y.Conversion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual int CompareUnary(UnaryExpression x, UnaryExpression y)
        {
            int result = x.IsLifted.CompareTo(y.IsLifted);
            if (result != 0) return result;

            result = x.IsLiftedToNull.CompareTo(y.IsLiftedToNull);
            if (result != 0) return result;

            result = CompareMemberInfo(x.Method, y.Method);
            if (result != 0) return result;

            return Compare(x.Operand, y.Operand);
        }
    }
}
