//--------------------------------------------------------------------------------------------------
// <copyright file="ISignalLogic.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System.Collections.Generic;
    using NautechSystems.CSharp;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The <see cref="ISignalLogic"/> interface. Contains logic to determine the validity of entry
    /// signals.
    /// </summary>
    public interface ISignalLogic
    {
        /// <summary>
        /// Returns a value indicating whether a buy signal is valid with the given inputs.
        /// </summary>
        /// <param name="entrySignalsBuy">The buy entry signals.</param>
        /// <param name="entrySignalsSell">The sell entry signals.</param>
        /// <param name="exitSignalLong">The long exit signal (optional).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsValidBuySignal(
            IReadOnlyCollection<EntrySignal> entrySignalsBuy,
            IReadOnlyCollection<EntrySignal> entrySignalsSell,
            Option<ExitSignal> exitSignalLong);

        /// <summary>
        /// Returns a value indicating whether a sell signal is valid with the given inputs.
        /// </summary>
        /// <param name="entrySignalsBuy">The buy entry signals.</param>
        /// <param name="entrySignalsSell">The sell entry signals.</param>
        /// <param name="exitSignalShort">The short exit signal (optional).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsValidSellSignal(
            IReadOnlyCollection<EntrySignal> entrySignalsBuy,
            IReadOnlyCollection<EntrySignal> entrySignalsSell,
            Option<ExitSignal> exitSignalShort);
    }
}