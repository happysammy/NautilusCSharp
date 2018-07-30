//--------------------------------------------------------------------------------------------------
// <copyright file="MessagingComponent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    /// <summary>
    /// The messaging component enumeration.
    /// </summary>
    public enum MessagingComponent
    {
        /// <summary>
        /// The command bus messaging component.
        /// </summary>
        CommandBus = 0,

        /// <summary>
        /// The event bus messaging component.
        /// </summary>
        EventBus = 1,

        /// <summary>
        /// The document bus messaging component.
        /// </summary>
        DocumentBus = 2,
    }
}
