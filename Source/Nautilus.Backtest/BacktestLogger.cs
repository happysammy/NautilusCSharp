// -------------------------------------------------------------------------------------------------
// <copyright file="BacktestLogger.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Backtest
{
    using System;
    using System.Threading;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Logging;
    using Nautilus.DomainModel.Extensions;

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

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="service">
        /// The context.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Verbose(BlackBoxService service, string message)
        {
            Console.WriteLine($"{this.clock.TimeNow().ToStringFormattedIsoUtc()} [{Thread.CurrentThread.ManagedThreadId:000}][VRB] [{LogFormatter.ToOutput(service)}] {message}");
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
        public void Debug(BlackBoxService service, string message)
        {
            Console.WriteLine($"{this.clock.TimeNow().ToStringFormattedIsoUtc()} [{Thread.CurrentThread.ManagedThreadId:000}][DBG] [{LogFormatter.ToOutput(service)}] {message}");
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
        public void Information(BlackBoxService service, string message)
        {
            Console.WriteLine($"{this.clock.TimeNow().ToStringFormattedIsoUtc()} [{Thread.CurrentThread.ManagedThreadId:000}][INF] [{LogFormatter.ToOutput(service)}] {message}");
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
        public void Warning(BlackBoxService service, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{this.clock.TimeNow().ToStringFormattedIsoUtc()} [{Thread.CurrentThread.ManagedThreadId:000}][WRN] [{LogFormatter.ToOutput(service)}] {message}");
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
        public void Error(BlackBoxService service, string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{this.clock.TimeNow().ToStringFormattedIsoUtc()} [{Thread.CurrentThread.ManagedThreadId:000}][ERR] [{LogFormatter.ToOutput(service)}] {message}");
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
        public void Fatal(BlackBoxService service, string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{this.clock.TimeNow().ToStringFormattedIsoUtc()} [{Thread.CurrentThread.ManagedThreadId:000}][FTL] [{LogFormatter.ToOutput(service)}] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
