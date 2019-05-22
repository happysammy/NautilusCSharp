//--------------------------------------------------------------------------------------------------
// <copyright file="Stop.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using NodaTime;

    /// <summary>
    /// Represents a command to stop the component.
    /// </summary>
    [Immutable]
    public sealed class Stop : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stop"/> class.
        /// </summary>
        /// <param name="messageIdentifier">The commands identifier.</param>
        /// <param name="messageTimestamp">The commands timestamp.</param>
        public Stop(
            Guid messageIdentifier,
            ZonedDateTime messageTimestamp)
            : base(messageIdentifier, messageTimestamp)
        {
        }

        /// <summary>
        /// Returns a string representation of this message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(Stop)}({this.Identifier})";
    }
}
