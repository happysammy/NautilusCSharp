// -------------------------------------------------------------------------------------------------
// <copyright file="SessionId.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Identifiers;
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
        /// <param name="traderId">The session identifiers trader id.</param>
        /// <param name="dateTime">The date time the session was established.</param>
        public SessionId(TraderId traderId, ZonedDateTime dateTime)
         : this($"{traderId.Value}-{dateTime.Date.ToIso8601String()}-{dateTime.TickOfDay}")
        {
        }

        /// <summary>
        /// Return a none session identifier.
        /// </summary>
        /// <returns>The session identifier.</returns>
        public static SessionId None() => new SessionId(nameof(None));
    }
}
