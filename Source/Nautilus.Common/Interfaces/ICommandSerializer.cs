//--------------------------------------------------------------------------------------------------
// <copyright file="ICommandSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Core;

    /// <summary>
    /// Provides a binary serializer for <see cref="Command"/> messages.
    /// </summary>
    public interface ICommandSerializer
    {
        /// <summary>
        /// Returns the serialized command bytes.
        /// </summary>
        /// <param name="command">The command to serialize.</param>
        /// <returns>The serialized command bytes.</returns>
        byte[] Serialize(Command command);

        /// <summary>
        /// Returns the deserialize <see cref="Command"/>.
        /// </summary>
        /// <param name="commandBytes">The command bytes to deserialize.</param>
        /// <returns>The deserialized <see cref="Command"/>.</returns>
        Command Deserialize(byte[] commandBytes);
    }
}
