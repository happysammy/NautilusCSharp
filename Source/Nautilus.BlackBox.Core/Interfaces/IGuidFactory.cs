// -------------------------------------------------------------------------------------------------
// <copyright file="IGuidFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System;

    /// <summary>
    /// The <see cref="IGuidFactory"/> interface. Provides <see cref="Guid"/>(s) for the
    /// <see cref="BlackBox"/> system.
    /// </summary>
    public interface IGuidFactory
    {
        /// <summary>
        /// Returns a new <see cref="Guid"/>.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        Guid NewGuid();
    }
}