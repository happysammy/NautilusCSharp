//--------------------------------------------------------------------------------------------------
// <copyright file="CollateralInquiry.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
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
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public CollateralInquiry(
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                typeof(CollateralInquiry),
                commandId,
                commandTimestamp)
        {
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));
        }
    }
}
