//--------------------------------------------------------------------------------------------------
// <copyright file="CollateralInquiry.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// Represents a command to invoke a collateral inquiry request.
    /// </summary>
    [Immutable]
    public sealed class CollateralInquiry : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollateralInquiry"/> class.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public CollateralInquiry(
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(CollateralInquiry),
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
