// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentryContainer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

using NautechSystems.CSharp.Annotations;
using NautechSystems.CSharp.Validation;

namespace Nautilus.Common.Componentry
{
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The base class for all component setup containers.
    /// </summary>
    [Immutable]
    public abstract class ComponentryContainer
    {
        protected ComponentryContainer(
            IZonedClock clock,
            IGuidFactory guidFactory,
            ILoggerFactory loggerFactory)
        {
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(guidFactory, nameof(guidFactory));
            Validate.NotNull(loggerFactory, nameof(loggerFactory));

            this.Clock = clock;
            this.GuidFactory = guidFactory;
            this.LoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Gets the containers clock.
        /// </summary>
        public IZonedClock Clock { get; }

        /// <summary>
        /// Gets the containers guid factory.
        /// </summary>
        public IGuidFactory GuidFactory { get; }

        /// <summary>
        /// Gets the containers logger.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }
    }
}
