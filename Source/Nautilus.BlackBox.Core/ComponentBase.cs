// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentBase.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    using System;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The abstract <see cref="ComponentBase"/> class. The base class for all <see cref="BlackBox"/>
    /// components.
    /// </summary>
    public abstract class ComponentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase"/> class.
        /// </summary>
        /// <param name="service">The black box service name.</param>
        /// <param name="component">The component label.</param>
        /// <param name="setupContainer">The black box setup container.</param>
        protected ComponentBase(
            BlackBoxService service,
            Label component,
            BlackBoxSetupContainer setupContainer)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(setupContainer, nameof(setupContainer));

            this.Service = service;
            this.Component = component;
            this.Environment = setupContainer.Environment;
            this.Clock = setupContainer.Clock;
            this.Logger = setupContainer.LoggerFactory.Create(service, this.Component);
            this.GuidFactory = setupContainer.GuidFactory;
            this.CommandHandler = new CommandHandler(this.Logger);
        }

        /// <summary>
        /// Gets the black box service context.
        /// </summary>
        protected BlackBoxService Service { get; }

        /// <summary>
        /// Gets the components label.
        /// </summary>
        protected Label Component { get; }

        /// <summary>
        /// Gets the black box environment.
        /// </summary>
        protected BlackBoxEnvironment Environment { get; }

        /// <summary>
        /// Gets the black box system clock.
        /// </summary>
        protected IZonedClock Clock { get; }

        /// <summary>
        /// Gets the black box system logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the black box <see cref="Guid"/> factory.
        /// </summary>
        protected IGuidFactory GuidFactory { get; }

        /// <summary>
        /// Gets the command handler.
        /// </summary>
        protected CommandHandler CommandHandler { get; }

        /// <summary>
        /// Creates a log event with the given level and text.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logText">The log text.</param>
        protected void Log(LogLevel logLevel, string logText)
        {
            this.Logger.Log(logLevel, logText);
        }

        /// <summary>
        /// Returns the current time of the black box system clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow()
        {
            return this.Clock.TimeNow();
        }

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the black box systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid()
        {
            return this.GuidFactory.NewGuid();
        }
    }
}
