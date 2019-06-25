// -------------------------------------------------------------------------------------------------
// <copyright file="ByteArrayUtf8Serializer{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using System.Text;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a generic serializer for type T to an array of byte arrays.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    public static class ByteArrayUtf8Serializer<T>
    {
        /// <summary>
        /// Return the serialized array of byte arrays.
        /// </summary>
        /// <param name="objects">The objects to serialize.</param>
        /// <returns>The serialized array of byte arrays.</returns>
        public static byte[][] Serialize(T[] objects)
        {
            var serializedLength = objects.Length;
            var serialized = new byte[serializedLength][];
            for (var i = 0; i < serializedLength; i++)
            {
                serialized[i] = Encoding.UTF8.GetBytes(objects[i]?.ToString());
            }

            return serialized;
        }

        /// <summary>
        /// Return the deserialized array of type T.
        /// </summary>
        /// <param name="objectsBytes">The objects bytes to deserialize.</param>
        /// <param name="deserializer">The deserialization function.</param>
        /// <returns>The deserialized objects array.</returns>
        public static T[] Deserialize(byte[][] objectsBytes, Func<string, T> deserializer)
        {
            var deserializedLength = objectsBytes.Length;
            var deserialized = new T[deserializedLength];
            for (var i = 0; i < deserializedLength; i++)
            {
                deserialized[i] = deserializer(Encoding.UTF8.GetString(objectsBytes[i]));
            }

            return deserialized;
        }

        /// <summary>
        /// Return the deserialized array of type T.
        /// </summary>
        /// <param name="objectsBytes">The objects bytes to deserialize.</param>
        /// <param name="symbol">The symbol argument for the deserialized object.</param>
        /// <param name="deserializer">The deserialization function.</param>
        /// <returns>The deserialized objects array.</returns>
        public static T[] Deserialize(byte[][] objectsBytes, Symbol symbol, Func<Symbol, string, T> deserializer)
        {
            var deserializedLength = objectsBytes.Length;
            var deserialized = new T[deserializedLength];
            for (var i = 0; i < deserializedLength; i++)
            {
                deserialized[i] = deserializer(symbol, Encoding.UTF8.GetString(objectsBytes[i]));
            }

            return deserialized;
        }
    }
}
