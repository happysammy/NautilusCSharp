//--------------------------------------------------------------------------------------------------
// <copyright file="TickPublisher.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Common.Interfaces;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Network;
using Nautilus.Network.Encryption;
using Nautilus.Network.Nodes;

namespace Nautilus.Data.Network
{
    /// <summary>
    /// Provides a publisher for <see cref="Tick"/> data.
    /// </summary>
    public sealed class TickPublisher : PublisherDataBus
    {
        private const string Quote = nameof(Quote);
        private const string Trade = nameof(Trade);

        private readonly IDataSerializer<QuoteTick> quoteSerializer;
        private readonly IDataSerializer<TradeTick> tradeSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickPublisher"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="quoteSerializer">The quote tick serializer.</param>
        /// <param name="tradeSerializer">The trade tick serializer.</param>
        /// <param name="compressor">The data compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The publisher port.</param>
        public TickPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<QuoteTick> quoteSerializer,
            IDataSerializer<TradeTick> tradeSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            Port port)
            : base(
                container,
                dataBusAdapter,
                compressor,
                encryption,
                ZmqNetworkAddress.AllInterfaces(port))
        {
            this.quoteSerializer = quoteSerializer;
            this.tradeSerializer = tradeSerializer;

            this.RegisterHandler<QuoteTick>(this.OnMessage);
            this.RegisterHandler<TradeTick>(this.OnMessage);

            this.Subscribe<QuoteTick>();
            this.Subscribe<TradeTick>();
        }

        private void OnMessage(QuoteTick tick)
        {
            this.Publish($"{Quote}:{tick.Symbol.Value}", this.quoteSerializer.Serialize(tick));
        }

        private void OnMessage(TradeTick tick)
        {
            this.Publish($"{Trade}:{tick.Symbol.Value}", this.tradeSerializer.Serialize(tick));
        }
    }
}
