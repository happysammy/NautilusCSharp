//--------------------------------------------------------------------------------------------------
// <copyright file="MockComponent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Mocks
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockComponent : Component
    {
        private readonly int workDelayMilliseconds;

        public MockComponent(
            IComponentryContainer container,
            string subName = "",
            int workDelayMilliseconds = 1000)
            : base(container, subName)
        {
            this.Messages = new List<object>();
            this.workDelayMilliseconds = workDelayMilliseconds;
        }

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

        public void OnMessageWithWorkDelay(object message)
        {
            this.Messages.Add(message);
            Task.Delay(this.workDelayMilliseconds).Wait();
        }
    }
}
