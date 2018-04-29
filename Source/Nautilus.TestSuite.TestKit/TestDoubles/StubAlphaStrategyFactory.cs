//--------------------------------------------------------------------------------------------------
// <copyright file="StubAlphaStrategyFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Algorithms.Entry;
    using Nautilus.Algorithms.EntryStop;
    using Nautilus.Algorithms.Exit;
    using Nautilus.Algorithms.ProfitTarget;
    using Nautilus.Algorithms.StopLoss;
    using Nautilus.Algorithms.TrailingStop;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Interfaces;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubAlphaStrategyFactory
    {
        public static AlphaStrategy Create()
        {
            var instrument = StubInstrumentFactory.AUDUSD();
            const int tradePeriod = 20;
            var tradeProfile = StubTradeProfileFactory.Create(tradePeriod);
            var signalLogicFsm = new SignalLogic(false, false);

            var entryStopAlgorithm = new BarStretchStop(tradeProfile, instrument);
            var stoplossAlgorithm = new BarsBackWithBufferStop(tradeProfile, instrument, 3, 2.0m);
            var profitTargetAlgorithm = new RiskMultiplesTarget(tradeProfile, instrument, tradeProfile.Units - 1, 1.0m);

            var entryAlgorithms = new List<IEntryAlgorithm> { new CloseDirectionEntry(tradeProfile, instrument, entryStopAlgorithm) };
            var trailingStopAlgorithms = new List<ITrailingStopAlgorithm> { new BarsBackTrail(tradeProfile, instrument, 1, 0) };
            var exitAlgorithms = new List<IExitAlgorithm> { new AlwaysExit(tradeProfile, instrument, 0) };

            return new AlphaStrategy(
                instrument,
                tradeProfile,
                signalLogicFsm,
                entryStopAlgorithm,
                stoplossAlgorithm,
                profitTargetAlgorithm,
                entryAlgorithms,
                trailingStopAlgorithms,
                exitAlgorithms);
        }
    }
}
