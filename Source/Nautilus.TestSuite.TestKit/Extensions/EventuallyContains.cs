// -------------------------------------------------------------------------------------------------
// <copyright file="EventuallyContains.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Extensions
{
    /// <summary>
    /// The eventually contains.
    /// </summary>
    public static class EventuallyContains
    {
        /// <summary>
        /// The timeout milliseconds.
        /// </summary>
        public static int TimeoutMilliseconds => 500;

        /// <summary>
        /// The poll interval milliseconds.
        /// </summary>
        public static int PollIntervalMilliseconds => 20;
    }
}