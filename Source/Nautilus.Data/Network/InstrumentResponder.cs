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
    using System.Linq;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.DomainModel.Entities;
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

        private void OnMessage(byte[] requestBytes)
        {
            try
            {
                var request = this.requestSerializer.Deserialize(requestBytes);

                switch (request)
                {
                    case InstrumentRequest req:
                        this.HandleRequest(req);
                        break;
                    case InstrumentsRequest req:
                        this.HandleRequest(req);
                        break;
                    default:
                    {
                        var message = $"request type {request.GetType()} not valid on this port";
                        this.SendInvalidResponse(message);
                        this.Log.Error(message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SendInvalidResponse(ex.Message);
                this.Log.Error(ex.Message);
            }
        }

        private void HandleRequest(InstrumentRequest request)
        {
            var query = this.repository.FindInCache(request.Symbol);

            if (query.IsSuccess)
            {
                this.SendResponse(this.instrumentSerializer.Serialize(query.Value));
                return;
            }

            this.SendInvalidResponse(query.Message);
            this.Log.Error(query.Message);
        }

        private void HandleRequest(InstrumentsRequest request)
        {
            var query = this.repository.FindInCache(request.Venue);

            if (query.IsSuccess)
            {
                var serialized = query.Value.Select(i => this.instrumentSerializer.Serialize(i)).ToList();
                this.SendResponse(serialized);
                return;
            }

            this.SendInvalidResponse(query.Message);
            this.Log.Error(query.Message);
        }

        private void SendInvalidResponse(string message)
        {
            this.SendResponse(Encoding.UTF8.GetBytes(INVALID + $" ({message})."));
        }
    }
}
