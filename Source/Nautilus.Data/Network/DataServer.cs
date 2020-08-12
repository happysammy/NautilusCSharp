//--------------------------------------------------------------------------------------------------
// <copyright file="DataServer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
using Nautilus.Core.Message;
using Nautilus.Core.Types;
using Nautilus.Data.Messages.Requests;
using Nautilus.Data.Messages.Responses;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Network;
using Nautilus.Network.Encryption;
using Nautilus.Network.Messages;
using Nautilus.Network.Nodes;

namespace Nautilus.Data.Network
{
    /// <summary>
    /// Provides a data server to receive requests and send responses.
    /// </summary>
    public sealed class DataServer : MessageServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="headerSerializer">The header serializer.</param>
        /// <param name="requestSerializer">The inbound message serializer.</param>
        /// <param name="responseSerializer">The outbound message serializer.</param>
        /// <param name="compressor">The data compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="serviceName">The service name.</param>
        /// <param name="requestPort">The inbound port.</param>
        /// <param name="responsePort">The outbound port.</param>
        public DataServer(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            ISerializer<Dictionary<string, string>> headerSerializer,
            IMessageSerializer<Request> requestSerializer,
            IMessageSerializer<Response> responseSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            Label serviceName,
            Port requestPort,
            Port responsePort)
            : base(
                container,
                messagingAdapter,
                headerSerializer,
                requestSerializer,
                responseSerializer,
                compressor,
                encryption,
                serviceName,
                ZmqNetworkAddress.AllInterfaces(requestPort),
                ZmqNetworkAddress.AllInterfaces(responsePort))
        {
            this.RegisterHandler<DataRequest>(this.OnMessage);
            this.RegisterHandler<DataResponse>(this.OnMessage);
            this.RegisterHandler<QueryFailure>(this.OnMessage);
        }

        private void OnMessage(DataRequest request)
        {
            try
            {
                this.Logger.LogInformation(LogId.Network,$"<--[REQ] {request}.");

                var dataType = request.Query["DataType"];
                if (dataType == typeof(Tick[]).Name)
                {
                    this.Send(request, ComponentAddress.TickProvider);
                }
                else if (dataType == typeof(Bar[]).Name)
                {
                    this.Send(request, ComponentAddress.BarProvider);
                }
                else if (dataType == typeof(Instrument[]).Name)
                {
                    this.Send(request, ComponentAddress.InstrumentProvider);
                }
                else
                {
                    this.SendRejected(
                        $"Could not process DataRequest, " +
                        $"incorrect DataType requested, was {dataType}",
                        request.Id);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Network, $"{ex}");
                this.SendQueryFailure(ex.Message, request.Id);
            }
        }

        private void OnMessage(DataResponse response)
        {
            this.SendMessage(response);

            this.Logger.LogInformation(LogId.Network,$"[RES]--> {response}.");
        }

        private void OnMessage(QueryFailure response)
        {
            this.SendMessage(response);

            this.Logger.LogInformation(LogId.Network,$"[RES]--> {response}.");
        }
    }
}
