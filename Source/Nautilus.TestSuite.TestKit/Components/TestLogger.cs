//--------------------------------------------------------------------------------------------------
// <copyright file="TestLogger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Components
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TestLogger : ILogger
    {
        private readonly ConcurrentQueue<string> log = new ConcurrentQueue<string>();
        private readonly ITestOutputHelper output;
        private readonly string contextName;

        public TestLogger(ITestOutputHelper output, string contextName)
        {
            this.output = output;
            this.contextName = contextName;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = string.Empty;

            if (formatter != null)
            {
                message += formatter(state, exception);
            }

            this.output.WriteLine($"[{FormatLogLevel(logLevel)}] [{this.contextName}] [{eventId.Id}] - {message}");

            // this.log.Enqueue($"[{logLevel.ToString()}] [{this.contextName}] [{eventId.Id}] - {message}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        private static string FormatLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "VRB";
                case LogLevel.Debug:
                    return "DBG";
                case LogLevel.Information:
                    return "INF";
                case LogLevel.Warning:
                    return "WRN";
                case LogLevel.Error:
                    return "ERR";
                case LogLevel.Critical:
                    return "CRT";
                case LogLevel.None:
                    goto default;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private sealed class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
