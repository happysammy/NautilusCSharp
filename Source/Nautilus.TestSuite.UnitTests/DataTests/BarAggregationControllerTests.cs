//--------------------------------------------------------------------------------------------------
// <copyright file="TimeBarAggregatorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Enums;
    using Nautilus.Data;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarAggregationControllerTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly IActorRef controllerRef;

        public BarAggregationControllerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var container = setupFactory.Create();

            this.logger = setupFactory.LoggingAdapter;

            var testActorSystem = ActorSystem.Create(nameof(BarAggregationControllerTests));
            var messagingAdapter = new MockMessagingAdapter(TestActor);

            var props = Props.Create(() => new BarAggregationController(
                container,
                messagingAdapter,
                testActorSystem.Scheduler,
                new List<Enum>{ServiceContext.Database}.ToImmutableList(),
                ServiceContext.Database));

            this.controllerRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);
        }
    }
}
