//--------------------------------------------------------------------------------------------------
// <copyright file="ConnectFixJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Jobs
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a job to connect to the FIX sessions.
    /// </summary>
    [Immutable]
    public sealed class ConnectFixJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectFixJob"/> class.
        /// </summary>
        public ConnectFixJob()
        {
        }
    }
}
