// -------------------------------------------------------------------------------------------------
// <copyright file="NetworkAddress.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;

namespace Nautilus.Network
{
    /// <summary>
    /// Represents a valid network address.
    /// </summary>
    [Immutable]
    public sealed class NetworkAddress
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkAddress"/> class.
        /// </summary>
        /// <param name="address">The network address.</param>
        /// <exception cref="ArgumentException">If the address is empty or white space.</exception>
        public NetworkAddress(string address)
        {
            Condition.NotEmptyOrWhiteSpace(address, nameof(address));

            this.value = address;
        }

        /// <summary>
        /// Gets the local host 127.0.0.1 network address.
        /// </summary>
        /// <returns>The local host network address.</returns>
        public static NetworkAddress LocalHost { get; } = new NetworkAddress("127.0.0.1");

        /// <summary>
        /// Returns a string representation of this <see cref="NetworkAddress"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.value;
    }
}
