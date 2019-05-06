// -------------------------------------------------------------------------------------------------
// <copyright file="Port.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a valid network port number.
    /// </summary>
    [Immutable]
    public sealed class Port
    {
        private readonly string valueString;

        /// <summary>
        /// Initializes a new instance of the <see cref="Port"/> class.
        /// </summary>
        /// <param name="portNumber">The port number.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the portNumber is out of range [0, 65535].</exception>
        public Port(int portNumber)
        {
            Precondition.NotOutOfRangeInt32(portNumber, 0, 65535, nameof(portNumber));

            this.Value = portNumber;
            this.valueString = portNumber.ToString();
        }

        /// <summary>
        /// Gets the port numbers value.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="Port"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.valueString;
    }
}
