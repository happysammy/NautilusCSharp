// -------------------------------------------------------------------------------------------------
// <copyright file="ActionRunnable.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Internal
{
    using System;

    /// <summary>
    /// <see cref="IRunnable"/> which executes an <see cref="Action"/>.
    /// </summary>
    internal sealed class ActionRunnable : IRunnable
    {
        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRunnable"/> class.
        /// </summary>
        /// <param name="action">The runnable delegate.</param>
        internal ActionRunnable(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        public void Run()
        {
            this.action();
        }
    }
}
