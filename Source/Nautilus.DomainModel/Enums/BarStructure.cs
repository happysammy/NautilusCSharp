//--------------------------------------------------------------------------------------------------
// <copyright file="BarStructure.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents the granularity of a time resolution.
    /// </summary>
    public enum BarStructure
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The bar structure is based on ticks.
        /// </summary>
        Tick = 1,

        /// <summary>
        /// The bar structure is based on tick imbalance.
        /// </summary>
        TickImbalance = 2,

        /// <summary>
        /// The bar structure is based on volume.
        /// </summary>
        Volume = 3,

        /// <summary>
        /// The bar structure is based on volume imbalance.
        /// </summary>
        VolumeImbalance = 4,

        /// <summary>
        /// The bar structure is based on dollars.
        /// </summary>
        Dollar = 5,

        /// <summary>
        /// The bar structure is based on dollar imbalance.
        /// </summary>
        DollarImbalance = 6,

        /// <summary>
        /// The bar structure is based on seconds.
        /// </summary>
        Second = 7,

        /// <summary>
        /// The bar structure is based on minutes.
        /// </summary>
        Minute = 8,

        /// <summary>
        /// The bar structure is based on hours.
        /// </summary>
        Hour = 9,

        /// <summary>
        /// The bar structure is based on days.
        /// </summary>
        Day = 10,
    }
}
