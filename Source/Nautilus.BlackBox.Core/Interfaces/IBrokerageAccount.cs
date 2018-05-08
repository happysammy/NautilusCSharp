//--------------------------------------------------------------------------------------------------
// <copyright file="IBrokerageAccount.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.CQS;

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
