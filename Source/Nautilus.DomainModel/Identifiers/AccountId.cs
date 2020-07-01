//--------------------------------------------------------------------------------------------------
// <copyright file="AccountId.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Extensions;
using Nautilus.Core.Types;
using Nautilus.DomainModel.Enums;

namespace Nautilus.DomainModel.Identifiers
{
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
            : base($"{broker.Value}-{accountNumber.Value}-{accountType.ToString().ToUpper()}")
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
