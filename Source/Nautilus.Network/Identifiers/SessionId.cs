// -------------------------------------------------------------------------------------------------
// <copyright file="SessionId.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Identifiers
{
    using System.Security.Cryptography;
    using System.Text;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using NodaTime;

    /// <summary>
    /// Represents a unique network session identifier.
    /// </summary>
    [Immutable]
    public sealed class SessionId : Identifier<SessionId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionId"/> class.
        /// </summary>
        /// <param name="value">The value of the session identifier.</param>
        public SessionId(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionId"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="dateTime">The date time the session was established.</param>
        /// <param name="secret">The user secret for authentication.</param>
        /// <returns>The session identifier.</returns>
        public static SessionId Create(ClientId clientId, ZonedDateTime dateTime, string secret)
        {
            var authentication = $"{clientId.Value}-{dateTime.ToIso8601String()}-{secret}";

            return new SessionId($"{clientId.Value}-{Sha256(authentication)}");
        }

        /// <summary>
        /// Return a none session identifier.
        /// </summary>
        /// <returns>The session identifier.</returns>
        public static SessionId None() => new SessionId(nameof(None));

        private static string Sha256(string value)
        {
            var cipher = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(value));
            var hash = new StringBuilder();
            foreach (var part in cipher)
            {
                hash.Append(part.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}
