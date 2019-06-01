//--------------------------------------------------------------------------------------------------
// <copyright file="TickResponder.cs" company="Nautech Systems Pty Ltd">
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
    public sealed class TickResponder : Responder
    {
        private const string INVALID = "INVALID REQUEST";

        private readonly ITickRepository repository;
        private readonly IRequestSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickResponder"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The tick repository.</param>
        /// <param name="serializer">The request serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public TickResponder(
            IComponentryContainer container,
            ITickRepository repository,
            IRequestSerializer serializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                host,
                port,
                Guid.NewGuid())
        {
            this.repository = repository;
            this.serializer = serializer;

            this.RegisterHandler<byte[]>(this.OnMessage);
        }

        private void OnMessage(byte[] requestBytes)
        {
            try
            {
                var request = this.serializer.Deserialize(requestBytes);

                if (!(request is TickDataRequest))
                {
                    var message = $"request type {request.GetType()} not valid on this port";
                    this.SendInvalidResponse(message);
                    this.Log.Error(message);
                }

                var dataRequest = (TickDataRequest)request;
                var query = this.repository.Find(
                    dataRequest.Symbol,
                    dataRequest.FromDateTime,
                    dataRequest.ToDateTime);

                if (query.IsFailure)
                {
                    this.SendInvalidResponse(query.Message);
                    this.Log.Error(query.Message);
                }

                var ticksList = query
                    .Value
                    .Select(t => Encoding.UTF8.GetBytes(t.ToString()))
                    .ToList();

                this.SendResponse(ticksList);
            }
            catch (Exception ex)
            {
                this.SendInvalidResponse(ex.Message);
                this.Log.Error(ex.Message);
            }
        }

        private void SendInvalidResponse(string message)
        {
            this.SendResponse(Encoding.UTF8.GetBytes(INVALID + $" ({message})."));
        }
    }
}
