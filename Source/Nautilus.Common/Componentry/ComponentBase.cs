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
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all system components.
    /// </summary>
    public abstract class ComponentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase"/> class.
        /// </summary>
        /// <param name="service">The service name.</param>
        /// <param name="component">The component label.</param>
        /// <param name="container">The setup container.</param>
        protected ComponentBase(
            Enum service,
            Label component,
            ComponentryContainer container)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));

            this.Service = service;
            this.Component = component;
            this.Clock = container.Clock;
            this.Logger = container.LoggerFactory.Create(service, this.Component);
            this.GuidFactory = container.GuidFactory;
            this.CommandHandler = new CommandHandler(this.Logger);
        }

        /// <summary>
        /// Gets the black box service context.
        /// </summary>
        protected Enum Service { get; }

        /// <summary>
        /// Gets the components label.
        /// </summary>
        protected Label Component { get; }

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
        /// Logs the result with the <see cref="ILogger"/>.
        /// </summary>
        /// <param name="result">The command result.</param>
        public void LogResult(ResultBase result)
        {
            if (result.IsSuccess)
            {
                this.Log(LogLevel.Information, result.Message);
            }
            else
            {
                this.Log(LogLevel.Warning, result.Message);
            }
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
