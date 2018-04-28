//--------------------------------------------------------------
// <copyright file="ServiceContext.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

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
