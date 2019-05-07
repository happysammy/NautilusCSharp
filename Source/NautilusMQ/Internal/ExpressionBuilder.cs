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
            IReadOnlyList<Handler> handlers,
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
                    mainBodyExpressions.Add(ReferenceTypeHandling(handler, returnTarget, message));
                }
            }

// Expression<Func<object, Task>> unhandledExpression = msg => unhandled(msg);
//            var unhandledCall = Expression.Call(
//                unhandledExpression,
//                typeof(Func<object, Task>).GetMethod(nameof(Func<object, Task>.Invoke)),
//                message);
//            expressions.Add(unhandledCall);
            mainBodyExpressions.Add(Expression.Label(returnTarget, Expression.Constant(false)));
            var body = Expression.Block(typeof(Task), mainBodyExpressions.ToArray());
            var lambda = Expression.Lambda<Func<object, Task>>(body, message);

            return lambda.Compile();
        }

        /// <summary>
        /// TBD.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="returnTarget">The return target.</param>
        /// <returns>The created expression.</returns>
        internal static Expression ValueTypeHandling(Handler handler, LabelTarget returnTarget)
        {
            var message = Expression.Parameter(handler.Type, "message");
            var castedVariable = Expression.Variable(handler.Type);
            var tryCast = Expression.Assign(castedVariable, Expression.Convert(message, handler.Type));

            var ifTrueBlock = new List<Expression>();
            Expression<Action<object>> handlerExpression = msg => handler.Handle(msg);
            ifTrueBlock.Add(Expression.Call(
                handlerExpression,
                typeof(Action<object>).GetMethod(nameof(Action<object>.Invoke)),
                message));
            ifTrueBlock.Add(Expression.Return(returnTarget, Expression.Constant(true)));

            return Expression.IfThenElse(
                Expression.TypeIs(tryCast, handler.Type),
                Expression.Block(ifTrueBlock),
                Expression.Return(returnTarget, Expression.Constant(false)));
        }

        /// <summary>
        /// TBD.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="returnTarget">The return target.</param>
        /// <param name="message">The message expression.</param>
        /// <returns>The created expression.</returns>
        internal static Expression ReferenceTypeHandling(Handler handler, LabelTarget returnTarget, Expression message)
        {
            var castedVariable = Expression.Variable(handler.Type, $"handlerType {handler.Type}");
            var tryCast = Expression.Assign(castedVariable, Expression.Convert(message, handler.Type));

            var ifTrueBlock = new List<Expression>();
            Expression<Action<object>> handlerExpression = msg => handler.Handle(msg);
            ifTrueBlock.Add(Expression.Call(
                handlerExpression,
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
