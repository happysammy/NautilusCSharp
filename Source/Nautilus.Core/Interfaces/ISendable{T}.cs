//--------------------------------------------------------------------------------------------------
// <copyright file="ISendable{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Interfaces
{
    using System;
    using NodaTime;

    /// <summary>
    /// Represents an object which is sendable through the messaging system.
    /// </summary>
    /// <typeparam name="T">The sendable object type.</typeparam>
    public interface ISendable<T> : IEquatable<T>
        where T : class
    {
        /// <summary>
        /// Gets the sendable objects identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the sendable objects timestamp.
        /// </summary>
        ZonedDateTime Timestamp { get; }
    }
}
