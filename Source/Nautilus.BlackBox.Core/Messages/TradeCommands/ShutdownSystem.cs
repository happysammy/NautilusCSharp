//--------------------------------------------------------------------------------------------------
// <copyright file="ShutdownSystem.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.TradeCommands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Core;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="ShutdownSystem"/> class.
    /// </summary>
    [Immutable]
    public sealed class ShutdownSystem : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownSystem"/> class.
        /// </summary>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public ShutdownSystem(
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));
        }
    }
}
