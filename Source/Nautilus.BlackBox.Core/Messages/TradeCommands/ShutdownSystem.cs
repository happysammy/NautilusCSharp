//--------------------------------------------------------------
// <copyright file="ShutdownSystem.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.TradeCommands
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Messaging;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="ShutdownSystem"/> class.
    /// </summary>
    [Immutable]
    public sealed class ShutdownSystem : CommandMessage
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

        /// <summary>
        /// Returns a string representation of the <see cref="ShutdownSystem"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(ShutdownSystem);
    }
}
