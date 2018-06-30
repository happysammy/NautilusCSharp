//--------------------------------------------------------------------------------------------------
// <copyright file="TradeBook.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Collections;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="TradeBook"/> class (implements <see cref="ITradeBook"/>).
    /// </summary>
    public sealed class TradeBook : ComponentBase, ITradeBook
    {
        // Concreate list for performance reasons.
        private readonly List<Trade> tradeList = new List<Trade>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBook"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public TradeBook(
            IComponentryContainer container,
            Symbol symbol)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Component(nameof(TradeBook), symbol),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(symbol, nameof(symbol));
        }

        /// <summary>
        /// Gets the trade books label.
        /// </summary>
        public Label Label => this.Component;

        /// <summary>
        /// Processes all trades of the given trade type in the trade book.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <exception cref="ValidationException">Throws if the trade type is null.</exception>
        public void Process(TradeType tradeType)
        {
            Debug.NotNull(tradeType, nameof(tradeType));

            var tradesByType = this.GetTradesByTradeType(tradeType);

            foreach (var trade in tradesByType)
            {
                if (trade.TradeStatus == TradeStatus.Completed)
                {
                    this.tradeList.Remove(trade);

                    this.Log.Information(
                        $"Trade removed ({trade.TradeId}), "
                      + $"TradeStatus={trade.TradeStatus})");

                    Debug.CollectionDoesNotContain(trade, nameof(trade), this.tradeList.ToList().AsReadOnly());
                }
            }
        }

        /// <summary>
        /// Adds the given trade to the trade book.
        /// </summary>
        /// <param name="trade">The trade.</param>
        /// <exception cref="ValidationException">Throws if the trade is null.</exception>
        public void AddTrade(Trade trade)
        {
            Debug.NotNull(trade, nameof(trade));

            this.tradeList.Add(trade);

            this.Log.Information(
                $"Trade added ({trade.TradeId}), "
              + $"TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition})");
        }

        /// <summary>
        /// Returns a list of all trades matching the given trade type.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <returns>A <see cref="Trade"/>.</returns>
        /// <exception cref="ValidationException">Throws if the trade type is null.</exception>
        public ReadOnlyList<Trade> GetTradesByTradeType(TradeType tradeType)
        {
            Validate.NotNull(tradeType, nameof(tradeType));

            return new ReadOnlyList<Trade>(this.tradeList
               .Where(t => t.TradeType.Equals(tradeType))
               .ToList());
        }

        /// <summary>
        /// Returns the trade containing the order which matches the given order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="Trade"/>.</returns>
        /// <exception cref="ValidationException">Throws if the order identifier is null.</exception>
        public QueryResult<Trade> GetTradeForOrder(EntityId orderId)
        {
            Debug.NotNull(orderId, nameof(orderId));

            var trade = this.tradeList
               .FirstOrDefault(t => t.OrderIdList.Contains(orderId));

            return trade != null
                ? QueryResult<Trade>.Ok(trade)
                : QueryResult<Trade>.Fail("Could not find trade matching the given order identifier");
        }

        /// <summary>
        /// Returns a list of all active order identifiers.
        /// </summary>
        /// <returns>A <see cref="IList{EntityId}"/>.</returns>
        public ReadOnlyList<EntityId> GetAllActiveOrderIds()
        {
            return new ReadOnlyList<EntityId>(this.tradeList
               .SelectMany(trade => trade.OrderIdList)
               .ToList());
        }
    }
}
