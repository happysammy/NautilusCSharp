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
        /// Returns the given <see cref="Instrument"/> as serialized bytes.
        /// </summary>
        /// <param name="instrument">The instrument to serialize.</param>
        /// <returns>The serialized instrument bytes.</returns>
        byte[] Serialize(Instrument instrument);

        /// <summary>
        /// Returns the deserialize <see cref="Instrument"/> from the given bytes.
        /// </summary>
        /// <param name="serialized">The serialized instrument.</param>
        /// <returns>The deserialized <see cref="Instrument"/>.</returns>
        Instrument Deserialize(byte[] serialized);
    }
}
