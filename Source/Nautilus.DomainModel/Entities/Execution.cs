//--------------------------------------------------------------------------------------------------
// <copyright file="Execution.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Entities.Base;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents an execution in a financial market.
    /// </summary>
    [Immutable]
    public sealed class Execution : Entity<Execution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Execution"/> class.
        /// </summary>
        /// <param name="identifier">The execution identifier.</param>
        /// <param name="timestamp">The execution timestamp.</param>
        public Execution(
            ExecutionId identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
        }
    }
}
