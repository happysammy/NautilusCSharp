//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceContext.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    /// <summary>
    /// The system service context.
    /// </summary>
    public enum ServiceContext
    {
        /// <summary>
        /// The messaging service context.
        /// </summary>
        Messaging = 0,

        /// <summary>
        /// THe command bus service context.
        /// </summary>
        CommandBus = 1,

        /// <summary>
        /// The event bus service context.
        /// </summary>
        EventBus = 2,

        /// <summary>
        /// The document bus service context.
        /// </summary>
        DocumentBus = 3,

        /// <summary>
        /// The database service context.
        /// </summary>
        Database = 4,

        /// <summary>
        /// The FIX service context.
        /// </summary>
        FIX = 5,

        /// <summary>
        /// The ASP.NET Core hosting context.
        /// </summary>
        AspCoreHost = 6,

        /// <summary>
        /// The Serilog service context.
        /// </summary>
        Serilog = 7,

        /// <summary>
        /// The RavenDB service context.
        /// </summary>
        RavenDB = 8
    }
}
