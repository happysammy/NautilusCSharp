// -------------------------------------------------------------------------------------------------
// <copyright file="SessionId.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
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
