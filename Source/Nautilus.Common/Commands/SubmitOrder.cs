namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Common.Commands.Base;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Aggregates;
    using NodaTime;

    /// <summary>
    /// Represents a command to submit an order to the execution system.
    /// </summary>
    [Immutable]
    public sealed class SubmitOrder : OrderCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitOrder"/> class.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="commandId"></param>
        /// <param name="commandTimestamp"></param>
        public SubmitOrder(
            Order order,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(order.Symbol, order.Id, commandId, commandTimestamp)
        {
        }
    }
}
