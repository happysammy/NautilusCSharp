// -------------------------------------------------------------------------------------------------
// <copyright file="ObjectSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
{
    using System;
    using System.Globalization;
    using System.Text;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides methods for converting objects to serialized bytes.
    /// </summary>
    internal static class ObjectSerializer
    {
        private static readonly Func<string, byte[]> Encode = Encoding.UTF8.GetBytes;
        private static readonly byte[] None = Encode(nameof(None));

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(string value)
        {
            return Encode(value);
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(decimal value)
        {
            return Encode(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(Enum value)
        {
            return Encode(value.ToString());
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="type">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(Type type)
        {
            return Encode(type.Name);
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="guid">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(Guid guid)
        {
            return Encode(guid.ToString());
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="dateTime">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(ZonedDateTime dateTime)
        {
            return Encode(dateTime.ToIsoString());
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="identifier">The object to serialize.</param>
        /// <typeparam name="T">The identifier type.</typeparam>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize<T>(Identifier<T> identifier)
            where T : class
        {
            return Encode(identifier.Value);
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="quantity">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(Quantity quantity)
        {
            return Encode(quantity.ToString());
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="price">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(Price? price)
        {
            return price is null ? None : Encode(price.ToString());
        }

        /// <summary>
        /// Return the serialized object.
        /// </summary>
        /// <param name="money">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(Money money)
        {
            return Encode(money.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Returns a string to pack from the given <see cref="ZonedDateTime"/>?.
        /// </summary>
        /// <param name="dateTime">The nullable zoned date time.</param>
        /// <returns>The serialized object.</returns>
        internal static byte[] Serialize(ZonedDateTime? dateTime)
        {
            return dateTime is null ? None : Encode(dateTime.Value.ToIsoString());
        }
    }
}
