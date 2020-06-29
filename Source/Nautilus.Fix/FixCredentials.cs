//--------------------------------------------------------------------------------------------------
// <copyright file="FixCredentials.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
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
