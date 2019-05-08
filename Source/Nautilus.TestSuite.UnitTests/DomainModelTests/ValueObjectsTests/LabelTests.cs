//--------------------------------------------------------------------------------------------------
// <copyright file="LabelTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class LabelTests
    {
        [Fact]
        internal void Equals_IdenticalComponentLabels_ReturnsTrue()
        {
            // Arrange
            var label1 = LabelFactory.Create("TimeBarAggregator", new Symbol("AUDUSD", Venue.LMAX));
            var label2 = LabelFactory.Create("TimeBarAggregator", new Symbol("AUDUSD", Venue.LMAX));

            // Act
            var result = label1.Equals(label2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void GetHashcode_WithNormalComponentLabel_ReturnsExpectedHashCode()
        {
            // Arrange
            var label = new Label("ExecutionService");

            // Act
            var result = label.GetHashCode();

            // Assert
            Assert.Equal(typeof(int), result.GetType());
        }

        [Fact]
        internal void Equals_WithTheSameValue_ReturnsTrue()
        {
            // Arrange
            var label1 = LabelFactory.Create("Portfolio", new Symbol("AUDUSD", Venue.LMAX));
            var label2 = LabelFactory.Create("Portfolio", new Symbol("AUDUSD", Venue.LMAX));

            // Act
            var result1 = label1.Equals(label2);
            var result2 = label1 == label2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void Equals_WithObjectOfDifferentType_ReturnsFalse()
        {
            // Arrange
            var label1 = LabelFactory.Create("Portfolio", new Symbol("AUDUSD", Venue.LMAX));
            const string obj = "some_random_object";

            // Act (ignore the warning, this is why the result is false!).
            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = label1.Equals(obj);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void Equals_WithDifferentComponentLabels_ReturnsFalse()
        {
            // Arrange
            var label1 = new Label("SecurityPortfolio");
            var label2 = new Label("TradeBook");

            // Act
            var result = label1.Equals(label2);

            // Assert
            Assert.False(result);
        }
    }
}
