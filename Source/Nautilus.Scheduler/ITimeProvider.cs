// -------------------------------------------------------------------------------------------------
// <copyright file="ITimeProvider.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;

    /// <summary>
    /// TBD.
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Gets the schedulers notion of current time.
        /// </summary>
        DateTimeOffset Now { get; }

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        TimeSpan ElapsedHighRes { get; }
    }
}
