// -------------------------------------------------------------------------------------------------
// <copyright file="Handler.cs" company="Nautech Systems Pty Ltd">
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
    internal sealed class Handler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusMQ.Handler"/> class.
        /// </summary>
        /// <param name="handle">The delegate handle.</param>
        private Handler(Func<object, Task<bool>> handle)
        {
            this.Id = Guid.NewGuid();
            this.Handle = handle;
        }

        /// <summary>
        /// Gets the subscription identifier.
        /// </summary>
        internal Guid Id { get; }

        /// <summary>
        /// Gets the handler identifier.
        /// </summary>
        internal Func<object, Task<bool>> Handle { get; }

        /// <summary>
        /// Create the subscription with the given handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <typeparam name="TMessage">The handler message type.</typeparam>
        /// <returns>The subscription.</returns>
        public static Handler Create<TMessage>(Action<TMessage> handler)
        {
            return BuildHandler<TMessage>(
                message =>
                {
                    handler(message);
                    return Task.FromResult(true);
                });
        }

        private static Handler BuildHandler<TMessage>(Func<TMessage, Task<bool>> handlerAction)
        {
            async Task<bool> ActionWithCheck(object message)
            {
                if (message is TMessage typedMessage)
                {
                    await handlerAction(typedMessage);
                    return true;
                }

                return false;
            }

            return new Handler(ActionWithCheck);
        }
    }
}
