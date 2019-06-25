// -------------------------------------------------------------------------------------------------
// <copyright file="IRunnable.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Internal
{
    /// <summary>
    /// An asynchronous operation will be executed by a message dispatcher.
    /// </summary>
    internal interface IRunnable
    {
        /// <summary>
        /// Runs the runnable.
        /// </summary>
        void Run();
    }
}
