//--------------------------------------------------------------------------------------------------
// <copyright file="AccountInquiry.cs" company="Nautech Systems Pty Ltd">
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

using System;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Message;
using Nautilus.DomainModel.Identifiers;
using NodaTime;

namespace Nautilus.DomainModel.Commands
{
    /// <summary>
    /// Represents a command for an account inquiry.
    /// </summary>
    [Immutable]
    public sealed class AccountInquiry : Command
    {
        private static readonly Type CommandType = typeof(AccountInquiry);

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInquiry"/> class.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="accountId">The account identifier for the inquiry.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public AccountInquiry(
            TraderId traderId,
            AccountId accountId,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                CommandType,
                commandId,
                commandTimestamp)
        {
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.TraderId = traderId;
            this.AccountId = accountId;
        }

        /// <summary>
        /// Gets the commands trader identifier.
        /// </summary>
        public TraderId TraderId { get; }

        /// <summary>
        /// Gets the commands account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(TraderId={this.TraderId.Value}, AccountId={this.AccountId.Value})";
    }
}
