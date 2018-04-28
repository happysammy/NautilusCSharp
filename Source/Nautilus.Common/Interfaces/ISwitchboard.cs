//--------------------------------------------------------------
// <copyright file="ISwitchboard.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Common.Messaging;

    /// <summary>
    /// Represents a messaging switchboard of all addresses within the system.
    /// </summary>
    public interface ISwitchboard
    {
        /// <summary>
        /// Sends the given message envelope to the receivers.
        /// </summary>
        /// <param name="envelope">The message envelope.</param>
        /// <typeparam name="T">The message type.</typeparam>
        void SendToReceivers<T>(Envelope<T> envelope)
            where T : Message;
    }
}
