//--------------------------------------------------------------------------------------------------
// <copyright file="StubAccountFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Creates an example stub order with default but correct values.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubAccountFactory
    {
        public static Account Create()
        {
            var account = new Account(
                Broker.InteractiveBrokers,
                "123456789",
                "my_username",
                "my_password",
                CurrencyCode.USD,
                StubZonedDateTime.UnixEpoch());

            var accountEventMessage = new AccountEvent(
                Broker.InteractiveBrokers,
                "123456789",
                CurrencyCode.USD,
                Money.Create(100000, CurrencyCode.USD),
                Money.Create(100000, CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            account.Apply(accountEventMessage);

            return account;
        }

        public static Account ZeroCash()
        {
            var account = new Account(
                Broker.InteractiveBrokers,
                "123456789",
                "my_username",
                "my_password",
                CurrencyCode.USD,
                StubZonedDateTime.UnixEpoch());

            var accountEventMessage = new AccountEvent(
                Broker.InteractiveBrokers,
                "123456789",
                CurrencyCode.USD,
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            account.Apply(accountEventMessage);

            return account;
        }
    }
}
