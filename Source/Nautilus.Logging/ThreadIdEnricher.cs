//--------------------------------------------------------------------------------------------------
// <copyright file="ThreadIdEnricher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Logging
{
    using System.Threading;
    using Serilog.Core;
    using Serilog.Events;

    /// <summary>
    /// The Serilog thread identifier enricher.
    /// </summary>
    public sealed class ThreadIdEnricher : ILogEventEnricher
    {
        /// <summary>
        /// Enriches log events with the thread identifier.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="propertyFactory">The property factory.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
    }
}
