// -------------------------------------------------------------------------------------------------
// <copyright file="BarProviderTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.NetworkTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Network;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class BarProviderTests
    {
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly BarProvider provider;

        public BarProviderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.loggingAdapter = setupFactory.LoggingAdapter;
        }
    }
}
