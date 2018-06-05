//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityPortfolioStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using System.Collections.Generic;
    using System.Linq;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="SecurityPortfolioStore"/> class. Container for the collection of
    /// <see cref="SecurityPortfolio"/>(s).
    /// </summary>
    public sealed class SecurityPortfolioStore
    {
        private readonly IDictionary<Symbol, IActorRef> portfolioIndex = new Dictionary<Symbol, IActorRef>();

        /// <summary>
        /// The <see cref="SecurityPortfolio"/> count held by the store.
        /// </summary>
        public int Count => this.portfolioIndex.Count;

        /// <summary>
        /// Returns a list of all <see cref="Symbol"/>(s) held by the store.
        /// </summary>
        public IReadOnlyList<Symbol> SymbolList => this.portfolioIndex.Keys.ToList();

        /// <summary>
        /// Adds the given <see cref="SecurityPortfolio"/> to the store (if one doesn't already exist).
        /// </summary>
        /// <param name="portfolioSymbol">The security portfolio symbol.</param>
        /// <param name="portfolioRef">The security portfolio actor address.</param>
        /// <exception cref="ValidationException">Throws if either argument is null, or if the
        /// store already contains the security portfolio symbol.</exception>
        public void AddPortfolio(Symbol portfolioSymbol, IActorRef portfolioRef)
        {
            Validate.NotNull(portfolioSymbol, nameof(portfolioSymbol));
            Validate.NotNull(portfolioRef, nameof(portfolioRef));
            Validate.DictionaryDoesNotContainKey(portfolioSymbol, nameof(portfolioSymbol), this.portfolioIndex);

            this.portfolioIndex.Add(portfolioSymbol, portfolioRef);

            Debug.DictionaryContainsKey(portfolioSymbol, nameof(portfolioSymbol), this.portfolioIndex);
        }

        /// <summary>
        /// Sends the given message to the <see cref="SecurityPortfolio"/> matching the given <see cref="Symbol"/>.
        /// </summary>
        /// <param name="portfolioSymbol">The portfolio symbol.</param>
        /// <param name="message">The message. </param>
        /// <exception cref="ValidationException">Throws if either argument is null, or if the
        /// store does not contain the portfolio symbol.</exception>
        public void Tell(Symbol portfolioSymbol, object message)
        {
            Validate.NotNull(portfolioSymbol, nameof(portfolioSymbol));
            Validate.NotNull(message, nameof(message));
            Validate.DictionaryContainsKey(portfolioSymbol, nameof(portfolioSymbol), this.portfolioIndex);

            this.portfolioIndex[portfolioSymbol].Tell(message);
        }
    }
}
