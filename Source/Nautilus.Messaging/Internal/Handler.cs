// -------------------------------------------------------------------------------------------------
// <copyright file="Handler.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Threading.Tasks;
using Nautilus.Core.Annotations;

namespace Nautilus.Messaging.Internal
{
    /// <summary>
    /// Provides a handler for a specified type of message.
    /// </summary>
    [Immutable]
    internal sealed class Handler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Handler"/> class.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="handle">The delegate handle.</param>
        private Handler(Type type, Func<object, Task<bool>> handle)
        {
            this.Type = type;
            this.Handle = handle;
        }

        /// <summary>
        /// Gets the handlers message type.
        /// </summary>
        internal Type Type { get; }

        /// <summary>
        /// Gets the handlers delegate.
        /// </summary>
        internal Func<object, Task<bool>> Handle { get; }

        /// <summary>
        /// Creates a new handler from the given delegate.
        /// </summary>
        /// <param name="handle">The delegate handle.</param>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The created handler.</returns>
        internal static Handler Create<TMessage>(Action<TMessage> handle)
        {
            Task<bool> ActionDelegate(object message)
            {
                if (message is TMessage typedMessage)
                {
                    handle.Invoke(typedMessage);
                    return Task.FromResult(true);
                }

                // The given message was not of the handlers type
                return Task.FromResult(false);
            }

            return new Handler(typeof(TMessage), ActionDelegate);
        }
    }
}
