//--------------------------------------------------------------------------------------------------
// <copyright file="AlwaysExit.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms.Exit
{
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immediate close test (a test algorithm which immediately sends a close signal for any active positions).
    /// </summary>
    public sealed class AlwaysExit : ExitAlgorithmBase, IExitAlgorithm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlwaysExit"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="forUnit">
        /// The for units.
        /// </param>
        public AlwaysExit(
            TradeProfile tradeProfile,
            Instrument instrument,
            int forUnit)
            : base(new Label(nameof(AlwaysExit)), tradeProfile, instrument, forUnit)
        {
           Validate.NotNull(tradeProfile, nameof(tradeProfile));
           Validate.NotNull(instrument, nameof(instrument));
           Validate.Int32NotOutOfRange(forUnit, nameof(forUnit), 0, int.MaxValue);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="bar">
        /// The bar.
        /// </param>
        public void Update(Bar bar)
        {
        }

        /// <summary>
        /// The process signal long.
        /// </summary>
        /// <returns>
        /// The <see cref="IExitResponse"/>.
        /// </returns>
        public Option<IExitResponse> CalculateLong()
        {
            const bool IsSignal = true;

            return this.SignalResponseLong(IsSignal);
        }

        /// <summary>
        /// The process signal short.
        /// </summary>
        /// <returns>
        /// The <see cref="IExitResponse"/>.
        /// </returns>
        public Option<IExitResponse> CalculateShort()
        {
            const bool IsSignal = true;

            return this.SignalResponseShort(IsSignal);
        }
    }
}