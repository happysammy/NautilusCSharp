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
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// Represents an identifier for accounts.
    /// </summary>
    [Immutable]
    public sealed class AccountId : Identifier<Account>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountId"/> class.
        /// </summary>
        /// <param name="brokerage">The brokerage identifier.</param>
        /// <param name="accountNumber">The account number identifier.</param>
        public AccountId(Brokerage brokerage, AccountNumber accountNumber)
            : base($"{brokerage}-{accountNumber}")
        {
            this.Brokerage = brokerage;
            this.AccountNumber = accountNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountId"/> class.
        /// </summary>
        /// <param name="brokerage">The brokerage identifier.</param>
        /// <param name="accountNumber">The account number identifier.</param>
        public AccountId(string brokerage, string accountNumber)
            : this(new Brokerage(brokerage), new AccountNumber(accountNumber))
        {
        }

        /// <summary>
        /// Gets the account identifiers brokerage.
        /// </summary>
        public Brokerage Brokerage { get; }

        /// <summary>
        /// Gets the account identifiers account number.
        /// </summary>
        public AccountNumber AccountNumber { get; }

        /// <summary>
        /// Return a new <see cref="AccountId"/> from the given string.
        /// </summary>
        /// <param name="value">The account identifier value.</param>
        /// <returns>The account identifier.</returns>
        public static AccountId FromString(string value)
        {
            var splitString = value.Split("-", 2);

            return new AccountId(splitString[0], splitString[1]);
        }
    }
}
