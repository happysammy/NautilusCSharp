//--------------------------------------------------------------------------------------------------
// <copyright file="Address.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Messaging
{
    using System.Text;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Types;

    /// <summary>
    /// Represents a components messaging address within a service.
    /// </summary>
    [Immutable]
    public sealed class Address : Identifier<Address>
    {
        private static readonly Address NoneAddress = new Address("None");

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="value">The value of the address.</param>
        public Address(string value)
            : base(value)
        {
            this.Utf8Bytes = Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="value">The value of the address.</param>
        public Address(byte[] value)
            : base(Encoding.UTF8.GetString(value))
        {
            this.Utf8Bytes = value;
        }

        /// <summary>
        /// Gets the address represented as UTF-8 bytes.
        /// </summary>
        public byte[] Utf8Bytes { get; }

        /// <summary>
        /// Gets a value indicating whether the address is none.
        /// </summary>
        public bool IsNone => this.Equals(NoneAddress);

        /// <summary>
        /// Returns a null address.
        /// </summary>
        /// <returns>The null address.</returns>
        public static Address None()
        {
            return NoneAddress;
        }
    }
}
