//--------------------------------------------------------------------------------------------------
// <copyright file="IDataSerializer{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Nautilus.Common.Enums;

namespace Nautilus.Common.Interfaces
{
    /// <summary>
    /// Provides a binary serializer for data objects of type T.
    /// </summary>
    /// <typeparam name="T">The serializable data type.</typeparam>
    public interface IDataSerializer<T> : ISerializer<T>
    {
        /// <summary>
        /// Gets the serializers data blob encoding.
        /// </summary>
        DataEncoding BlobEncoding { get; }

        /// <summary>
        /// Gets the serializers individual data object encoding.
        /// </summary>
        DataEncoding ObjectEncoding { get; }

        /// <summary>
        /// Returns the serialized data object two dimensional bytes array.
        /// </summary>
        /// <param name="dataObjects">The data objects to serialize.</param>
        /// <returns>The serialized data object byte arrays array.</returns>
        byte[][] Serialize(T[] dataObjects);

        /// <summary>
        /// Return the data objects byte arrays serialized to a single array.
        /// </summary>
        /// <param name="dataObjectsArray">The data objects array to serialize.</param>
        /// <param name="metadata">The metadata for the given data objects array.</param>
        /// <returns>The serialized data bytes.</returns>
        byte[] SerializeBlob(byte[][] dataObjectsArray, Dictionary<string, string>? metadata);

        /// <summary>
        /// Returns the deserialize data object of type T array.
        /// </summary>
        /// <param name="dataBytesArray">The bytes array to deserialize.</param>
        /// <param name="metadata">The optional metadata for deserializing.</param>
        /// <returns>The deserialized data object.</returns>
        T[] Deserialize(byte[][] dataBytesArray, Dictionary<string, string>? metadata);

        /// <summary>
        /// Return the data objects bytes array deserialized to a two dimensional array.
        /// </summary>
        /// <param name="dataBytes">The data bytes to deserialize.</param>
        /// <returns>The deserialized data byte arrays.</returns>
        T[] DeserializeBlob(byte[] dataBytes);
    }
}
