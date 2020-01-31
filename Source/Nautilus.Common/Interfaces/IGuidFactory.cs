//--------------------------------------------------------------------------------------------------
// <copyright file="IGuidFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System;

    /// <summary>
    /// Provides <see cref="Guid"/>(s) for the service.
    /// </summary>
    public interface IGuidFactory
    {
        /// <summary>
        /// Generates and returns a new <see cref="Guid"/>.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        Guid Generate();
    }
}
