//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceContext.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    public enum ServiceContext
    {
        Messaging = 0,
        CommandBus = 1,
        EventBus = 2,
        DocumentBus = 3,
        Database = 4,
        AspCoreHost = 5
    }
}
