//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using NautilusMQ;
    using NodaTime;

    /// <summary>
    /// The base class for all system components.
    /// </summary>
    public abstract class ComponentBase : MessageReceiver
    {
        private readonly IZonedClock clock;
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
            this.clock = container.Clock;
            this.StartTime = this.clock.TimeNow();
            this.Log = container.LoggerFactory.Create(serviceContext, component);
            this.guidFactory = container.GuidFactory;
            this.commandHandler = new CommandHandler(this.Log);

            this.RegisterHandler<StartSystem>(this.OnMessage);
            this.RegisterHandler<ShutdownSystem>(this.OnMessage);
        }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        /// Gets the time the component was last started or reset.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        protected ZonedDateTime StartTime { get; }

        /// <summary>
        /// Starts the component.
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Returns the current time of the system clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow() => this.clock.TimeNow();

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid() => this.guidFactory.NewGuid();

        /// <summary>
        /// Passes the given <see cref="Action"/> to the <see cref="commandHandler"/> for execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected void Execute(Action action)
        {
            this.commandHandler.Execute(action);
        }

        private void OnMessage(StartSystem message)
        {
            this.Start();
        }

        private void OnMessage(ShutdownSystem message)
        {
            this.Stop();
        }
    }
}
