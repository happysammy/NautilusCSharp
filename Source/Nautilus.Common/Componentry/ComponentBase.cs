//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all system components.
    /// </summary>
    [Stateless]
    public abstract class ComponentBase
    {
        private readonly IZonedClock clock;
        private readonly ILogger logger;
        private readonly IGuidFactory guidFactory;
        private readonly CommandHandler commandHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase"/> class.
        /// </summary>
        /// <param name="serviceContext">The components service context.</param>
        /// <param name="component">The components label.</param>
        /// <param name="container">The components componentry container.</param>
        protected ComponentBase(
            NautilusService serviceContext,
            Label component,
            IComponentryContainer container)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));

            this.Service = serviceContext;
            this.Component = component;
            this.clock = container.Clock;
            this.logger = container.LoggerFactory.Create(serviceContext, this.Component);
            this.guidFactory = container.GuidFactory;
            this.commandHandler = new CommandHandler(this.logger);
        }

        /// <summary>
        /// Gets the components service context.
        /// </summary>
        protected NautilusService Service { get; }

        /// <summary>
        /// Gets the components label.
        /// </summary>
        protected Label Component { get; }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Log => this.logger;

        /// <summary>
        /// Returns the current time of the black box system clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow()
        {
            return this.clock.TimeNow();
        }

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the black box systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid()
        {
            return this.guidFactory.NewGuid();
        }

        /// <summary>
        /// Passes the given <see cref="Action"/> to the <see cref="commandHandler"/> for execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected void Execute(Action action)
        {
            this.commandHandler.Execute(action);
        }
    }
}
