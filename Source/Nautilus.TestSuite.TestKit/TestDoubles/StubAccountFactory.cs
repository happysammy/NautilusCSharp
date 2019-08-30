﻿//--------------------------------------------------------------------------------------------------
// <copyright file="StubAccountFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubAccountFactory
    {
        public static Account Create()
        {
            var account = new Account(
                new Brokerage("IB"),
                new AccountNumber("123456789"),
                Currency.USD,
                StubZonedDateTime.UnixEpoch());

            var accountEventMessage = new AccountStateEvent(
                new AccountId("FXCM", "D1234565"),
                Currency.USD,
                Money.Create(100000, Currency.USD),
                Money.Create(100000, Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
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
                new Brokerage("IB"),
                new AccountNumber("123456789"),
                Currency.USD,
                StubZonedDateTime.UnixEpoch());

            var accountEventMessage = new AccountStateEvent(
                new AccountId("FXCM", "D1234565"),
                Currency.USD,
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            account.Apply(accountEventMessage);

            return account;
        }
    }
}
