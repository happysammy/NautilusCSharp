//--------------------------------------------------------------------------------------------------
// <copyright file="MessageTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.FixTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Fxcm.MessageFactories;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class MessageTests
    {
        [Fact]
        internal void MarketDataRequestMessage()
        {
            // Arrange
            // Act
            var message = MarketDataRequestFactory.Create("AUD/USD", 1, StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("8=FIX.4.49=6735=V262=MD_0263=1264=1265=1267=2269=0269=1146=155=AUD/USD10=239", message.ToString());
        }
    }
}
