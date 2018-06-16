//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDatabase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Messages;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Enums;
    using Nautilus.Database.Messages;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The main macro object which contains the <see cref="Database"/> and presents its API.
    /// </summary>
    public sealed class Database : ComponentBusConnectedBase, IDisposable
    {
        private readonly ActorSystem actorSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="messagingAdatper">The messaging adapter.</param>
        /// <param name="addresses">The system service addresses.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public Database(
            DatabaseSetupContainer setupContainer,
            ActorSystem actorSystem,
            MessagingAdapter messagingAdatper,
            IReadOnlyDictionary<Enum, IActorRef> addresses)
            : base(
                ServiceContext.Database,
                LabelFactory.Component(nameof(Database)),
                setupContainer,
                messagingAdatper)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(messagingAdatper, nameof(messagingAdatper));

            this.actorSystem = actorSystem;

            messagingAdatper.Send(new InitializeMessageSwitchboard(
                new Switchboard(addresses),
                setupContainer.GuidFactory.NewGuid(),
                this.TimeNow()));
        }

        /// <summary>
        /// Start the database.
        /// </summary>
        public void Start()
        {
            this.Send(
                DatabaseService.DatabaseCollectionManager,
                new StartSystem(
                    Guid.NewGuid(),
                    this.TimeNow()));
        }

        /// <summary>
        /// Gracefully shuts down the <see cref="Database"/> system.
        /// </summary>
        public void Shutdown()
        {
            // Placeholder for the log events (do not refactor away).
            var actorSystemName = this.actorSystem.Name;

            this.Log.Information($"{actorSystemName} ActorSystem shutting down...");

//            var shutdownTasks = new Task[]
//            {
//                this.actorReferences.DataCollectionManager.GracefulStop(TimeSpan.FromSeconds(10)),
//                this.actorReferences.DatabaseTaskManager.GracefulStop(TimeSpan.FromSeconds(10))
//            };

            this.Log.Information($"waiting for actors to shut down...");
            //Task.WhenAll(shutdownTasks);

            this.actorSystem.Terminate();
            this.Log.Information($"{actorSystemName} terminated");

            this.Dispose();
        }

        /// <summary>
        /// Disposes the <see cref="Database"/> object.
        /// </summary>
        public void Dispose()
        {
            this.actorSystem?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
