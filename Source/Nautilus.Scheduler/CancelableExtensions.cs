// -------------------------------------------------------------------------------------------------
// <copyright file="CancelableExtensions.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    /// <summary>
    /// TBD.
    /// </summary>
    public static class CancelableExtensions
    {
        /// <summary>
        /// If <paramref name="cancelable"/> is not <c>null</c> it's canceled.
        /// </summary>
        /// <param name="cancelable">The cancelable. Will be canceled if it's not <c>null</c>.</param>
        public static void CancelIfNotNull(this ICancelable cancelable)
        {
            cancelable?.Cancel();
        }
    }
}
