//--------------------------------------------------------------------------------------------------
// <copyright file="Address.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Primitives;

    /// <summary>
    /// Represents a components messaging address within the system.
    /// </summary>
    [Immutable]
    public sealed class Address : ValidString<Address>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="value">The value of the address.</param>
        public Address(string value)
            : base(value)
        {
        }
    }
}
