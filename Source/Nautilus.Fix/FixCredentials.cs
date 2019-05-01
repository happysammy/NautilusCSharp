//--------------------------------------------------------------------------------------------------
// <copyright file="FixCredentials.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents the credentials for a FIX session.
    /// </summary>
    [Immutable]
    public sealed class FixCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixCredentials"/> class.
        /// </summary>
        /// <param name="account">The FIX account number.</param>
        /// <param name="username">The FIX account username.</param>
        /// <param name="password">The FIX account password.</param>
        public FixCredentials(
            string account,
            string username,
            string password)
        {
            Precondition.NotEmptyOrWhiteSpace(account, nameof(account));
            Precondition.NotEmptyOrWhiteSpace(username, nameof(username));
            Precondition.NotEmptyOrWhiteSpace(password, nameof(password));

            this.Account = account;
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// Gets the FIX account.
        /// </summary>
        public string Account { get; }

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
