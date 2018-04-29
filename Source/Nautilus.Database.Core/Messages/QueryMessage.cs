// -------------------------------------------------------------------------------------------------
// <copyright file="QueryMessage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Messages
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;
    using Nautilus.Common.Messaging;

    [Immutable]
    public abstract class QueryMessage : Message
    {
        /// <exception cref="T:NautechSystems.CSharp.Validation.ValidationException">Throws if the validation fails.</exception>
        protected QueryMessage(Guid identifier, ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
