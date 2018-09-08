//--------------------------------------------------------------------------------------------------
// <copyright file="CollateralInquiry.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// A command to invoke a collateral inquiry request.
    /// </summary>
    [Immutable]
    public sealed class CollateralInquiry : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollateralInquiry"/> class.
        /// </summary>
        /// <param name="identifier">The message identifier (cannot be default).</param>
        /// <param name="timestamp">The message timestamp (cannot be default).</param>
        public CollateralInquiry(
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
