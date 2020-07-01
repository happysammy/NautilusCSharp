// -------------------------------------------------------------------------------------------------
// <copyright file="Port.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid network port number. Should be between 49152 to 65535 to avoid registered
    /// IANA port collision.
    /// </summary>
    [Immutable]
    public sealed class Port
    {
        private readonly string valueString;

        /// <summary>
        /// Initializes a new instance of the <see cref="Port"/> class.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the portNumber is out of range [49152, 65535].</exception>
        public Port(int port)
        {
            Condition.NotOutOfRangeInt32(port, 49152, 65535, nameof(port));

            this.Value = port;
            this.valueString = port.ToString();
        }

        /// <summary>
        /// Gets the port number.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="Port"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.valueString;
    }
}
