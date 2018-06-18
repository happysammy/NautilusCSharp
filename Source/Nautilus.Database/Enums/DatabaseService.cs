//--------------------------------------------------------------------------------------------------
// <copyright file="DatabaseService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Enums
{
    /// <summary>
    /// The database service enumeration.
    /// </summary>
    public enum DatabaseService
    {
        /// <summary>
        /// The core database service.
        /// </summary>
        Core = 0,

        /// <summary>
        /// The task manager database service.
        /// </summary>
        TaskManager = 1,

        /// <summary>
        /// The collection manager database service.
        /// </summary>
        CollectionManager = 2,
    }
}
