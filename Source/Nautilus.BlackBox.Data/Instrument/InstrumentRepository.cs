//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Instrument
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Akka.Util.Internal;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="InstrumentRepository"/> class. A repository to hold a collection of
    /// <see cref="DomainModel.Entities.Instrument"/>(s).
    /// </summary>
    internal sealed class InstrumentRepository : IInstrumentRepository
    {
        private const string ErrorMsgDatabase = "The repository is not connected to the database";

        private readonly IZonedClock clock;
        private readonly IDatabaseAdapter database;
        private readonly IDictionary<Symbol, Instrument> instrumentIndex = new Dictionary<Symbol, Instrument>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentRepository"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="database">The database adapter.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public InstrumentRepository(
            IZonedClock clock,
            IDatabaseAdapter database)
        {
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(database, nameof(database));

            this.clock = clock;
            this.database = database;

            database.OpenConnection();
            this.LoadAllInstrumentsFromDatabase();
        }

        /// <summary>
        /// Gets the instrument symbol list.
        /// </summary>
        public IReadOnlyCollection<Symbol> InstrumentSymbolList => this.instrumentIndex.Keys.ToImmutableList();

        /// <summary>
        /// Gets the document store count.
        /// </summary>
        public int DocumentStoreCount => this.database.Query<Instrument>().ToList().Count;

        /// <summary>
        /// Loads all instruments from the database.
        /// </summary>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public CommandResult LoadAllInstrumentsFromDatabase()
        {
//            var instruments = this.database.Query<Instrument>().ToList();
//
//            foreach (var instrument in instruments)
//            {
//                if (!this.instrumentIndex.ContainsKey(instrument.Symbol))
//                {
//                    this.instrumentIndex.Add(instrument.Symbol, instrument);
//                }
//            }

            return CommandResult.Ok($"All instruments loaded from database [{this.database}.Instruments] (count = {0})");
        }

        /// <summary>
        /// The delete all instruments from database.
        /// </summary>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public CommandResult DeleteAll()
        {
            this.database
               .Query<Instrument>()
               .ToList()
               .ForEach(i => this.database.Delete(i));

            this.SaveChanges();

            Task.Delay(500).Wait();

            var instrumentsRemaining = this.database
               .Query<Instrument>()
               .ToList();

            if (instrumentsRemaining.Count > 0)
            {
                return CommandResult.Fail("Could not delete all instruments from the database");
            }

            return CommandResult.Ok($"All instruments deleted from database [{this.database}.Instruments]");
        }

        /// <summary>
        /// The update instrument.
        /// </summary>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public CommandResult Add(Instrument instrument)
        {
            Validate.NotNull(instrument, nameof(instrument));

            var result = this.StoreInstrument(instrument);

            if (result.IsFailure)
            {
                return result;
            }

            return this.SaveChanges().IsSuccess
                       ? CommandResult.Ok($"Instruments updated in database [{this.database}.Instruments]")
                       : CommandResult.Fail("Could not update instrument");
        }

        /// <summary>
        /// Updates the given list of <see cref="Instrument"/>(s) in the database.
        /// </summary>
        /// <param name="instruments">The instruments (cannot be null or empty).</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public CommandResult Add(IReadOnlyCollection<Instrument> instruments)
        {
            Validate.NotNull(instruments, nameof(instruments));

            foreach (var instrument in instruments)
            {
                var result = this.StoreInstrument(instrument);

                if (result.IsFailure)
                {
                    return result;
                }
            }

            return this.SaveChanges().IsSuccess
                       ? CommandResult.Ok($"All instruments updated in database [{this.database}.Instruments]")
                       : CommandResult.Fail("Could not update instruments");
        }

        /// <summary>
        /// Returns the instrument from the repository if contained (otherwise returns a failed
        /// query).
        /// </summary>
        /// <param name="symbol">The instrument symbol.</param>
        /// <returns>A <see cref="QueryResult{Instrument}"/>.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public QueryResult<Instrument> GetInstrument(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            return this.instrumentIndex.ContainsKey(symbol)
                ? QueryResult<Instrument>.Ok(this.instrumentIndex[symbol])
                : QueryResult<Instrument>.Fail($"{nameof(InstrumentRepository)} cannot find symbol {symbol}");
        }

        /// <summary>
        /// Returns the tick size of the given <see cref="Symbol"/> (if not contained returns a failed
        /// query).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="QueryResult{Decimal}"/>.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public QueryResult<decimal> GetTickSize(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            return this.instrumentIndex.ContainsKey(symbol)
                ? QueryResult<decimal>.Ok(this.instrumentIndex[symbol].TickSize)
                : QueryResult<decimal>.Fail($"{nameof(InstrumentRepository)} cannot find symbol {symbol}");
        }

        /// <summary>
        /// Disposes the <see cref="InstrumentRepository"/>.
        /// </summary>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public CommandResult Dispose()
        {
            this.database.Dispose();
            //GC.SuppressFinalize(this);

            return CommandResult.Ok($"{nameof(InstrumentRepository)} disposed");
        }

        private CommandResult StoreInstrument(Instrument instrument)
        {
            Debug.NotNull(instrument, nameof(instrument));
            Debug.NotNull(this.database, nameof(this.database));

            var symbol = instrument.Symbol;

            if (!this.instrumentIndex.ContainsKey(symbol))
            {
                this.instrumentIndex.Add(symbol, instrument);
                this.database.Store(instrument);

                return CommandResult.Ok($"Added instrument ({instrument})...");
            }

            var instrumentBuilder = new InstrumentBuilder(this.instrumentIndex[symbol]).Update(instrument);

            if (instrumentBuilder.Changes.Count == 0)
            {
                return CommandResult.Ok();
            }

            var changesString = new StringBuilder();

            instrumentBuilder.Changes.ForEach(c => changesString.Append(c));

            var updatedInstrument = instrumentBuilder.Build(this.clock.TimeNow());

            this.database.Delete(this.instrumentIndex[symbol]);
            this.instrumentIndex.Remove(symbol);

            this.instrumentIndex.Add(symbol, updatedInstrument);
            this.database.Store(updatedInstrument);

            return CommandResult.Ok($"Instrument {symbol} updated" + changesString);
        }

        private CommandResult SaveChanges()
        {
            this.database.SaveChanges();

            return CommandResult.Ok();
        }
    }
}
