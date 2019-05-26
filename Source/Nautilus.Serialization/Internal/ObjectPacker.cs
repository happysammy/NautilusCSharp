// -------------------------------------------------------------------------------------------------
// <copyright file="ObjectPacker.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
{
    using MsgPack;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides methods for converting objects to <see cref="MessagePackObject"/>s.
    /// </summary>
    internal static class ObjectPacker
    {
        /// <summary>
        /// Return a <see cref="MessagePackObject"/> from the given price.
        /// </summary>
        /// <param name="price">The nullable price.</param>
        /// <returns>The <see cref="MessagePackObject"/>.</returns>
        internal static MessagePackObject NullablePrice(Price? price)
        {
            return price is null
                ? MessagePackObject.Nil
                : price.ToString();
        }

        /// <summary>
        /// Returns a <see cref="MessagePackObject"/> from the given nullable expire time.
        /// </summary>
        /// <param name="dateTime">The nullable zoned date time.</param>
        /// <returns>The <see cref="MessagePackObject"/>.</returns>
        internal static MessagePackObject NullableZonedDateTime(ZonedDateTime? dateTime)
        {
            return dateTime is null
                ? MessagePackObject.Nil
                : dateTime.Value.ToIsoString();
        }
    }
}
