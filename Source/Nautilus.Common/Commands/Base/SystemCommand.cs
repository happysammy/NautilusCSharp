//--------------------------------------------------------------------------------------------------
// <copyright file="SystemCommand.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands.Base
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The base class for all system commands.
    /// </summary>
    [Immutable]
    public class SystemCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCommand"/> class.
        /// </summary>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        protected SystemCommand(
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(commandId, commandTimestamp)
        {
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));
        }
    }
}
