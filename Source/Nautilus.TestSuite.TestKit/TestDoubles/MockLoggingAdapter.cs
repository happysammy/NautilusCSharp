//--------------------------------------------------------------------------------------------------
// <copyright file="MockLoggingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Akka.Util.Internal;
    using Nautilus.Common.Interfaces;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockLoggingAdapter : ILoggingAdapter
    {
        private readonly ConcurrentQueue<string> stash = new ConcurrentQueue<string>();

        public string AssemblyVersion => "1.0.0";

        public void WriteStashToOutput(ITestOutputHelper output)
        {
            this.GetLogStashTextAsStringList().ForEach(output.WriteLine);
        }

        public bool Contains(string text)
        {
            return this.GetLogStashTextAsStringList().Any(logEntry => logEntry.StartsWith(text));
        }

        public void Verbose(Enum service, string message)
        {
            this.stash.Enqueue(message);
        }

        public void Debug(Enum service, string message)
        {
            this.stash.Enqueue(message);
        }

        public void Information(Enum service, string message)
        {
            this.stash.Enqueue(message);
        }

        public void Warning(Enum service, string message)
        {
            this.stash.Enqueue(message);
        }

        public void Error(Enum service, string message, Exception ex)
        {
            this.stash.Enqueue(message);
        }

        public void Fatal(Enum service, string message, Exception ex)
        {
            this.stash.Enqueue(message);
        }

        private IEnumerable<string> GetLogStashTextAsStringList()
        {
            return this.stash.ToList().AsReadOnly();
        }
    }
}
