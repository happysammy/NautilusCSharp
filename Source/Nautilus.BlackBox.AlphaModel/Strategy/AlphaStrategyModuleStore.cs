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
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="AlphaStrategyModuleStore"/> class. Stores alpha strategy modules within
    /// the AlphaModel service.
    /// </summary>
    public sealed class AlphaStrategyModuleStore
    {
        private readonly Dictionary<Label, IActorRef> alphaStrategyIndex = new Dictionary<Label, IActorRef>();

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
        /// <param name="moduleRef">The module actor address.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public void AddStrategy(Label strategyLabel, IActorRef moduleRef)
        {
            Validate.NotNull(strategyLabel, nameof(strategyLabel));
            Validate.NotNull(moduleRef, nameof(moduleRef));
            Validate.DictionaryDoesNotContainKey(strategyLabel, nameof(strategyLabel), this.alphaStrategyIndex);

            this.alphaStrategyIndex.Add(strategyLabel, moduleRef);
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
    }
}
