//--------------------------------------------------------------------------------------------------
// <copyright file="DatabaseTaskManager.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Documents;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The component manages the queue of job messages being sent to the database.
    /// </summary>
    public class DatabaseTaskManager : ComponentBase
    {
        private readonly IBarRepository barRepository;
        private readonly IInstrumentRepository instrumentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTaskManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="barRepository">The bar repository.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        public DatabaseTaskManager(
            IComponentryContainer container,
            IBarRepository barRepository,
            IInstrumentRepository instrumentRepository)
            : base(NautilusService.Data, container)
        {
            this.barRepository = barRepository;
            this.instrumentRepository = instrumentRepository;

            this.RegisterHandler<DataDelivery<(BarType, Bar)>>(this.OnMessage);
            this.RegisterHandler<DataDelivery<BarDataFrame>>(this.OnMessage);
            this.RegisterHandler<DataDelivery<Instrument>>(this.OnMessage);
            this.RegisterHandler<TrimBarData>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.Log.Information($"Stopping from {message}...");
            this.barRepository.SnapshotDatabase();
        }

        private void OnMessage(TrimBarData message)
        {
            foreach (var resolution in message.Resolutions)
            {
                this.barRepository
                    .TrimToDays(resolution, message.RollingWindowDaysDays)
                    .OnSuccess(result => this.Log.Information(result.Message))
                    .OnFailure(result => this.Log.Error(result.Message));
            }
        }

        private void OnMessage(DataDelivery<(BarType BarType, Bar Bar)> message)
        {
            this.barRepository
                .Add(message.Data.BarType, message.Data.Bar)
                .OnSuccess(result => this.Log.Verbose(result.Message))
                .OnFailure(result => this.Log.Warning(result.Message));
        }

        private void OnMessage(DataDelivery<BarDataFrame> message)
        {
            this.barRepository
                .Add(message.Data)
                .OnSuccess(result => this.Log.Debug(result.Message))
                .OnFailure(result => this.Log.Warning(result.Message));
        }

        private void OnMessage(DataDelivery<Instrument> message)
        {
            this.instrumentRepository
                .Add(message.Data, this.TimeNow())
                .OnSuccess(result => this.Log.Information(result.Message))
                .OnFailure(result => this.Log.Error(result.Message));
        }
    }
}
