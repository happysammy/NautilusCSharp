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
    using System.Threading;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Enums;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The main macro object which contains the <see cref="Database"/> and presents its API.
    /// </summary>
    public sealed class Database : ComponentBusConnectedBase, IDisposable
    {
        private readonly ActorSystem actorSystem;
        private readonly IReadOnlyDictionary<Enum, IActorRef> addresses;
        private readonly IDataClient dataClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="addresses">The system service addresses.</param>
        /// <param name="dataClient">The data client.</param>
        /// <param name="quoteProvider">The quote provider.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public Database(
            DatabaseSetupContainer setupContainer,
            ActorSystem actorSystem,
            MessagingAdapter messagingAdapter,
            IReadOnlyDictionary<Enum, IActorRef> addresses,
            IDataClient dataClient,
            IQuoteProvider quoteProvider)
            : base(
                ServiceContext.Database,
                LabelFactory.Component(nameof(Database)),
                setupContainer,
                messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(addresses, nameof(addresses));
            Validate.NotNull(quoteProvider, nameof(quoteProvider));

            this.actorSystem = actorSystem;
            this.addresses = addresses;
            this.dataClient = dataClient;

            messagingAdapter.Send(new InitializeMessageSwitchboard(
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
                DatabaseService.CollectionManager,
                new StartSystem(
                    Guid.NewGuid(),
                    this.TimeNow()));

            this.dataClient.Connect();

            Thread.Sleep(1000);

            this.dataClient.InitializeSession();
        }

        /// <summary>
        /// Gracefully shuts down the <see cref="Database"/> system.
        /// </summary>
        public void Shutdown()
        {
            // Placeholder for the log events (do not refactor away).
            var actorSystemName = this.actorSystem.Name;

            this.Log.Information($"{actorSystemName} ActorSystem shutting down...");

            var shutdownTasks = new List<Task>();
            foreach (var address in this.addresses)
            {
                shutdownTasks.Add(address.Value.GracefulStop(TimeSpan.FromSeconds(10)));
            }
            shutdownTasks.ToArray();

            this.Log.Information($"Waiting for actors to shut down...");
            Task.WhenAll(shutdownTasks);

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
