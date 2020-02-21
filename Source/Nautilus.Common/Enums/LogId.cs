//--------------------------------------------------------------------------------------------------
// <copyright file="LogId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a <see cref="Nautilus"/> specific log event identifier.
    /// </summary>
    public enum LogId
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The event identifier groups component operation events.
        /// </summary>
        Operation = 1,

        /// <summary>
        /// The event identifier groups input output events.
        /// </summary>
        // ReSharper disable once InconsistentNaming (correct name)
        IO = 2,

        /// <summary>
        /// The event identifier groups database events.
        /// </summary>
        Database = 3,

        /// <summary>
        /// The event identifier groups trading events.
        /// </summary>
        Trading = 4,
    }
}
