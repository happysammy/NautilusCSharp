//--------------------------------------------------------------------------------------------------
// <copyright file="EventuallyContains.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Extensions
{
    /// <summary>
    /// Provides eventually contains assert constants.
    /// </summary>
    public static class EventuallyContains
    {
        /// <summary>
        /// Gets the timeout milliseconds.
        /// </summary>
        public static int TimeoutMilliseconds => 500;

        /// <summary>
        /// Gets the poll interval milliseconds.
        /// </summary>
        public static int PollIntervalMilliseconds => 20;
    }
}
