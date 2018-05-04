//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using QuickFix.Transport;

    using Message = QuickFix.Message;
    using Price = DomainModel.ValueObjects.Price;
    using Symbol = DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// Provides a generic QuickFix client.
    /// </summary>
    public class FixClient : MessageCracker, IApplication, IBrokerageClient
    {
        private readonly string username;
        private readonly string password;
        private readonly string accountNumber;
        private readonly IList<Symbol> marketDataSubscriptions = new List<Symbol>();
        private readonly FxcmFixMessageHandler messageHandler;
        private readonly FxcmFixMessageRouter messageRouter;

        private IBrokerageGateway brokerageGateway;
        private SocketInitiator initiator;
        private Session session;
        private Session sessionMd;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixClient"/> class.
        /// </summary>
        /// <param name="broker">The account brokerage.</param>
        /// <param name="username">The account username.</param>
        /// <param name="password">The account password.</param>
        /// <param name="accountNumber">The account number.</param>
        public FixClient(
            Broker broker,
            string username,
            string password,
            string accountNumber)
        {
            Validate.NotNull(username, nameof(username));
            Validate.NotNull(password, nameof(password));
            Validate.NotNull(accountNumber, nameof(accountNumber));

            this.Broker = broker;
            this.username = username;
            this.password = password;
            this.accountNumber = accountNumber;

            this.messageHandler = new FxcmFixMessageHandler();
            this.messageRouter = new FxcmFixMessageRouter();

            foreach (var symbol in ConversionRateCurrencySymbols.GetList())
            {
                if (!this.marketDataSubscriptions.Contains(symbol))
                {
                    this.marketDataSubscriptions.Add(symbol);
                }
            }
        }

        /// <summary>
        /// The broker.
        /// </summary>
        public Broker Broker { get; }

        /// <summary>
        /// The is connected.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsConnected => this.session.IsLoggedOn;

        /// <summary>
        /// The initialize brokerage gateway.
        /// </summary>
        /// <param name="gateway">
        /// The gateway.
        /// </param>
        public void InitializeBrokerageGateway(IBrokerageGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.brokerageGateway = gateway;
            this.messageHandler.InitializeBrokerageGateway(gateway);
            this.messageRouter.InitializeBrokerageGateway(gateway);
        }

        /// <summary>
        /// The connect.
        /// </summary>
        public void Connect()
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                Validate.NotNull(this.brokerageGateway, nameof(this.brokerageGateway));

                var settings = new SessionSettings("fix_fxcm.cfg");
                var storeFactory = new FileStoreFactory(settings);
                var logFactory = new ScreenLogFactory(settings);
                this.initiator = new SocketInitiator(this, storeFactory, settings, null);

                this.initiator.Start();

                Console.WriteLine($"BrokerClient {this}");
                //Console.WriteLine($"QuickFix (version {Assembly.LoadFrom("QuickFix.dll").GetName().Version})");
                Console.WriteLine($"Starting initiator... ");
            });
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                Validate.NotNull(this.initiator, nameof(this.initiator));

                this.initiator.Stop();

                Console.WriteLine($"Stopping initiator... ");
            });
        }

        /// <summary>
        /// The on create.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        public void OnCreate(SessionID sessionId)
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                if (this.session == null)
                {
                    Console.WriteLine($"Creating session...");
                    this.session = Session.LookupSession(sessionId);
                    this.messageRouter.ConnectSession(this.session);
                    Console.WriteLine($"Session {this.session}");
                }

                this.sessionMd = Session.LookupSession(sessionId);
                this.messageRouter.ConnectSessionMd(this.sessionMd);
            });
        }

        /// <summary>
        /// The on logon.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        public void OnLogon(SessionID sessionId)
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                Console.WriteLine($"Logon - {sessionId}");

                // allow logon to complete
                Task.Delay(100).Wait();

                if (sessionId.Equals(this.sessionMd.SessionID))
                {
                    return;
                }

                this.CollateralInquiry();
                this.TradingSessionStatus();
                this.RequestAllPositions();
                this.UpdateInstrumentsSubscribeAll();

                foreach (var symbol in this.marketDataSubscriptions)
                {
                    this.RequestMarketDataSubscribe(symbol);
                }
            });
        }

        /// <summary>
        /// The on logout.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        public void OnLogout(SessionID sessionId)
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                Console.WriteLine($"Logout - {sessionId}");
            });
        }

        /// <summary>
        /// The from admin.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        public void FromAdmin(Message message, SessionID sessionId)
        {
        }

        /// <summary>
        /// The to admin.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        public void ToAdmin(Message message, SessionID sessionId)
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));
                Validate.NotNull(sessionId, nameof(sessionId));

                if (message.GetType() == typeof(Logon))
                {
                    message.SetField(new Username(this.username));
                    message.SetField(new Password(this.password));

                    Console.WriteLine("Authorizing session...");
                }

                message.SetField(new Account(this.accountNumber));
            });
        }

        /// <summary>
        /// The from app.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        public void FromApp(Message message, SessionID sessionId)
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));
                Validate.NotNull(sessionId, nameof(sessionId));

                this.Crack(message, sessionId);
            });
        }

        /// <summary>
        /// The to app.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <exception cref="DoNotSend">
        /// Will not send
        /// </exception>
        public void ToApp(Message message, SessionID sessionId)
        {
            this.brokerageGateway.Command.Execute<FieldNotFoundException>(() =>
            {
                message.SetField(new Account(this.accountNumber));

                var possDupFlag = false;

                if (message.Header.IsSetField(Tags.PossDupFlag))
                {
                    possDupFlag = QuickFix.Fields.Converters.BoolConverter.Convert(message.Header.GetField(Tags.PossDupFlag)); // TODO (FIXME)??
                }

                if (possDupFlag)
                {
                    throw new DoNotSend();
                }
            });
        }

        /// <summary>
        /// The submit entry limit stop order.
        /// </summary>
        /// <param name="elsOrder">
        /// The ELS order.
        /// </param>
        public void SubmitEntryLimitStopOrder(AtomicOrder elsOrder)
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                this.messageRouter.SubmitEntryLimitStopOrder(elsOrder);
            });
        }

        /// <summary>
        /// The submit entry stop order.
        /// </summary>
        /// <param name="elsOrder">
        /// The ELS order.
        /// </param>
        public void SubmitEntryStopOrder(AtomicOrder elsOrder)
        {
            this.brokerageGateway.Command.Execute(() =>
            {
                this.messageRouter.SubmitEntryStopOrder(elsOrder);
            });
        }

        /// <summary>
        /// The modify stop-loss order.
        /// </summary>
        /// <param name="orderModification">
        /// The order modification.
        /// </param>
        public void ModifyStoplossOrder(KeyValuePair<Order, Price> orderModification)
        {
            this.messageRouter.ModifyStoplossOrder(orderModification);
        }

        /// <summary>
        /// The cancel order.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        public void CancelOrder(Order order)
        {
            this.messageRouter.CancelOrder(order);
        }

        /// <summary>
        /// The close position.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        public void ClosePosition(Position position)
        {
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session.
        /// </param>
        public void OnMessage(BusinessMessageReject message, SessionID sessionId)
        {
            this.messageHandler.OnBusinessMessageReject(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session.
        /// </param>
        public void OnMessage(TradingSessionStatus message, SessionID sessionId)
        {
            // TODO
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        public void OnMessage(SecurityList message, SessionID sessionId)
        {
            this.messageHandler.OnSecurityList(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        public void OnMessage(CollateralInquiryAck message, SessionID sessionId)
        {
            this.messageHandler.OnCollateralInquiryAck(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        public void OnMessage(CollateralReport message, SessionID sessionId)
        {
            this.messageHandler.OnCollateralReport(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        public void OnMessage(RequestForPositionsAck message, SessionID sessionId)
        {
            this.messageHandler.OnRequestForPositionsAck(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        public void OnMessage(MarketDataSnapshotFullRefresh message, SessionID sessionId)
        {
            this.messageHandler.OnMarketDataSnapshotFullRefresh(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        public void OnMessage(OrderCancelReject message, SessionID sessionId)
        {
            this.messageHandler.OnOrderCancelReject(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        public void OnMessage(ExecutionReport message, SessionID sessionId)
        {
            this.messageHandler.OnExecutionReport(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        public void OnMessage(PositionReport message, SessionID sessionId)
        {
            this.messageHandler.OnPositionReport(message);
        }

        /// <summary>
        /// The collateral inquiry.
        /// </summary>
        public void CollateralInquiry()
        {
            this.messageRouter.CollateralInquiry();
        }

        /// <summary>
        /// The trading session status.
        /// </summary>
        public void TradingSessionStatus()
        {
            this.messageRouter.TradingSessionStatus();
        }

        /// <summary>
        /// The request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            this.messageRouter.RequestAllPositions();
        }

        /// <summary>
        /// The update instrument subscribe.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            this.messageRouter.UpdateInstrumentSubscribe(symbol);
        }

        /// <summary>
        /// The update instruments subscribe all.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.messageRouter.UpdateInstrumentsSubscribeAll();
        }

        /// <summary>
        /// The request market data subscribe.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void RequestMarketDataSubscribe(Symbol symbol)
        {
            if (!this.marketDataSubscriptions.Contains(symbol))
            {
                this.marketDataSubscriptions.Add(symbol);
            }

            this.messageRouter.MarketDataRequestSubscribe(symbol);
        }
    }
}
