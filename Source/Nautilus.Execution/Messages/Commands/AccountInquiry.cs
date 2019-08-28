//--------------------------------------------------------------------------------------------------
// <copyright file="AccountInquiry.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Messages.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using NodaTime;

    /// <summary>
    /// Represents a command for an account inquiry.
    /// </summary>
    [Immutable]
    public sealed class AccountInquiry : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInquiry"/> class.
        /// </summary>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public AccountInquiry(
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                typeof(AccountInquiry),
                commandId,
                commandTimestamp)
        {
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));
        }
    }
}
