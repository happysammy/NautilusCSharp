// -------------------------------------------------------------------------------------------------
// <copyright file="IRunnable.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;

    /// <summary>
    /// An asynchronous operation will be executed by a message dispatcher.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// TBD.
        /// </summary>
        void Run();
    }

    /// <summary>
    /// <see cref="IRunnable"/> which executes an <see cref="Action"/>
    /// </summary>
    public sealed class ActionRunnable : IRunnable
    {
        private readonly Action action;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="action">TBD</param>
        public ActionRunnable(Action action)
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

    /// <summary>
    /// <see cref="IRunnable"/> which executes an <see cref="Action{state}"/> and an <see cref="object"/> representing the state.
    /// </summary>
    public sealed class ActionWithStateRunnable : IRunnable
    {
        private readonly Action<object> actionWithState;
        private readonly object state;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="actionWithState">TBD</param>
        /// <param name="state">TBD</param>
        public ActionWithStateRunnable(Action<object> actionWithState, object state)
        {
            this.actionWithState = actionWithState;
            this.state = state;
        }

        /// <summary>
        /// Invoke the action with state.
        /// </summary>
        public void Run()
        {
            this.actionWithState(this.state);
        }
    }
}

