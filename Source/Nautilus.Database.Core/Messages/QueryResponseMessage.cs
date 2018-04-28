// -------------------------------------------------------------------------------------------------
// <copyright file="QueryResponseMessage.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Messages
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;

    using Nautilus.Common.Messaging;

    /// <summary>
    /// The base class for all response message types.
    /// </summary>
    [Immutable]
    public abstract class QueryResponseMessage : Message
    {
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected QueryResponseMessage(
            bool isSuccess,
            string message,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.IsSuccess = isSuccess;
            this.Message = message;
        }

        /// <summary>
        /// Gets a value indicating whether the result of the response message is successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the message of the response message.
        /// </summary>
        public string Message { get; }
    }
}
