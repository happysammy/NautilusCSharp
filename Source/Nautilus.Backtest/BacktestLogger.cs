//--------------------------------------------------------------------------------------------------
// <copyright file="BacktestLogger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Backtest
{
    using System;
    using System.Threading;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;

    /// <summary>
    /// The mock system logger.
    /// </summary>
    public class BacktestLogger : ILoggingAdapter
    {
        private readonly IZonedClock clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="BacktestLogger"/> class.
        /// </summary>
        /// <param name="clock">
        /// The clock.
        /// </param>
        public BacktestLogger(IZonedClock clock)
        {
            Validate.NotNull(clock, nameof(clock));

            this.clock = clock;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string AssemblyVersion => "1.0.0";

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="service">
        /// The context.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Verbose(Enum service, string message)
        {
            Console.WriteLine($"{this.clock.TimeNow().ToIsoString()} [{Thread.CurrentThread.ManagedThreadId:000}][VRB] [{LogFormatter.ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="service">
        /// The context.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Debug(Enum service, string message)
        {
            Console.WriteLine($"{this.clock.TimeNow().ToIsoString()} [{Thread.CurrentThread.ManagedThreadId:000}][DBG] [{LogFormatter.ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="service">
        /// The context.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Information(Enum service, string message)
        {
            Console.WriteLine($"{this.clock.TimeNow().ToIsoString()} [{Thread.CurrentThread.ManagedThreadId:000}][INF] [{LogFormatter.ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="service">
        /// The context.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Warning(Enum service, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{this.clock.TimeNow().ToIsoString()} [{Thread.CurrentThread.ManagedThreadId:000}][WRN] [{LogFormatter.ToOutput(service)}] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="service">
        /// The context.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public void Error(Enum service, string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{this.clock.TimeNow().ToIsoString()} [{Thread.CurrentThread.ManagedThreadId:000}][ERR] [{LogFormatter.ToOutput(service)}] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="service">
        /// The context.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public void Fatal(Enum service, string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{this.clock.TimeNow().ToIsoString()} [{Thread.CurrentThread.ManagedThreadId:000}][FTL] [{LogFormatter.ToOutput(service)}] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LogResult(ResultBase result)
        {
            throw new NotImplementedException();
        }
    }
}
