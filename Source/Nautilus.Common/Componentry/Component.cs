//--------------------------------------------------------------------------------------------------
// <copyright file="Component.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using NodaTime;

    /// <summary>
    /// The base class for all service components.
    /// </summary>
    public abstract class Component
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        /// <param name="container">The components componentry container.</param>
        /// <param name="subName">The sub-name for the component.</param>
        protected Component(IComponentryContainer container, string subName = "")
        {
            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;

            this.Name = new Label(this.GetType().NameFormatted() + SetSubName(subName));
            this.Logger = container.LoggerFactory.CreateLogger(this.Name.Value);

            this.InitializedTime = this.clock.TimeNow();
            this.Logger.LogDebug(LogId.Operation, "Initialized.");
        }

        /// <summary>
        /// Gets the components name.
        /// </summary>
        public Label Name { get; }

        /// <summary>
        /// Gets the time the component was initialized.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public ZonedDateTime InitializedTime { get; }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Returns the current time of the service clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow() => this.clock.TimeNow();

        /// <summary>
        /// Returns the current instant of the service clock.
        /// </summary>
        /// <returns>
        /// An <see cref="Instant"/>.
        /// </returns>
        protected Instant InstantNow() => this.clock.InstantNow();

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid() => this.guidFactory.Generate();

        private static string SetSubName(string subName)
        {
            if (subName != string.Empty)
            {
                subName = $"-{subName}";
            }

            return subName;
        }
    }
}
