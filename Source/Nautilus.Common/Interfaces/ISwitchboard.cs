//--------------------------------------------------------------------------------------------------
// <copyright file="ISwitchboard.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

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
