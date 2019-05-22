//--------------------------------------------------------------------------------------------------
// <copyright file="SystemCommand.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands.Base
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
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
        /// <param name="commandIdentifier">The commands identifier.</param>
        /// <param name="commandTimestamp">The commands timestamp.</param>
        protected SystemCommand(
            Guid commandIdentifier,
            ZonedDateTime commandTimestamp)
            : base(commandIdentifier, commandTimestamp)
        {
            Debug.NotDefault(commandIdentifier, nameof(commandIdentifier));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));
        }
    }
}
