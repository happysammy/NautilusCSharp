// -------------------------------------------------------------------------------------------------
// <copyright file="Handler.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Internal
{
    using System;
    using System.Threading.Tasks;
    using Nautilus.Core.Annotations;

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
