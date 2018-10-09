//--------------------------------------------------------------------------------------------------
// <copyright file="DisconnectFixJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Jobs
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a job to disconnect the FIX sessions.
    /// </summary>
    [Immutable]
    public sealed class DisconnectFixJob : IScheduledJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectFixJob"/> class.
        /// </summary>
        public DisconnectFixJob()
        {
        }
    }
}
