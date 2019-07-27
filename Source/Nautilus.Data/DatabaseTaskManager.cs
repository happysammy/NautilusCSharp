//--------------------------------------------------------------------------------------------------
// <copyright file="DatabaseTaskManager.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The component manages the queue of job messages being sent to the database.
    /// </summary>
    public sealed class DatabaseTaskManager : DataBusConnected
    {
        private readonly IBarRepository barRepository;
        private readonly IInstrumentRepository instrumentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTaskManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="barRepository">The bar repository.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        public DatabaseTaskManager(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IBarRepository barRepository,
            IInstrumentRepository instrumentRepository)
            : base(container, dataBusAdapter)
        {
            this.barRepository = barRepository;
            this.instrumentRepository = instrumentRepository;

            this.RegisterHandler<BarData>(this.OnMessage);
            this.RegisterHandler<BarDataFrame>(this.OnMessage);
            this.RegisterHandler<Instrument>(this.OnMessage);
            this.RegisterHandler<TrimBarData>(this.OnMessage);

            this.Subscribe<BarData>();
            this.Subscribe<Instrument>();
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.barRepository.SnapshotDatabase();
        }

        private void OnMessage(BarData data)
        {
            this.barRepository
                .Add(data.BarType, data.Bar)
                .OnSuccess(result => this.Log.Verbose(result.Message))
                .OnFailure(result => this.Log.Warning(result.Message));
        }

        private void OnMessage(BarDataFrame data)
        {
            this.barRepository
                .Add(data)
                .OnSuccess(result => this.Log.Debug(result.Message))
                .OnFailure(result => this.Log.Warning(result.Message));
        }

        private void OnMessage(Instrument data)
        {
            this.instrumentRepository
                .Add(data, this.TimeNow())
                .OnSuccess(result => this.Log.Information(result.Message))
                .OnFailure(result => this.Log.Error(result.Message));
        }

        private void OnMessage(TrimBarData message)
        {
            foreach (var resolution in message.Resolutions)
            {
                this.barRepository
                    .TrimToDays(resolution, message.RollingWindowDays)
                    .OnSuccess(result => this.Log.Information(result.Message))
                    .OnFailure(result => this.Log.Error(result.Message));
            }
        }
    }
}
