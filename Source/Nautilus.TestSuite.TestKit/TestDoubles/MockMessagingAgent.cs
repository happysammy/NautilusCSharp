//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessagingAgent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockMessagingAgent : MessagingAgent
    {
        private readonly int workDelayMilliseconds;

        public MockMessagingAgent(
            string name = nameof(MockMessagingAgent),
            int workDelayMilliseconds = 1000)
        {
            this.Mailbox = new Mailbox(new Address(name), this.Endpoint);
            this.Messages = new List<object>();
            this.workDelayMilliseconds = workDelayMilliseconds;
        }

        public Mailbox Mailbox { get; }

        public List<object> Messages { get; }

        public void OnMessage(object message)
        {
            this.Messages.Add(message);
        }

        public void OnMessage(Tick message)
        {
            this.Messages.Add(message);
        }

        public void OnMessage(BarData message)
        {
            this.Messages.Add(message);
        }

        public void OnMessage(int message)
        {
            this.Messages.Add(message);
        }

        public void OnMessage(byte[] message)
        {
            this.Messages.Add(Encoding.UTF8.GetString(message));
        }

        public void OnMessageWithWorkDelay(object message)
        {
            this.Messages.Add(message);
            Task.Delay(this.workDelayMilliseconds).Wait();
        }
    }
}
