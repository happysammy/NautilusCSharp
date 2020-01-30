//--------------------------------------------------------------------------------------------------
// <copyright file="NewsImpact.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The relative historic impact on a financial market of a particular news event type.
    /// </summary>
    public enum NewsImpact
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The low news impact.
        /// </summary>
        Low = 1,

        /// <summary>
        /// The medium news impact.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// The high news impact.
        /// </summary>
        High = 3,
    }
}
