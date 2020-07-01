// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackDictionarySerializer.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MessagePack;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Correctness;
using Nautilus.Core.Extensions;

namespace Nautilus.Serialization.MessageSerializers
{
    /// <summary>
    /// Provides a serializer for dictionaries with string keys and values.
    /// </summary>
    public sealed class MsgPackDictionarySerializer : ISerializer<Dictionary<string, string>>
    {
        /// <inheritdoc />
        public byte[] Serialize(Dictionary<string, string> dict)
        {
            Debug.NotEmpty(dict, nameof(dict));

            // TODO: TEMP
            Console.WriteLine($"Serializing {dict.Print()}");

            var package = new Dictionary<string, string>();
            foreach (var (key, value) in dict)
            {
                package.Add(key, value);
            }

            return MessagePackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Dictionary<string, string> Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, string>>(dataBytes);

            var dict = new Dictionary<string, string>();
            foreach (var (key, value) in unpacked)
            {
                dict.Add(key, value);
            }

            return dict;
        }
    }
}
