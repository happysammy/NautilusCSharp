// -------------------------------------------------------------------------------------------------
// <copyright file="StartSystem.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Messages
{
    using System;
    using NodaTime;
    using Nautilus.Common.Messaging;

    public sealed class StartSystem : CommandMessage
    {
        public StartSystem(Guid identifier, ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
        }

        /// <summary>
        /// Gets a string representation of the <see cref="StartSystem"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(StartSystem)}-{this.Id}";
    }
}
