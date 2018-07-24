//--------------------------------------------------------------------------------------------------
// <copyright file="InitializeBrokerageGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="InitializeGateway"/> class.
    /// </summary>
    [Immutable]
    public sealed class InitializeGateway : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeGateway"/> class.
        /// </summary>
        /// <param name="fixGateway">The message brokerage gateway.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public InitializeGateway(
            IFixGateway fixGateway,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(fixGateway, nameof(fixGateway));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.FixGateway = fixGateway;
        }

        /// <summary>
        /// Gets the messages brokerage gateway.
        /// </summary>
        public IFixGateway FixGateway { get; }
    }
}
