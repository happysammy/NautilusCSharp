//--------------------------------------------------------------------------------------------------
// <copyright file="Address.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a components messaging address within the service.
    /// </summary>
    [Immutable]
    public struct Address : IEquatable<object>, IEquatable<Address>
    {
        private static readonly byte[] Empty = { };

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> structure.
        /// </summary>
        /// <param name="value">The value of the address.</param>
        /// <param name="decoder">The decoder for the address string.</param>
        public Address(byte[] value, Func<byte[], string> decoder)
        {
            Debug.NotEmpty(value, nameof(value));

            this.BytesValue = value;
            this.StringValue = decoder(value).Replace(Environment.NewLine, string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> structure.
        /// </summary>
        /// <param name="value">The value of the address.</param>
        public Address(string value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));

            this.StringValue = value;
            this.BytesValue = Empty;
        }

        /// <summary>
        /// Gets addresses byte[] value (can be an empty byte[]).
        /// </summary>
        public byte[] BytesValue { get; }

        /// <summary>
        /// Gets addresses string value.
        /// </summary>
        public string StringValue { get; }

        /// <summary>
        /// Gets a value indicating whether the address has a bytes represented value.
        /// </summary>
        public bool HasBytesValue => this.BytesValue != Empty;

        /// <summary>
        /// Returns a value indicating whether the <see cref="Address"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Address left, Address right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Address"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Address left,  Address right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Address"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object? other) => other is Address label && this.Equals(label);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Address"/> is equal
        /// to the given <see cref="Address"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Address other)
        {
            return this.StringValue == other.StringValue;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="Address"/>.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.StringValue);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Address"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.StringValue;
    }
}
