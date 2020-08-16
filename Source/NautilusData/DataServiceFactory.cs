//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceFactory.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Common.Componentry;
using Nautilus.Common.Configuration;
using Nautilus.Common.Data;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Common.Messaging;
using Nautilus.Core.Correctness;
using Nautilus.Data;
using Nautilus.Data.Network;
using Nautilus.Data.Providers;
using Nautilus.Fix;
using Nautilus.Fxcm;
using Nautilus.Network.Compression;
using Nautilus.Redis.Data;
using Nautilus.Scheduling;
using Nautilus.Serialization.DataSerializers;
using Nautilus.Serialization.MessageSerializers;
using NodaTime;
using StackExchange.Redis;

namespace NautilusData
{
    /// <summary>
    /// Provides a factory for creating the <see cref="DataService"/>.
    /// </summary>
    public static class DataServiceFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="DataService"/>.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The service.</returns>
        public static DataService Create(ServiceConfiguration config)
        {
            VersionChecker.Run(
                config.LoggerFactory.CreateLogger(nameof(DataService)),
                "NAUTILUS DATA - Algorithmic Trading Data Service");

            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                config.LoggerFactory);

            var messagingAdapter = MessageBusFactory.Create(container);
            messagingAdapter.Start();

            var dataBusAdapter = DataBusFactory.Create(container);
            dataBusAdapter.Start();

            var scheduler = new Scheduler(container, messagingAdapter);
            scheduler.Start();

            var quoteSerializer = new QuoteTickSerializer();
            var tradeSerializer = new TradeTickSerializer();
            var barDataSerializer = new BarSerializer();
            var instrumentDataSerializer = new InstrumentSerializer();
            var headerSerializer = new MsgPackDictionarySerializer();
            var requestSerializer = new MsgPackRequestSerializer();
            var responseSerializer = new MsgPackResponseSerializer();
            var compressor = CompressorFactory.Create(config.WireConfig.CompressionCodec);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfig);

            var dataGateway = FixDataGatewayFactory.Create(
                container,
                dataBusAdapter,
                fixClient);

            var connection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");

            var marketDataRepository = new RedisMarketDataRepository(
                container,
                dataBusAdapter,
                connection,
                config.DataConfig.RetentionTimeTicksDays,
                config.DataConfig.RetentionTimeBarsDays);

            var tickProvider = new TickProvider(
                container,
                messagingAdapter,
                marketDataRepository,
                quoteSerializer,
                tradeSerializer);

            var barProvider = new BarProvider(
                container,
                messagingAdapter,
                marketDataRepository,
                barDataSerializer);

            var instrumentRepository = new RedisInstrumentRepository(
                container,
                dataBusAdapter,
                instrumentDataSerializer,
                connection);
            instrumentRepository.CacheAll();

            var instrumentProvider = new InstrumentProvider(
                container,
                messagingAdapter,
                instrumentRepository,
                instrumentDataSerializer);

            var dataServer = new DataServer(
                container,
                messagingAdapter,
                headerSerializer,
                requestSerializer,
                responseSerializer,
                compressor,
                config.WireConfig.EncryptionConfig,
                config.WireConfig.ServiceName,
                config.NetworkConfig.DataReqPort,
                config.NetworkConfig.DataResPort);

            var dataPublisher = new DataPublisher(
                container,
                dataBusAdapter,
                instrumentDataSerializer,
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.DataPubPort);

            var tickPublisher = new TickPublisher(
                container,
                dataBusAdapter,
                quoteSerializer,
                tradeSerializer,
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.TickPubPort);

            var dataService = new DataService(
                container,
                messagingAdapter,
                dataBusAdapter,
                dataGateway,
                config);

            var registrar = new ComponentAddressRegistrar();

            registrar.Register(ComponentAddress.Scheduler, scheduler);
            registrar.Register(ComponentAddress.DataGateway, dataGateway);
            registrar.Register(ComponentAddress.DataServer, dataServer);
            registrar.Register(ComponentAddress.DataPublisher, dataPublisher);
            registrar.Register(ComponentAddress.MarketDataRepository, marketDataRepository);
            registrar.Register(ComponentAddress.TickProvider, tickProvider);
            registrar.Register(ComponentAddress.TickPublisher, tickPublisher);
            registrar.Register(ComponentAddress.BarProvider, barProvider);
            registrar.Register(ComponentAddress.InstrumentRepository, instrumentRepository);
            registrar.Register(ComponentAddress.InstrumentProvider, instrumentProvider);
            registrar.Register(ComponentAddress.DataService, dataService);

            messagingAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(registrar.GetAddressBook()),
                guidFactory.Generate(),
                clock.TimeNow()));

            return dataService;
        }

        private static IFixClient CreateFixClient(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            FixConfiguration configuration)
        {
            switch (configuration.Broker.Value)
            {
                case "FXCM":
                    return FxcmFixClientFactory.Create(
                        container,
                        messagingAdapter,
                        configuration);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(
                        configuration.Broker,
                        nameof(configuration.Broker));
            }
        }
    }
}
