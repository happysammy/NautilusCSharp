//--------------------------------------------------------------------------------------------------
// <copyright file="TickDealer.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Network;

    /// <summary>
    /// Provides a responder for <see cref="Instrument"/> data requests.
    /// </summary>
    public sealed class TickDealer : Dealer
    {
        private readonly ITickRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickDealer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The tick repository.</param>
        /// <param name="requestSerializer">The request serializer.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public TickDealer(
            IComponentryContainer container,
            ITickRepository repository,
            IRequestSerializer requestSerializer,
            IResponseSerializer responseSerializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                requestSerializer,
                responseSerializer,
                host,
                port,
                Guid.NewGuid())
        {
            this.repository = repository;

            this.RegisterHandler<TickDataRequest>(this.OnMessage);
        }

        private void OnMessage(TickDataRequest request)
        {
            var query = this.repository.Find(
                request.Symbol,
                request.FromDateTime,
                request.ToDateTime);

            if (query.IsFailure)
            {
                this.SendBadRequest(request, query.Message);
                this.Log.Error(query.Message);
            }

            var ticks = query
                .Value
                .Select(t => Encoding.UTF8.GetBytes(t.ToString()))
                .ToArray();

            var response = new TickDataResponse(
                request.Symbol,
                ticks,
                request.Id,
                this.NewGuid(),
                this.TimeNow());

            this.SendResponse(request.RequesterId, response);
        }
    }
}
