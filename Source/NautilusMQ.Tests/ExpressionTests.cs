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
            var body = ExpressionBuilder.ReferenceTypeHandling(handler, returnTarget);
            var lambda = Expression.Lambda<Action<object>>(body, message).Compile();

            lambda.Invoke("test");

            // Assert
            Assert.True(returnTarget.Equals(true));
        }
    }
}
