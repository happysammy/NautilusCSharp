//--------------------------------------------------------------
// <copyright file="InitializeMessageSwitchboard.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="InitializeMessageSwitchboard"/> class.
    /// </summary>
    [Immutable]
    public sealed class InitializeMessageSwitchboard : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeMessageSwitchboard"/> class.
        /// </summary>
        /// <param name="switchboard">The message switchboard.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public InitializeMessageSwitchboard(
            Switchboard switchboard,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(switchboard, nameof(switchboard));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.Switchboard = switchboard;
        }

        /// <summary>
        /// Gets the messages account.
        /// </summary>
        public Switchboard Switchboard { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="InitializeMessageSwitchboard"/>
        /// service message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(InitializeMessageSwitchboard);
    }
}
