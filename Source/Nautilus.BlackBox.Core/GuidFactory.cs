// -------------------------------------------------------------------------------------------------
// <copyright file="GuidFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using Nautilus.BlackBox.Core.Interfaces;

    /// <summary>
    /// The immutable sealed <see cref="GuidFactory"/> class. Provides <see cref="Guid"/>(s) for the
    /// <see cref="BlackBox"/> system.
    /// </summary>
    [Immutable]
    public sealed class GuidFactory : IGuidFactory
    {
        /// <summary>
        /// Returns a new <see cref="Guid"/>.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        public Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
}
