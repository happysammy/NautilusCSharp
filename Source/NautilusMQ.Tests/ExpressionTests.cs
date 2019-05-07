//--------------------------------------------------------------------------------------------------
// <copyright file="ExpressionTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusMQ.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using NautilusMQ.Internal;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ExpressionTests
    {
        [Fact]
        internal void IfThenExpression()
        {
            // Arrange
            var message = Expression.Parameter(typeof(object), "message");
            var returnTarget = Expression.Label(typeof(bool), "return");

            var receiver = new List<string>();
            var handler = Handler.Create<string>(receiver.Add);

            // Act
            var body = ExpressionBuilder.ReferenceTypeHandling(handler, returnTarget, message);
            var lambda = Expression.Lambda<Action<object>>(body, message).Compile();

            lambda.Invoke("test");

            // Assert
            Assert.True(returnTarget.Equals(true));
        }

        [Fact]
        internal void SwitchExpression()
        {
            // Arrange
            var receiver = new List<string>();
            var unhandled = new List<object>();

            var handleString = Handler.Create<string>(receiver.Add);
            var handleAny = Handler.Create<object>(unhandled.Add);

            var messageObject = Expression.Parameter(typeof(object), "message");
            var messageString = Expression.Parameter(typeof(string), "message");
            var parameters = new List<ParameterExpression> { messageObject, messageString }.ToArray();

            Expression<Action<string>> handleExpression = msg => handleString.Handle(msg);
            var callHandleString = Expression.Call(
                handleExpression,
                typeof(Action<string>).GetMethod(nameof(Action<string>.Invoke)),
                messageString);

            var castedVariable = Expression.Variable(handleString.Type, $"handlerType {handleString.Type}");
            var tryCast = Expression.Assign(castedVariable, Expression.Convert(messageString, handleString.Type));
            var switch1 = Expression.SwitchCase(callHandleString, Expression.TypeIs(tryCast, handleString.Type));

            Expression<Action<object>> defaultExpression = msg => handleAny.Handle(msg);
            var defaultHandleAny = Expression.Call(
                defaultExpression,
                typeof(Action<object>).GetMethod(nameof(Action<object>.Invoke)),
                messageObject);

            var bodies = new List<Expression> { callHandleString, defaultHandleAny, }.ToArray();

            // Act
            var body = Expression.Switch(Expression.Constant(true, typeof(bool)), defaultHandleAny, switch1);
            var lambda = Expression.Lambda<Action<object>>(bodies, parameters).Compile();

            lambda.Invoke("test");

            // Assert
            Assert.Contains("test", receiver);
        }
    }
}
