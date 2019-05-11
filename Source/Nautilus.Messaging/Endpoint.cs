// -------------------------------------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Represents a messaging endpoint.
    /// </summary>
    [Immutable]
    public sealed class Endpoint : IEndpoint
    {
        private readonly Func<object, bool> target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="target">The target delegate for the end point.</param>
        public Endpoint(Func<object, bool> target)
        {
            this.target = target;
        }

        /// <summary>
        /// Send the given message to the endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(object message)
        {
            this.target.Invoke(message);
        }
    }
}
