//--------------------------------------------------------------------------------------------------
// <copyright file="MockComponent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockComponent : MessagingComponent
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

        /// <summary>
        /// Register the message type with the given handler.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The handler to register.</param>
        public new void RegisterHandler<TMessage>(Action<TMessage> handler)
        {
            base.RegisterHandler(handler);
        }

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
