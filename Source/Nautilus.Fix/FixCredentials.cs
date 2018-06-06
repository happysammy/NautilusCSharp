//--------------------------------------------------------------------------------------------------
// <copyright file="FixCredentials.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents the credentials for a FIX session.
    /// </summary>
    public class FixCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixCredentials"/> class.
        /// </summary>
        /// <param name="username">The FIX account username.</param>
        /// <param name="password">The FIX account password.</param>
        /// <param name="accountNumber">The FIX account number.</param>
        public FixCredentials(
            string username,
            string password,
            string accountNumber)
        {
            Validate.NotNull(username, nameof(username));
            Validate.NotNull(username, nameof(username));
            Validate.NotNull(username, nameof(username));

            this.Username = username;
            Password = password;
            AccountNumber = accountNumber;
        }

        /// <summary>
        /// Gets the FIX account username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the FIX account password.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets the FIX account number.
        /// </summary>
        public string AccountNumber { get; }
    }
}
