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
    public enum Messaging
    {
        /// <summary>
        /// The command bus service context.
        /// </summary>
        CommandBus = 0,

        /// <summary>
        /// The event bus service context.
        /// </summary>
        EventBus = 1,

        /// <summary>
        /// The document bus service context.
        /// </summary>
        DocumentBus = 2,
    }
}
