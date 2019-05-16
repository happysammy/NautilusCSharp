// -------------------------------------------------------------------------------------------------
// <copyright file="IRunnable.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    /// <summary>
    /// An asynchronous operation will be executed by a message dispatcher.
    /// </summary>
    internal interface IRunnable
    {
        /// <summary>
        /// TBD.
        /// </summary>
        void Run();
    }
}
