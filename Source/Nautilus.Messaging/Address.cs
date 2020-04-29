//--------------------------------------------------------------------------------------------------
// <copyright file="Address.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
