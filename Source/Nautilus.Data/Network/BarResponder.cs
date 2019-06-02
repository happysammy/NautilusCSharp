//--------------------------------------------------------------------------------------------------
// <copyright file="BarResponder.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;

    /// <summary>
    /// Provides a responder for <see cref="Instrument"/> data requests.
    /// </summary>
    public sealed class BarResponder : Responder
    {
        private readonly IBarRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarResponder"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="requestSerializer">The request serializer.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public BarResponder(
            IComponentryContainer container,
            IBarRepository repository,
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

            this.RegisterHandler<BarDataRequest>(this.OnMessage);
            this.RegisterUnhandled(this.UnhandledRequest);
        }

        private void OnMessage(BarDataRequest request)
        {
            var barType = new BarType(request.Symbol, request.BarSpecification);
            var query = this.repository.Find(
                barType,
                request.FromDateTime,
                request.ToDateTime);

            if (query.IsFailure)
            {
                this.SendBadResponse(query.Message);
                this.Log.Error(query.Message);
            }

            var bars = query
                .Value
                .Bars
                .Select(b => Encoding.UTF8.GetBytes(b.ToString()))
                .ToArray();

            var response = new BarDataResponse(
                request.Symbol,
                request.BarSpecification,
                bars,
                this.CorrelationId,
                this.NewGuid(),
                this.TimeNow());

            this.SendResponse(response);
        }
    }
}
