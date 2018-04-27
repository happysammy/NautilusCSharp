// -------------------------------------------------------------------------------------------------
// <copyright file="IBrokerageAccount.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.CQS;
    using Nautilus.Core;
    using Nautilus.DomainModel;

    /// <summary>
    /// The <see cref="IBrokerageAccount"/> interface. Represents a generic brokerage account.
    /// </summary>
    public interface IBrokerageAccount : IReadOnlyBrokerageAccount
    {
        /// <summary>
        /// Gets the accounts event count.
        /// </summary>
        int EventCount { get; }

        /// <summary>
        /// Applies the event to the brokerage account.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        CommandResult Apply(Event @event);
    }
}
