//--------------------------------------------------------------------------------------------------
// <copyright file="FixConfiguration.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Represents the configuration for a FIX session.
    /// </summary>
    [Immutable]
    public sealed class FixConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixConfiguration"/> class.
        /// </summary>
        /// <param name="broker">The FIX brokerage name.</param>
        /// <param name="accountType">The FIX account type.</param>
        /// <param name="accountCurrency">The FIX account currency.</param>
        /// <param name="configPath">The FIX configuration file path.</param>
        /// <param name="credentials">The FIX credentials.</param>
        /// <param name="sendAccountTag">The option flag to send account tags with messages.</param>
        /// <param name="connectWeeklyTime">The time to connect FIX sessions.</param>
        /// <param name="disconnectWeeklyTime">The time to disconnect FIX sessions.</param>
        public FixConfiguration(
            Brokerage broker,
            AccountType accountType,
            Currency accountCurrency,
            string configPath,
            FixCredentials credentials,
            bool sendAccountTag,
            WeeklyTime connectWeeklyTime,
            WeeklyTime disconnectWeeklyTime)
        {
            Condition.NotEmptyOrWhiteSpace(configPath, nameof(configPath));

            this.AccountId = new AccountId(broker, credentials.AccountNumber, accountType);
            this.Broker = broker;
            this.AccountType = accountType;
            this.AccountCurrency = accountCurrency;
            this.ConfigPath = configPath;
            this.Credentials = credentials;
            this.SendAccountTag = sendAccountTag;
            this.ConnectWeeklyTime = connectWeeklyTime;
            this.DisconnectWeeklyTime = disconnectWeeklyTime;
        }

        /// <summary>
        /// Gets the FIX configuration account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the FIX brokerage name.
        /// </summary>
        public Brokerage Broker { get; }

        /// <summary>
        /// Gets the FIX account type.
        /// </summary>
        public AccountType AccountType { get; }

        /// <summary>
        /// Gets the FIX account currency.
        /// </summary>
        public Currency AccountCurrency { get; }

        /// <summary>
        /// Gets the FIX configuration file path.
        /// </summary>
        public string ConfigPath { get; }

        /// <summary>
        /// Gets the FIX account credentials.
        /// </summary>
        public FixCredentials Credentials { get; }

        /// <summary>
        /// Gets a value indicating whether the Account tag should be sent with FIX messages.
        /// </summary>
        public bool SendAccountTag { get; }

        /// <summary>
        /// Gets the weekly time for connecting the FIX session.
        /// </summary>
        public WeeklyTime ConnectWeeklyTime { get; }

        /// <summary>
        /// Gets the weekly time for disconnecting the FIX session.
        /// </summary>
        public WeeklyTime DisconnectWeeklyTime { get; }
    }
}
