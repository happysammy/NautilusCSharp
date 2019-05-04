// -------------------------------------------------------------------------------------------------
// <copyright file="SendMessageTask.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System;

    /// <summary>
    /// Represents a task to send a single message.
    /// </summary>
    public sealed class SendMessageTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessageTask"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="onSendComplete">The on send complete action.</param>
        public SendMessageTask(object payload, Action<bool> onSendComplete)
        {
            this.Payload = payload;
            this.OnSendComplete = onSendComplete;
        }

        /// <summary>
        /// Gets the tasks payload.
        /// </summary>
        public object Payload { get; }

        /// <summary>
        /// Gets the tasks on send complete action.
        /// </summary>
        public Action<bool> OnSendComplete { get; }
    }
}
