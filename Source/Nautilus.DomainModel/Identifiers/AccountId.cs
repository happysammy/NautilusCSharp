//--------------------------------------------------------------------------------------------------
// <copyright file="AccountId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents an identifier for accounts.
    /// </summary>
    [Immutable]
    public sealed class AccountId : Identifier<Account>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountId"/> class.
        /// </summary>
        /// <param name="value">The entity id value.</param>
        public AccountId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }

        /// <summary>
        /// Creates a new account identifier from the given arguments.
        /// </summary>
        /// <param name="brokerage">The brokerage for the identifier.</param>
        /// <param name="accountNumber">The account number for the identifier.</param>
        /// <returns>The created identifier.</returns>
        public static AccountId Create(Brokerage brokerage, string accountNumber)
        {
            return new AccountId($"{brokerage}-{accountNumber}");
        }
    }
}
