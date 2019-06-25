//--------------------------------------------------------------------------------------------------
// <copyright file="MockLoggingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class MockLoggingAdapter : ILoggingAdapter
    {
        private readonly ConcurrentQueue<string> log = new ConcurrentQueue<string>();

        public string AssemblyVersion => "1.0.0";

        public void WriteStashToOutput(ITestOutputHelper output)
        {
            foreach (var message in this.log.ToList())
            {
                output.WriteLine(message);
            }
        }

        public void Verbose(string message)
        {
            this.log.Enqueue("[VRB] " + message);
        }

        public void Debug(string message)
        {
            this.log.Enqueue("[DBG] " + message);
        }

        public void Information(string message)
        {
            this.log.Enqueue("[INF] " + message);
        }

        public void Warning(string message)
        {
            this.log.Enqueue("[WRN] " + message);
        }

        public void Error(string message)
        {
            this.log.Enqueue("[ERR] " + message);
        }

        public void Error(string message, Exception ex)
        {
            this.log.Enqueue("[ERR] " + message + Environment.NewLine + ex.Message);
        }

        public void Fatal(string message, Exception ex)
        {
            this.log.Enqueue("[FTL] " + message + Environment.NewLine + ex.Message);
        }
    }
}
