// -------------------------------------------------------------------------------------------------
// <copyright file="IScheduler.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    /// <summary>
    /// Provides an interface to a scheduler for schedules actions and message sending.
    /// </summary>
    public interface IScheduler : IActionScheduler, ISendScheduler
    {
    }
}
