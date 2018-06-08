//--------------------------------------------------------------------------------------------------
// <copyright file="AlphaStrategyModuleStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Strategy
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="AlphaStrategyModuleStore"/> class. Stores alpha strategy modules within
    /// the AlphaModel service.
    /// </summary>
    public sealed class AlphaStrategyModuleStore
    {
        private readonly Dictionary<Label, IActorRef> alphaStrategyIndex = new Dictionary<Label, IActorRef>();
        private readonly Dictionary<SymbolBarSpec, List<IActorRef>> alphaStrategyBarIndex = new Dictionary<SymbolBarSpec, List<IActorRef>>();

        /// <summary>
        /// Gets the count of alpha strategy modules contained in the store.
        /// </summary>
        public int Count => this.alphaStrategyIndex.Count;

        /// <summary>
        /// Gets a of alpha strategy module labels held in the store.
        /// </summary>
        public IReadOnlyCollection<Label> StrategyLabelList => this.alphaStrategyIndex.Keys.ToImmutableList();

        /// <summary>
        /// Adds the given strategy label and actor address to the store.
        /// </summary>
        /// <param name="strategyLabel">The strategy label.</param>
        /// <param name="symbolBarSpec">The symbol bar spec.</param>
        /// <param name="moduleRef">The module actor address.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public void AddStrategy(
            Label strategyLabel,
            SymbolBarSpec symbolBarSpec,
            IActorRef moduleRef)
        {
            Validate.NotNull(strategyLabel, nameof(strategyLabel));
            Validate.NotNull(moduleRef, nameof(moduleRef));
            Validate.DictionaryDoesNotContainKey(strategyLabel, nameof(strategyLabel), this.alphaStrategyIndex);

            this.alphaStrategyIndex.Add(strategyLabel, moduleRef);

            if (!this.alphaStrategyBarIndex.ContainsKey(symbolBarSpec))
            {
                this.alphaStrategyBarIndex.Add(symbolBarSpec, new List<IActorRef>());
            }

            this.alphaStrategyBarIndex[symbolBarSpec].Add(moduleRef);
        }

        /// <summary>
        /// Removes the strategy corresponding to the given label from the store.
        /// </summary>
        /// <param name="strategyLabel">The strategy label.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void RemoveStrategy(Label strategyLabel)
        {
            Validate.NotNull(strategyLabel, nameof(strategyLabel));
            Validate.DictionaryContainsKey(strategyLabel, nameof(strategyLabel), this.alphaStrategyIndex);

            this.alphaStrategyIndex[strategyLabel].Tell(PoisonPill.Instance);
            this.alphaStrategyIndex.Remove(strategyLabel);
        }

        /// <summary>
        /// Sends the given message to the strategy corresponding to the given label.
        /// </summary>
        /// <param name="strategyLabel">The strategy label.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public void Tell(Label strategyLabel, object message)
        {
            Validate.NotNull(strategyLabel, nameof(strategyLabel));
            Validate.NotNull(message, nameof(message));
            Validate.DictionaryContainsKey(strategyLabel, nameof(strategyLabel), this.alphaStrategyIndex);

            this.alphaStrategyIndex[strategyLabel].Tell(message);
        }

        /// <summary>
        /// Sends the given message to the strategy corresponding to the given label.
        /// </summary>
        /// <param name="symbolBarSpec">The symbol bar specification.</param>
        /// <param name="bar">The bar.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public void Tell(SymbolBarSpec symbolBarSpec, BarDataEvent bar)
        {
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));
            Validate.NotNull(bar, nameof(bar));
            Validate.DictionaryContainsKey(symbolBarSpec, nameof(symbolBarSpec), this.alphaStrategyBarIndex);

            foreach (var strategy in this.alphaStrategyBarIndex[symbolBarSpec])
            {
                strategy.Tell(bar);
            }
        }
    }
}
