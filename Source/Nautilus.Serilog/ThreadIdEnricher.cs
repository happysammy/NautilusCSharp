//--------------------------------------------------------------
// <copyright file="ThreadIdEnricher.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Serilog
{
    using System.Threading;
    using global::Serilog.Core;
    using global::Serilog.Events;

    /// <summary>
    /// The thread id enricher.
    /// </summary>
    public class ThreadIdEnricher : ILogEventEnricher
    {
        /// <summary>
        /// The enrich.
        /// </summary>
        /// <param name="logEvent">
        /// The log event.
        /// </param>
        /// <param name="propertyFactory">
        /// The property factory.
        /// </param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
    }
}