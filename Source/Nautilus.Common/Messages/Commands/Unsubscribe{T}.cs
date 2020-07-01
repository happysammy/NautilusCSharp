//--------------------------------------------------------------------------------------------------
// <copyright file="Unsubscribe{T}.cs" company="Nautech Systems Pty Ltd">
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

using System;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Message;
using Nautilus.Messaging;
using NodaTime;

namespace Nautilus.Common.Messages.Commands
{
    /// <summary>
    /// Represents a command to unsubscribe a component from type T.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class Unsubscribe<T> : Command
    {
        private static readonly Type EventType = typeof(Unsubscribe<T>);

        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscribe{T}"/> class.
        /// </summary>
        /// <param name="subscription">The subscription type.</param>
        /// <param name="subscriber">The subscriber endpoint.</param>
        /// <param name="id">The commands identifier.</param>
        /// <param name="timestamp">The commands timestamp.</param>
        public Unsubscribe(
            T subscription,
            Mailbox subscriber,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                EventType,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Subscription = subscription;
            this.Subscriber = subscriber;
        }

        /// <summary>
        /// Gets the commands type to unsubscribe from.
        /// </summary>
        public T Subscription { get; }

        /// <summary>
        /// Gets the commands subscriber mailbox.
        /// </summary>
        public Mailbox Subscriber { get; }
    }
}
