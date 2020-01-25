//--------------------------------------------------------------------------------------------------
// <copyright file="Stop.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Message;
    using NodaTime;

    /// <summary>
    /// Represents a command to stop a component.
    /// </summary>
    [Immutable]
    public sealed class Stop : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stop"/> class.
        /// </summary>
        /// <param name="id">The commands identifier.</param>
        /// <param name="timestamp">The commands timestamp.</param>
        public Stop(Guid id, ZonedDateTime timestamp)
            : base(typeof(Stop), id, timestamp)
        {
        }
    }
}
