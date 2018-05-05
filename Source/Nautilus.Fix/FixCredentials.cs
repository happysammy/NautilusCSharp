//--------------------------------------------------------------------------------------------------
// <copyright file="FixCredentials.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using NautechSystems.CSharp.Validation;

    public class FixCredentials
    {
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

        public string Username { get; }
        public string Password { get; }
        public string AccountNumber { get; }
    }
}
