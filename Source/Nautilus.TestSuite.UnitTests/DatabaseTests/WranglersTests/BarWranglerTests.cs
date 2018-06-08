// -------------------------------------------------------------------------------------------------
// <copyright file="BarWranglerTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.WranglersTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Database.Wranglers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using Xunit;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarWranglerTests
    {
        [Fact]
        internal void ParseBars()
        {
            // Arrange
            var bar = StubBarData.Create();

            var expectedBarList = new List<Bar> { bar, bar, bar };
            var barStringArray = new List<byte[]>
            {
                bar.ToUtf8Bytes(),
                bar.ToUtf8Bytes(),
                bar.ToUtf8Bytes()
            }.ToArray();

            // Act
            var result = BarWrangler.ParseBars(barStringArray);

            // Assert
            Assert.Equal(expectedBarList, result);
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(1, 1, 2)]
        [InlineData(1, 2, 3)]
        internal void OrganizeBarsByDay(
            int offset1,
            int offset2,
            int expectedCount)
        {
            // Arrange
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(Duration.FromDays(offset1));
            var bar4 = StubBarData.Create(Duration.FromDays(offset2));

            var barList = new [] { bar1, bar2, bar3, bar4 };

            // Act
            var result = BarWrangler.OrganizeBarsByDay(barList);

            // Assert
            Assert.Equal(expectedCount, result.Keys.Count);
        }
    }
}
