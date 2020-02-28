//--------------------------------------------------------------------------------------------------
// <copyright file="MessageType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a message type group.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The message is a UTF-8 encoded string.
        /// </summary>
        String = 1,

        /// <summary>
        /// The message is a type of command.
        /// </summary>
        Command = 2,

        /// <summary>
        /// The message is a type of document.
        /// </summary>
        Document = 3,

        /// <summary>
        /// The message is a type of event.
        /// </summary>
        Event = 4,

        /// <summary>
        /// The message is a type of request.
        /// </summary>
        Request = 5,

        /// <summary>
        /// The message is a type of response.
        /// </summary>
        Response = 6,
    }
}
