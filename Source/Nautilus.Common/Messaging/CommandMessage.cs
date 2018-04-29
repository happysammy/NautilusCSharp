//--------------------------------------------------------------------------------------------------
// <copyright file="CommandMessage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using NautechSystems.CSharp.Annotations;
using NautechSystems.CSharp.Validation;
using NodaTime;

namespace Nautilus.Common.Messaging
{
    [Immutable]
    public abstract class CommandMessage : Message
    {
        protected CommandMessage(Guid id, ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
