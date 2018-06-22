//--------------------------------------------------------------------------------------------------
// <copyright file="TickDataProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Processors
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Aggregators;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides an entry point for ticks into the <see cref="Nautilus.Database"/> system. The given
    /// tick size index must contain all symbols which the processor can expect to receive ticks for.
    /// </summary>
    public sealed class TickDataProcessor : ComponentBase, ITickDataProcessor
    {
        private readonly IReadOnlyDictionary<string, int> tickSizeIndex;
        private readonly IQuoteProvider quoteProvider;
        private readonly IActorRef barAggregationControllerRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickDataProcessor"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="tickSizeIndex">The tick size index.</param>
        /// <param name="quoteProvider">The quote provider.</param>
        /// <param name="barAggregationControllerRef">The bar aggregator controller actor address.</param>
        public TickDataProcessor(
            IComponentryContainer container,
            IReadOnlyDictionary<string, int> tickSizeIndex,
            IQuoteProvider quoteProvider,
            IActorRef barAggregationControllerRef) : base(
            ServiceContext.Database,
            LabelFactory.Component(nameof(TickDataProcessor)),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.CollectionNotNullOrEmpty(tickSizeIndex, nameof(tickSizeIndex));
            Validate.NotNull(quoteProvider, nameof(quoteProvider));
            Validate.NotNull(barAggregationControllerRef, nameof(barAggregationControllerRef));

            this.tickSizeIndex = tickSizeIndex;
            this.quoteProvider = quoteProvider;
            this.barAggregationControllerRef = barAggregationControllerRef;
        }

        /// <summary>
        /// Creates a new <see cref="Tick"/> and sends it to the <see cref="IQuoteProvider"/> and
        /// the <see cref="BarAggregationController"/>.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="exchange">The tick exchange.</param>
        /// <param name="bid">The tick bid price.</param>
        /// <param name="ask">The tick ask price.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        public void OnTick(
            string symbol,
            Exchange exchange,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.DecimalNotOutOfRange(bid, nameof(bid), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
                Validate.DecimalNotOutOfRange(ask, nameof(ask), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

                var securitySymbol = new Symbol(symbol, exchange);
                var tick = new Tick(
                    securitySymbol,
                    Price.Create(bid, tickSizeIndex[symbol]),
                    Price.Create(ask, tickSizeIndex[symbol]),
                    timestamp);

                this.quoteProvider.OnTick(tick);
                this.barAggregationControllerRef.Tell(tick);
            });
        }
    }
}
