//--------------------------------------------------------------------------------------------------
// <copyright file="AccountId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents a valid account identifier. The identifier values combination must be unique at
    /// the fund level.
    /// </summary>
    [Immutable]
    public sealed class AccountId : Identifier<AccountId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountId"/> class.
        /// </summary>
        /// <param name="broker">The broker identifier.</param>
        /// <param name="accountNumber">The account number identifier.</param>
        /// <param name="accountType">The account type identifier value.</param>
        public AccountId(Brokerage broker, AccountNumber accountNumber, AccountType accountType)
            : base($"{broker.Value}-{accountNumber.Value}-{accountType}")
        {
            this.Broker = broker;
            this.AccountNumber = accountNumber;
            this.AccountType = accountType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountId"/> class.
        /// </summary>
        /// <param name="brokerage">The broker identifier value.</param>
        /// <param name="accountNumber">The account number identifier value.</param>
        /// <param name="accountType">The account type identifier value.</param>
        public AccountId(string brokerage, string accountNumber, string accountType)
            : this(
                new Brokerage(brokerage),
                new AccountNumber(accountNumber),
                accountType.ToEnum<AccountType>())
        {
            Debug.NotEmptyOrWhiteSpace(brokerage, nameof(brokerage));
            Debug.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));
        }

        /// <summary>
        /// Gets the identifiers brokerage.
        /// </summary>
        public Brokerage Broker { get; }

        /// <summary>
        /// Gets the identifiers account number.
        /// </summary>
        public AccountNumber AccountNumber { get; }

        /// <summary>
        /// Gets the identifiers account type.
        /// </summary>
        public AccountType AccountType { get; }

        /// <summary>
        /// Returns a new <see cref="AccountId"/> from the given string.
        /// </summary>
        /// <param name="value">The account identifier value.</param>
        /// <returns>The account identifier.</returns>
        public static AccountId FromString(string value)
        {
            var splitString = value.Split("-", 3);

            return new AccountId(
                splitString[0],
                splitString[1],
                splitString[2]);
        }
    }
}
