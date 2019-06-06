//--------------------------------------------------------------------------------------------------
// <copyright file="MessageType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Enums
{
    /// <summary>
    /// Represents the base type of a message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// The message type is unknown (this is an invalid value).
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The message is a type of command.
        /// </summary>
        Command = 1,

        /// <summary>
        /// The message is a type of event.
        /// </summary>
        Event = 2,

        /// <summary>
        /// The message is a type of document.
        /// </summary>
        Document = 3,

        /// <summary>
        /// The message is a type of request.
        /// </summary>
        Request = 4,

        /// <summary>
        /// The message is a type of response.
        /// </summary>
        Response = 5,
    }
}
