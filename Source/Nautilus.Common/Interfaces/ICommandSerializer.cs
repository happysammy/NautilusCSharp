//--------------------------------------------------------------------------------------------------
// <copyright file="ICommandSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Common.Messaging;

    /// <summary>
    /// Provides an interface for command serializers.
    /// </summary>
    public interface ICommandSerializer
    {
        /// <summary>
        /// Serialize the given command object.
        /// </summary>
        /// <param name="command">The command message to serialize.</param>
        /// <returns>The serialized command.</returns>
        byte[] Serialize(CommandMessage command);

        /// <summary>
        /// Deserialize the given command byte[].
        /// </summary>
        /// <param name="commandBytes">The command bytes to deserialize.</param>
        /// <returns>The deserialized command.</returns>
        CommandMessage Deserialize(byte[] commandBytes);
    }
}
