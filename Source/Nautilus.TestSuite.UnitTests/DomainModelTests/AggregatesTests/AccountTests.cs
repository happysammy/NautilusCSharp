//--------------------------------------------------------------------------------------------------
// <copyright file="AccountTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.AggregatesTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Suppression is OK within the Test Suite.")]
    public sealed class AccountTests
    {
        private readonly IZonedClock clock;

        public AccountTests()
        {
            this.clock = new TestClock();
        }

        [Fact]
        internal void CanSetupAccount_EqualsInitializedValues()
        {
            // Arrange
            // Act
            var account = StubAccountProvider.ZeroCash();

            // Assert
            Assert.Equal(new Brokerage("IB"), account.Brokerage);
            Assert.Equal("IB-123456789-SIMULATED", account.Id.Value);
            Assert.Equal(Currency.USD, account.Currency);
            Assert.Equal(decimal.Zero, account.CashBalance.Value);
        }

        [Fact]
        internal void Update_MessageCorrect_EqualsUpdatedStatusValues()
        {
            // Arrange
            var account = StubAccountProvider.Create();
            var message = new AccountStateEvent(
                new AccountId("FXCM", "123456789", "SIMULATED"),
                Currency.AUD,
                Money.Create(150000m, Currency.AUD),
                Money.Create(150000m, Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Zero(Currency.AUD),
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
            var account = StubAccountProvider.Create();
            var message = new AccountStateEvent(
                new AccountId("FXCM", "123456789", "SIMULATED"),
                Currency.USD,
                Money.Create(150000m, Currency.AUD),
                Money.Create(150000m, Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Zero(Currency.AUD),
                Money.Create(2000m, Currency.AUD),
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
