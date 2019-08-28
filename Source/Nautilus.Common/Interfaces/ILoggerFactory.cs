//--------------------------------------------------------------------------------------------------
// <copyright file="ILoggerFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides a factory for creating <see cref="ILogger"/>s.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="ILogger"/> from the given inputs.
        /// </summary>
        /// <param name="component">The component label.</param>
        /// <returns>A <see cref="ILogger"/>.</returns>
        ILogger Create(Label component);
    }
}
