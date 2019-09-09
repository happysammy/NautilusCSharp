// -------------------------------------------------------------------------------------------------
// <copyright file="NetworkHost.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a valid network host address.
    /// </summary>
    [Immutable]
    public sealed class NetworkHost
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkHost"/> class.
        /// </summary>
        /// <param name="address">The network address.</param>
        /// <exception cref="ArgumentException">If the address is empty or white space.</exception>
        public NetworkHost(string address)
        {
            Condition.NotEmptyOrWhiteSpace(address, nameof(address));

            this.value = address;
        }

        /// <summary>
        /// Gets the local host 127.0.0.1 network address.
        /// </summary>
        /// <returns>The local host network address.</returns>
        public static NetworkHost LocalHost { get; } = new NetworkHost("127.0.0.1");

        /// <summary>
        /// Returns a string representation of this <see cref="NetworkHost"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.value;
    }
}
