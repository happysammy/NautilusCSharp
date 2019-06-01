//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentResponder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Network
{
    using System;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;

    /// <summary>
    /// Provides a responder for <see cref="Instrument"/> data requests.
    /// </summary>
    public class InstrumentResponder : Responder
    {
        private const string INVALID = "INVALID REQUEST";

        private readonly IInstrumentRepository repository;
        private readonly IInstrumentSerializer instrumentSerializer;
        private readonly IRequestSerializer requestSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentResponder"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="instrumentSerializer">The instrument serializer.</param>
        /// <param name="requestSerializer">The request serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public InstrumentResponder(
            IComponentryContainer container,
            IInstrumentRepository repository,
            IInstrumentSerializer instrumentSerializer,
            IRequestSerializer requestSerializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                host,
                port,
                Guid.NewGuid())
        {
            this.repository = repository;
            this.instrumentSerializer = instrumentSerializer;
            this.requestSerializer = requestSerializer;

            this.RegisterHandler<byte[]>(this.OnMessage);
        }

        private static QueryResult<Symbol> ParseSymbol(string symbol)
        {
            try
            {
                var symbolSplit = symbol.Split(".");
                return QueryResult<Symbol>.Ok(new Symbol(symbolSplit[0], symbolSplit[1].ToEnum<Venue>()));
            }
            catch (Exception ex)
            {
                return QueryResult<Symbol>.Fail(ex.Message);
            }
        }

        private void OnMessage(byte[] request)
        {
            var requestString = Encoding.UTF8.GetString(request);

            var symbolQuery = ParseSymbol(requestString);
            if (symbolQuery.IsFailure)
            {
                this.Log.Error(symbolQuery.Message);
                this.SendResponse(Encoding.UTF8.GetBytes(INVALID + $" ({symbolQuery.Message})."));
                return;
            }

            var query = this.repository.FindInCache(symbolQuery.Value);

            if (query.IsFailure)
            {
                this.Log.Error(query.Message);
                this.SendResponse(Encoding.UTF8.GetBytes(INVALID + $" ({query.Message})."));
                return;
            }

            this.SendResponse(this.instrumentSerializer.Serialize(query.Value));
        }
    }
}
