// -------------------------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a subscription.
    /// </summary>
    internal sealed class Subscription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        private Subscription(Func<object, Task> handler)
        {
            this.Id = Guid.NewGuid();
            this.Handler = handler;
        }

        /// <summary>
        /// Gets the subscription identifier.
        /// </summary>
        internal Guid Id { get; }

        /// <summary>
        /// Gets the handler identifier.
        /// </summary>
        internal Func<object, Task> Handler { get; }

        /// <summary>
        /// Create the subscription with the given handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <typeparam name="TMessage">The handler message type.</typeparam>
        /// <returns>The subscription.</returns>
        public static Subscription Create<TMessage>(Action<TMessage> handler)
        {
            return BuildSubscription<TMessage>(
                message =>
                {
                    handler(message);
                    return Task.FromResult(false);
                });
        }

        private static Subscription BuildSubscription<TMessage>(Func<TMessage, Task> handlerAction)
        {
            async Task ActionWithCheck(object message)
            {
                if (message is TMessage typedMessage)
                {
                    await handlerAction(typedMessage);
                }
            }

            return new Subscription(ActionWithCheck);
        }
    }
}
