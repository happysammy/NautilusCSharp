// -------------------------------------------------------------------------------------------------
// <copyright file="ScheduledSend.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduling.Internal
{
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// INTERNAL API.
    /// </summary>
    internal sealed class ScheduledSend : IRunnable
    {
        private readonly IEndpoint receiver;
        private readonly object message;
        private readonly IEndpoint sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledSend"/> class.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="sender">The message sender.</param>
        internal ScheduledSend(IEndpoint receiver, object message, IEndpoint sender)
        {
            this.receiver = receiver;
            this.message = message;
            this.sender = sender;
        }

        /// <inheritdoc/>
        public void Run()
        {
            this.receiver.Send(this.message);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{this.receiver}.Send({this.message}, {this.sender})]";
        }
    }
}
