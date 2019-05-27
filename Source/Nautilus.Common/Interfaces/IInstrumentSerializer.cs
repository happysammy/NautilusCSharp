//--------------------------------------------------------------------------------------------------
// <copyright file="IInstrumentSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// Provides a serializer for <see cref="Instrument"/>s.
    /// </summary>
    public interface IInstrumentSerializer
    {
        /// <summary>
        /// Serialize the given instrument.
        /// </summary>
        /// <param name="instrument">The instrument to serialize.</param>
        /// <returns>The serialized command.</returns>
        byte[] Serialize(Instrument instrument);

        /// <summary>
        /// Deserialize the given instrument bytes.
        /// </summary>
        /// <param name="serialized">The serialized instrument.</param>
        /// <returns>The deserialized instrument.</returns>
        Instrument Deserialize(byte[] serialized);
    }
}
