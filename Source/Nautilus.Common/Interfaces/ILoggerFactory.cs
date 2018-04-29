//--------------------------------------------------------------------------------------------------
// <copyright file="ILoggerFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="ILoggerFactory"/> interface.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="ILogger"/> from the given inputs.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <param name="component">The component label.</param>
        /// <returns>A <see cref="ILogger"/>.</returns>
        ILogger Create(Enum service, Label component);
    }
}
