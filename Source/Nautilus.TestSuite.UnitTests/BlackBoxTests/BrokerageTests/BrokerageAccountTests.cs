//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerageAccountTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.BrokerageTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BrokerageAccountTests
    {
        private readonly IZonedClock clock;
        private readonly CurrencyCode currency;

        public BrokerageAccountTests()
        {
            this.clock = new StubClock();
            this.currency = CurrencyCode.AUD;
        }

        [Fact]
        internal void CanSetupAccount_EqualsInitializedValues()
        {
            // Arrange
            // Act
            var account = new BrokerageAccount(
                Broker.FXCM,
                "some username",
                "some password",
                "123456789",
                this.currency,
                this.clock.TimeNow());

            // Assert
            Assert.Equal(Broker.FXCM, account.Broker);
            Assert.Equal("FXCM-123456789", account.AccountId.ToString());
            Assert.Equal(this.currency, account.Currency);
            Assert.Equal(decimal.Zero, account.CashBalance.Value);
        }

        [Fact]
        internal void Update_MessageCorrect_EqualsUpdatedStatusValues()
        {
            // Arrange
            var account = new BrokerageAccount(
                Broker.FXCM,
                "123456789",
                "some username",
                "some password",
                this.currency,
                this.clock.TimeNow());

            var message = new AccountEvent(
                Broker.FXCM,
                "123456789",
                Money.Create(150000m, this.currency),
                Money.Create(150000m, this.currency),
                Money.Zero(this.currency),
                Money.Zero(this.currency),
                Money.Zero(this.currency),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            account.Apply(message);

            // Assert
            Assert.Equal(150000m, account.CashBalance.Value);
            Assert.Equal(150000m, account.CashStartDay.Value);
            Assert.Equal(0m, account.CashActivityDay.Value);
            Assert.Equal(0m, account.MarginRatio);
            Assert.Equal(0m, account.MarginUsedMaintenance.Value);
            Assert.Equal(0m, account.MarginUsedLiquidation.Value);
            Assert.Equal(string.Empty, account.MarginCallStatus);
        }

        [Fact]
        internal void FreeEquity_MessageCorrect_ReturnsTrue()
        {
            // Arrange
            var account = new BrokerageAccount(
                Broker.FXCM,
                "123456789",
                "some username",
                "some password",
                CurrencyCode.USD,
                this.clock.TimeNow());

            var message = new AccountEvent(
                Broker.FXCM,
                "123456789",
                Money.Create(150000m, this.currency),
                Money.Create(150000m, this.currency),
                Money.Zero(this.currency),
                Money.Zero(this.currency),
                Money.Create(2000m, this.currency),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                this.clock.TimeNow());

            // Act
            account.Apply(message);
            var result = account.FreeEquity;

            // Assert
            Assert.Equal(150000m - 2000m, result.Value);
        }
    }
}
