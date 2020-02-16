// -------------------------------------------------------------------------------------------------
// <copyright file="ScheduledSend.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduling.Internal
{
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// INTERNAL API.
    /// </summary>
    internal sealed class ScheduledSend : IRunnable
    {
        private readonly IEndpoint receiver;
        private readonly object message;
        private readonly IEndpoint sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledSend"/> class.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="sender">The message sender.</param>
        internal ScheduledSend(IEndpoint receiver, object message, IEndpoint sender)
        {
            this.receiver = receiver;
            this.message = message;
            this.sender = sender;
        }

        /// <inheritdoc/>
        public void Run()
        {
            this.receiver.Send(this.message);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{this.receiver}.Send({this.message}, {this.sender})]";
        }
    }
}
