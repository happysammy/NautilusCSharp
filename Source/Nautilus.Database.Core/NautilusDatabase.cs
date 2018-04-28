//--------------------------------------------------------------
// <copyright file="NautilusDatabase.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Database.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Akka.Actor;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Messages;
    using Nautilus.Database.Core.Messages;

    /// <summary>
    /// The main macro object which contains the <see cref="NautilusDatabase"/> and presents its API.
    /// </summary>
    public sealed class NautilusDatabase : ComponentBase, IDisposable
    {
        private readonly ActorSystem actorSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusDatabase"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="actorReferences">The actor references.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public NautilusDatabase(
            ComponentryContainer container,
            ActorSystem actorSystem,
            ActorReferences actorReferences)
            : base(container, nameof(NautilusDatabase))
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(actorReferences, nameof(actorReferences));

            this.actorSystem = actorSystem;
            this.actorReferences = actorReferences;
        }

        public async void Start()
        {
            var status = await this.RequestSystemStatus();

            if (status == SystemStatus.Failure || status == SystemStatus.Suspended)
            {
                this.Logger.Warning($"{this.Component} cannot start due to a suspended or failed component.");
            }
            else
            {
                this.Logger.Information($"{this.Component} sending StartSystem message to all components...");

                this.actorReferences.DataCollectionManager.Tell(
                    new StartSystem(
                        Guid.NewGuid(),
                        this.Clock.TimeNow()));
            }
        }

        public async Task<SystemStatus> RequestSystemStatus()
        {
            var statusRequest = new SystemStatusRequest(Guid.NewGuid(), this.Clock.TimeNow());
            var startupTasks = new List<Task<SystemStatusResponse>>
                                   {
                                       this.actorReferences
                                           .DatabaseTaskManager
                                           .Ask<SystemStatusResponse>(statusRequest),

                                       this.actorReferences
                                           .DataCollectionManager
                                           .Ask<SystemStatusResponse>(statusRequest)
                                   };

            await Task.WhenAll(startupTasks);

            startupTasks.ForEach(t => this.Logger.Information(
                $"{t.Result.ComponentName}: "
              + $"SystemStatus={t.Result.Status}"));

            return startupTasks.Any(r => r.Result.Status == SystemStatus.Failure)
                             ? SystemStatus.Failure
                             : SystemStatus.OK;
        }

        /// <summary>
        /// Gracefully shuts down the <see cref="NautilusDatabase"/> system.
        /// </summary>
        public void Shutdown()
        {
            // Placeholder for the log events (do not refactor away).
            var actorSystemName = this.actorSystem.Name;

            this.Logger.Information($"{this.ComponentName} {actorSystemName} ActorSystem shutting down...");

            var shutdownTasks = new Task[]
            {
                this.actorReferences.DataCollectionManager.GracefulStop(TimeSpan.FromSeconds(10)),
                this.actorReferences.DatabaseTaskManager.GracefulStop(TimeSpan.FromSeconds(10))
            };

            this.Logger.Information($"{this.ComponentName} waiting for actors to shut down...");
            Task.WhenAll(shutdownTasks);

            this.actorSystem.Terminate();
            this.Logger.Information($"{this.ComponentName} {actorSystemName} terminated");

            this.Dispose();
        }

        /// <summary>
        /// Disposes the <see cref="NautilusDatabase"/> object.
        /// </summary>
        public void Dispose()
        {
            this.actorSystem?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}