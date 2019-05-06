//--------------------------------------------------------------------------------------------------
// <copyright file="ExpressionBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusMQ.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a builder for a <see cref="MessageProcessor"/>(s) handler expression tree.
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        /// Build the message handling delegate by compiling an expression tree based on the
        /// given parameters.
        /// </summary>
        /// <param name="handlers">The handlers.</param>
        /// <param name="unhandled">The unhandled message delegate.</param>
        /// <returns>The created delegate.</returns>
        internal static Func<object, Task> Build(
            IReadOnlyCollection<Handler> handlers,
            Action<object> unhandled)
        {
            var message = Expression.Parameter(typeof(object), "message");
            var returnTarget = Expression.Label(typeof(bool), "return");

            var mainBodyExpressions = new List<Expression>();
            foreach (var handler in handlers)
            {
                if (handler.Type.GetTypeInfo().IsValueType)
                {
                    mainBodyExpressions.Add(ValueTypeHandling(handler, returnTarget));
                }
                else
                {
                    mainBodyExpressions.Add(ReferenceTypeHandling(handler, returnTarget));
                }
            }


            Expression<Func<object, Task>> unhandledExpression = msg => unhandled(msg);
            var unhandledCall = Expression.Call(
                unhandledExpression,
                typeof(Func<object, Task>).GetMethod(nameof(Func<object, Task>.Invoke)),
                message);
            expressions.Add(unhandledCall);

            var body = Expression.Block(typeof(Task), mainBodyExpressions.ToArray());
            var tree = Expression.Lambda<Func<object, Task>>(body, message);

            return tree.Compile();
        }

        private static Expression ValueTypeHandling(Handler handler, LabelTarget returnTarget)
        {
            var message = Expression.Parameter(handler.Type, "message");
            var castedVariable = Expression.Variable(handler.Type);
            var tryCast = Expression.Assign(castedVariable, Expression.Convert(message, handler.Type));

            var ifTrueBlock = new List<Expression>();
            Expression<Action<object>> handlingExpression = msg => handler.Handle(msg);
            ifTrueBlock.Add(Expression.Call(
                handlingExpression,
                typeof(Action<object>).GetMethod(nameof(Action<object>.Invoke)),
                message));
            ifTrueBlock.Add(Expression.Return(returnTarget, Expression.Constant(true)));

            return Expression.IfThenElse(
                Expression.TypeIs(tryCast, handler.Type),
                Expression.Block(ifTrueBlock),
                Expression.Return(returnTarget, Expression.Constant(false)));
        }

        private static Expression ReferenceTypeHandling(Handler handler, LabelTarget returnTarget)
        {
            var message = Expression.Parameter(handler.Type, "message");
            var castedVariable = Expression.Variable(handler.Type);
            var tryCast = Expression.Assign(castedVariable, Expression.TypeAs(message, handler.Type));

            var ifTrueBlock = new List<Expression>();
            Expression<Action<object>> handlingExpression = msg => handler.Handle(msg);
            ifTrueBlock.Add(Expression.Call(
                handlingExpression,
                typeof(Action<object>).GetMethod(nameof(Action<object>.Invoke)),
                message));
            ifTrueBlock.Add(Expression.Return(returnTarget, Expression.Constant(true)));

            return Expression.IfThenElse(
                Expression.TypeIs(tryCast, handler.Type),
                Expression.Block(ifTrueBlock),
                Expression.Return(returnTarget, Expression.Constant(false)));
        }
    }
}
