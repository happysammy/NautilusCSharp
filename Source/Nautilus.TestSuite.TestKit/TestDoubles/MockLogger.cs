//--------------------------------------------------------------
// <copyright file="MockLogger.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp.Extensions;
    using Xunit.Abstractions;

    /// <summary>
    /// The mock system logger.
    /// </summary>
    public class MockLogger : ILoggingAdapter
    {
        private readonly ConcurrentQueue<string> stash = new ConcurrentQueue<string>();

        /// <summary>
        /// The write stash to output.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        public void WriteStashToOutput(ITestOutputHelper output)
        {
            this.GetLogStashTextAsStringList().ForEach(output.WriteLine);
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(string text)
        {
            return this.GetLogStashTextAsStringList().Any(logEntry => logEntry.StartsWith(text));
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
            this.stash.Enqueue(message);
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
            this.stash.Enqueue(message);
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
            this.stash.Enqueue(message);
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
            this.stash.Enqueue(message);
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
            this.stash.Enqueue(message);
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
            this.stash.Enqueue(message);
        }

        private IReadOnlyList<string> GetLogStashTextAsStringList()
        {
            return this.stash.ToList().AsReadOnly();
        }
    }
}