//--------------------------------------------------------------------------------------------------
// <copyright file="BarPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Protobuf
{
    using System.Threading.Tasks;
    using Akka.Actor;
    using Grpc.Core;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    using BarSpecification = Nautilus.DomainModel.ValueObjects.BarSpecification;
    using QuoteType = Nautilus.DomainModel.Enums.QuoteType;
    using Resolution = Nautilus.DomainModel.Enums.Resolution;

    /// <summary>
    /// Provides a protobuffer endpoint for subscribing to and unsubscribing from market data.
    /// </summary>
    public class DataSubscriptionServer : DataServer.DataServerBase
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly IActorRef dataCollectionManagerRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSubscriptionServer"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="guidFactory">The GUID factory.</param>
        /// <param name="dataCollectionManagerRef">The data collection managers actor address.</param>
        public DataSubscriptionServer(
            IZonedClock clock,
            IGuidFactory guidFactory,
            IActorRef dataCollectionManagerRef)
        {
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(guidFactory, nameof(guidFactory));
            Validate.NotNull(dataCollectionManagerRef, nameof(dataCollectionManagerRef));

            this.clock = clock;
            this.guidFactory = guidFactory;
            this.dataCollectionManagerRef = dataCollectionManagerRef;
        }
        /// <summary>
        /// Subscribes the sender to the given bar data.
        /// </summary>
        /// <param name="request">The rpc request.</param>
        /// <param name="context">the rpc context.</param>
        /// <returns>The senders reponse.</returns>
        public override Task<SubscribeTickDataResponse> OnSubscribeTickData(
            SubscribeTickData request,
            ServerCallContext context)
        {
            Validate.NotNull(request, nameof(request));
            Validate.NotNull(context, nameof(context));

            var subscribe = new Subscribe<Symbol>(
                new Symbol(
                    request.Symbol,
                    request.Venue.ToEnum<Exchange>()),
                this.guidFactory.NewGuid(),
                this.clock.TimeNow());

            this.dataCollectionManagerRef.Tell(subscribe);

            return Task.FromResult(new SubscribeTickDataResponse { Success = true });
        }

        /// <summary>
        /// Unsubscribes the sender to the given bar data.
        /// </summary>
        /// <param name="request">The rpc request.</param>
        /// <param name="context">the rpc context.</param>
        /// <returns>The senders reponse.</returns>
        public override Task<UnsubscribeTickDataResponse> OnUnsubscribeTickData(
            UnsubscribeTickData request,
            ServerCallContext context)
        {
            Validate.NotNull(request, nameof(request));
            Validate.NotNull(context, nameof(context));

            var unsubscribe = new Unsubscribe<Symbol>(
                new Symbol(
                    request.Symbol,
                    request.Venue.ToEnum<Exchange>()),
                this.guidFactory.NewGuid(),
                this.clock.TimeNow());

            this.dataCollectionManagerRef.Tell(unsubscribe);

            return Task.FromResult(new UnsubscribeTickDataResponse() { Success = true });
        }

        /// <summary>
        /// Subscribes the sender to the given bar data.
        /// </summary>
        /// <param name="request">The rpc request.</param>
        /// <param name="context">the rpc context.</param>
        /// <returns>The senders reponse.</returns>
        public override Task<SubscribeBarDataResponse> OnSubscribeBarData(
            SubscribeBarData request,
            ServerCallContext context)
        {
            Validate.NotNull(request, nameof(request));
            Validate.NotNull(context, nameof(context));

            var symbol = new Symbol(request.Symbol, request.Venue.ToEnum<Exchange>());
            var barSpec = new BarSpecification(
                (QuoteType)request.BarSpec.QuoteType,
                (Resolution)request.BarSpec.Resolution,
                request.BarSpec.Period);
            var barType = new BarType(symbol, barSpec);

            var subscribe = new Unsubscribe<BarType>(
                barType,
                this.guidFactory.NewGuid(),
                this.clock.TimeNow());

            this.dataCollectionManagerRef.Tell(subscribe);

            return Task.FromResult(new SubscribeBarDataResponse { Success = true });
        }

        /// <summary>
        /// Unsubscribes the sender from the given bar data.
        /// </summary>
        /// <param name="request">The rpc request.</param>
        /// <param name="context">the rpc context.</param>
        /// <returns>The senders reponse.</returns>
        public override Task<UnsubscribeBarDataResponse> OnUnsubscribeBarData(
            UnsubscribeBarData request,
            ServerCallContext context)
        {
            Validate.NotNull(request, nameof(request));
            Validate.NotNull(context, nameof(context));

            var symbol = new Symbol(request.Symbol, request.Venue.ToEnum<Exchange>());
            var barSpec = new BarSpecification(
                (QuoteType)request.BarSpec.QuoteType,
                (Resolution)request.BarSpec.Resolution,
                request.BarSpec.Period);
            var barType = new BarType(symbol, barSpec);

            var unsubscribe = new Unsubscribe<BarType>(
                barType,
                this.guidFactory.NewGuid(),
                this.clock.TimeNow());

            this.dataCollectionManagerRef.Tell(unsubscribe);

            return Task.FromResult(new UnsubscribeBarDataResponse() { Success = true });
        }
    }
}
