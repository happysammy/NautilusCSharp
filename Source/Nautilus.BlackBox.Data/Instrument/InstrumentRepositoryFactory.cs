// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentRepositoryFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Instrument
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;

    /// <summary>
    /// The immutable static <see cref="InstrumentRepositoryFactory"/> class. Provides
    /// <see cref="IInstrumentRepository"/>(s) for the <see cref="BlackBox"/> system.
    /// </summary>
    [Immutable]
    public static class InstrumentRepositoryFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="IInstrumentRepository"/> based on the given inputs.
        /// </summary>
        /// <param name="clock">The system clock.</param>
        /// <param name="databaseAdapter">The database adapter.</param>
        /// <returns>A <see cref="IInstrumentRepository"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static IInstrumentRepository Create(
            IZonedClock clock,
            IDatabaseAdapter databaseAdapter)
        {
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(databaseAdapter, nameof(databaseAdapter));

            return new InstrumentRepository(
                clock,
                databaseAdapter);
        }
    }
}
