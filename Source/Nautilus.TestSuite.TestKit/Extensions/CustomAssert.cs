//--------------------------------------------------------------------------------------------------
// <copyright file="CustomAssert.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    /// <summary>
    /// Provides custom asserts for testing purposes.
    /// </summary>
    public static class CustomAssert
    {
        /// <summary>
        /// The eventually contains.
        /// </summary>
        /// <param name="stringToContain">The string to contain.</param>
        /// <param name="loggingAdapter">The logger.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <param name="pollIntervalMilliseconds">The poll interval milliseconds.</param>
        public static void EventuallyContains(
            string stringToContain,
            MockLoggingAdapter loggingAdapter,
            int timeoutMilliseconds,
            int pollIntervalMilliseconds)
        {
            ValidateParameters(timeoutMilliseconds, pollIntervalMilliseconds);

            var stopwatch = Stopwatch.StartNew();

            do
            {
                Task.Delay(pollIntervalMilliseconds).Wait();
            }
            while (!loggingAdapter.Contains(stringToContain) && stopwatch.Elapsed.Duration().Milliseconds < (int)Period.FromMilliseconds(timeoutMilliseconds).Milliseconds);

            Assert.True(loggingAdapter.Contains(stringToContain));
        }

        /// <summary>
        /// The eventually contains.
        /// </summary>
        /// <param name="typeToContain">The type to contain.</param>
        /// <param name="envelopeList">The envelope list.</param>
        /// <param name="timeoutMilliseconds">The timeout milliseconds.</param>
        /// <param name="pollIntervalMilliseconds">The poll interval milliseconds.</param>
        /// <typeparam name="T">The message type to contain.</typeparam>
        public static void EventuallyContains<T>(
            Type typeToContain,
            IReadOnlyList<Envelope<T>> envelopeList,
            int timeoutMilliseconds,
            int pollIntervalMilliseconds)
            where T : Message
        {
            ValidateParameters(timeoutMilliseconds, pollIntervalMilliseconds);

            var stopwatch = Stopwatch.StartNew();

            do
            {
                Task.Delay(pollIntervalMilliseconds).Wait();
            }
            while (!ListContains(envelopeList.ToList(), typeToContain) && stopwatch.Elapsed < TimeSpan.FromMilliseconds(timeoutMilliseconds));

            Assert.True(ListContains(envelopeList.ToList(), typeToContain));
        }

        private static bool ListContains<T>(IEnumerable<Envelope<T>> envelopeList, Type typeToContain)
            where T : Message
        {
            return envelopeList.Any(e => e.Open(StubZonedDateTime.UnixEpoch()).GetType() == typeToContain);
        }

        private static void ValidateParameters(int timeoutMilliseconds, int pollIntervalMilliseconds)
        {
            if (timeoutMilliseconds <= 0)
            {
                throw new ArgumentException("timeoutMilliseconds must be > 0 milliseconds");
            }

            if (pollIntervalMilliseconds < 0)
            {
                throw new ArgumentException("pollIntervalMilliseconds must be >= 0 milliseconds");
            }
        }
    }
}
