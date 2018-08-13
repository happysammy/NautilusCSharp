//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessagingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Collections;

    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockMessagingAdapter : IMessagingAdapter
    {
        private readonly IActorRef testActorRef;

        public MockMessagingAdapter(IActorRef testActorRef)
        {
            this.testActorRef = testActorRef;
        }

        public void Send<T>(NautilusService receiver, T message, NautilusService sender)
            where T : Message
        {
            this.testActorRef.Tell(message);
        }

        public void Send<T>(ReadOnlyList<NautilusService> receivers, T message, NautilusService sender)
            where T : Message
        {
            this.testActorRef.Tell(message);
        }
    }
}
