// -------------------------------------------------------------------------------------------------
// <copyright file="NetworkPort.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid network port number.
    /// Should be between 49152 to 65535 to avoid registered IANA port collision.
    /// </summary>
    [Immutable]
    public sealed class NetworkPort
    {
        private readonly string valueString;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPort"/> class.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the portNumber is out of range [0, 65535].</exception>
        public NetworkPort(ushort port)
        {
            Condition.NotOutOfRangeInt32(port, 0, 65535, nameof(port));

            this.Value = port;
            this.valueString = port.ToString();
        }

        /// <summary>
        /// Gets the port number.
        /// </summary>
        public ushort Value { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="NetworkPort"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.valueString;
    }
}
