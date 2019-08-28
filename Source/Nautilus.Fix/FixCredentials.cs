//--------------------------------------------------------------------------------------------------
// <copyright file="FixCredentials.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Represents the credentials for a FIX session.
    /// </summary>
    [Immutable]
    public sealed class FixCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixCredentials"/> class.
        /// </summary>
        /// <param name="accountNumber">The FIX account number.</param>
        /// <param name="username">The FIX account username.</param>
        /// <param name="password">The FIX account password.</param>
        public FixCredentials(
            string accountNumber,
            string username,
            string password)
        {
            Condition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));
            Condition.NotEmptyOrWhiteSpace(username, nameof(username));
            Condition.NotEmptyOrWhiteSpace(password, nameof(password));

            this.AccountNumber = new AccountNumber(accountNumber);
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// Gets the FIX account.
        /// </summary>
        public AccountNumber AccountNumber { get; }

        /// <summary>
        /// Gets the FIX account username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the FIX account password.
        /// </summary>
        public string Password { get; }
    }
}
