// -------------------------------------------------------------------------------------------------
// <copyright file="Handler.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Internal
{
    using System;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides a handler for a type of message.
    /// </summary>
    [Immutable]
    internal sealed class Handler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Handler"/> class.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="handle">The delegate handle.</param>
        private Handler(Type type, Func<object, bool> handle)
        {
            this.Type = type;
            this.Handle = handle;
        }

        /// <summary>
        /// Gets the handlers type.
        /// </summary>
        internal Type Type { get; }

        /// <summary>
        /// Gets the handlers delegate.
        /// </summary>
        internal Func<object, bool> Handle { get; }

        /// <summary>
        /// Creates a new handler from the given delegate.
        /// </summary>
        /// <param name="handle">The delegate handle.</param>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The created handler.</returns>
        internal static Handler Create<TMessage>(Action<TMessage> handle)
        {
            bool ActionDelegate(object message)
            {
                if (!(message is TMessage typedMessage))
                {
                    return false;
                }

                handle.Invoke(typedMessage);

                return true;
            }

            return new Handler(typeof(TMessage), ActionDelegate);
        }
    }
}
