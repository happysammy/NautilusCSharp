//--------------------------------------------------------------------------------------------------
// <copyright file="AlgorithmHydrator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit
{
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Interfaces;

    /// <summary>
    /// The algorithm hydrator.
    /// </summary>
    public static class AlgorithmHydrator
    {
        /// <summary>
        /// The hydrate.
        /// </summary>
        /// <param name="entryStopAlgorithm">
        /// The entry stop algorithm.
        /// </param>
        /// <param name="barStore">
        /// The bar store.
        /// </param>
        public static void Hydrate(IEntryStopAlgorithm entryStopAlgorithm, IBarStore barStore)
        {
            for (int i = barStore.Count - 1; i >= 0; i--)
            {
                entryStopAlgorithm.Update(barStore.GetBar(i));
            }
        }

        /// <summary>
        /// The hydrate.
        /// </summary>
        /// <param name="entryStopAlgorithm">
        /// The entry stop algorithm.
        /// </param>
        /// <param name="barStore">
        /// The bar store.
        /// </param>
        public static void Hydrate(IEntryAlgorithm entryStopAlgorithm, IBarStore barStore)
        {
            for (int i = barStore.Count - 1; i >= 0; i--)
            {
                entryStopAlgorithm.Update(barStore.GetBar(i));
            }
        }

        /// <summary>
        /// The hydrate.
        /// </summary>
        /// <param name="exitAlgorithm">
        /// The entry stop algorithm.
        /// </param>
        /// <param name="barStore">
        /// The bar store.
        /// </param>
        public static void Hydrate(IExitAlgorithm exitAlgorithm, IBarStore barStore)
        {
            for (int i = barStore.Count - 1; i >= 0; i--)
            {
                exitAlgorithm.Update(barStore.GetBar(i));
            }
        }

        /// <summary>
        /// The hydrate.
        /// </summary>
        /// <param name="stopLossAlgorithm">
        /// The entry stop algorithm.
        /// </param>
        /// <param name="barStore">
        /// The bar store.
        /// </param>
        public static void Hydrate(IStopLossAlgorithm stopLossAlgorithm, IBarStore barStore)
        {
            for (int i = barStore.Count - 1; i >= 0; i--)
            {
                stopLossAlgorithm.Update(barStore.GetBar(i));
            }
        }

        /// <summary>
        /// The hydrate.
        /// </summary>
        /// <param name="trailingStopAlgorithm">
        /// The trailing stop algorithm.
        /// </param>
        /// <param name="barStore">
        /// The bar store.
        /// </param>
        public static void Hydrate(ITrailingStopAlgorithm trailingStopAlgorithm, IBarStore barStore)
        {
            for (int i = barStore.Count - 1; i >= 0; i--)
            {
                trailingStopAlgorithm.Update(barStore.GetBar(i));
            }
        }

        /// <summary>
        /// The hydrate.
        /// </summary>
        /// <param name="entrySignalGenerator">
        /// The entry signal generator.
        /// </param>
        /// <param name="barStore">
        /// The bar store.
        /// </param>
        public static void Hydrate(EntrySignalGenerator entrySignalGenerator, IBarStore barStore)
        {
            for (int i = barStore.Count - 1; i >= 0; i--)
            {
                entrySignalGenerator.Update(barStore.GetBar(i));
            }
        }

        /// <summary>
        /// The hydrate.
        /// </summary>
        /// <param name="exitSignalGenerator">
        /// The exit signal generator.
        /// </param>
        /// <param name="barStore">
        /// The bar store.
        /// </param>
        public static void Hydrate(ExitSignalGenerator exitSignalGenerator, IBarStore barStore)
        {
            for (int i = barStore.Count - 1; i >= 0; i--)
            {
                exitSignalGenerator.Update(barStore.GetBar(i));
            }
        }

        /// <summary>
        /// The hydrate.
        /// </summary>
        /// <param name="trailingStopSignalGenerator">
        /// The trailing stop signal generator.
        /// </param>
        /// <param name="barStore">
        /// The bar store.
        /// </param>
        public static void Hydrate(TrailingStopSignalGenerator trailingStopSignalGenerator, IBarStore barStore)
        {
            for (int i = barStore.Count - 1; i >= 0; i--)
            {
                trailingStopSignalGenerator.Update(barStore.GetBar(i));
            }
        }
    }
}
