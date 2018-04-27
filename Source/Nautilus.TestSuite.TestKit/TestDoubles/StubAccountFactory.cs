// -------------------------------------------------------------------------------------------------
// <copyright file="StubAccountFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.Brokerage;
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
        public static BrokerageAccount Create()
        {
            var account = new BrokerageAccount(
                Broker.InteractiveBrokers,
                "123456789",
                "my_username",
                "my_password",
                CurrencyCode.USD,
                StubDateTime.Now());

            var accountEventMessage = new AccountEvent(
                Broker.InteractiveBrokers,
                "123456789",
                Money.Create(100000, CurrencyCode.USD),
                Money.Create(100000, CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                StubDateTime.Now());

            account.Apply(accountEventMessage);

            return account;
        }

        public static BrokerageAccount ZeroCash()
        {
            var account = new BrokerageAccount(
                Broker.InteractiveBrokers,
                "123456789",
                "my_username",
                "my_password",
                CurrencyCode.USD,
                StubDateTime.Now());

            var accountEventMessage = new AccountEvent(
                Broker.InteractiveBrokers,
                "123456789",
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                StubDateTime.Now());

            account.Apply(accountEventMessage);

            return account;
        }
    }
}
