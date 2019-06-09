//--------------------------------------------------------------------------------------------------
// <copyright file="FixComponent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Messaging;
    using NodaTime;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using QuickFix.Transport;
    using Message = QuickFix.Message;

    /// <summary>
    /// The base class for all FIX protocol components.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reviewed. Access OK.")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Reviewed. Access OK.")]
    [PerformanceOptimized]
    public class FixComponent : MessageCracker, IApplication
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly ILogger logger;
        private readonly IMessagingAdapter messagingAdapter;
        private readonly CommandHandler commandHandler;
        private readonly FixConfiguration config;
        private readonly bool sendAccountTag;

        private SocketInitiator? initiator;
        private Session? session;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixComponent"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="config">The FIX configuration.</param>
        /// <param name="messageHandler">The FIX message handler.</param>
        /// <param name="messageRouter">The FIX message router.</param>
        protected FixComponent(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            FixConfiguration config,
            IFixMessageHandler messageHandler,
            IFixMessageRouter messageRouter)
        {
            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.logger = container.LoggerFactory.Create(new Label(nameof(FixClient)));
            this.messagingAdapter = messagingAdapter;
            this.commandHandler = new CommandHandler(this.logger);
            this.Broker = config.Broker;
            this.Account = config.Credentials.Account;
            this.config = config;
            this.sendAccountTag = config.SendAccountTag;

            this.FixMessageHandler = messageHandler;
            this.FixMessageRouter = messageRouter;
        }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        public ILogger Log => this.logger;

        /// <summary>
        /// Gets the name of the brokerage.
        /// </summary>
        public Brokerage Broker { get; }

        /// <summary>
        /// Gets the account number for FIX component.
        /// </summary>
        public string Account { get; }

        /// <summary>
        /// Gets the components FIX message handler.
        /// </summary>
        public IFixMessageHandler FixMessageHandler { get; }

        /// <summary>
        /// Gets the components FIX message router.
        /// </summary>
        public IFixMessageRouter FixMessageRouter { get; }

        /// <summary>
        /// Gets a value indicating whether the FIX session is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsConnected => this.session != null && this.session.IsLoggedOn;

        /// <summary>
        /// The initializes the execution gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        public void InitializeGateway(IFixGateway gateway)
        {
            this.FixMessageHandler.InitializeGateway(gateway);
        }

        /// <summary>
        /// Returns the current time of the components clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        public ZonedDateTime TimeNow()
        {
            return this.clock.TimeNow();
        }

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the black box systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        public Guid NewGuid()
        {
            return this.guidFactory.NewGuid();
        }

        /// <summary>
        /// Passes the given <see cref="Action"/> to the command handler for execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public void Execute(Action action)
        {
            this.commandHandler.Execute(action);
        }

        /// <summary>
        /// Connects to the FIX session.
        /// </summary>
        public void ConnectFix()
        {
            this.commandHandler.Execute(() =>
            {
                var settings = new SessionSettings(this.config.ConfigPath);
                var storeFactory = new FileStoreFactory(settings);

                // var logFactory = new ScreenLogFactory(settings);
                this.initiator = new SocketInitiator(this, storeFactory, settings, null);

                this.Log.Debug("Starting initiator...");
                this.initiator.Start();
            });
        }

        /// <summary>
        /// Disconnects from the FIX session.
        /// </summary>
        public void DisconnectFix()
        {
            this.commandHandler.Execute(() =>
            {
                this.Log.Debug("Stopping initiator... ");
                this.initiator?.Stop();
            });
        }

        /// <summary>
        /// Called when a new FIX session is created.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void OnCreate(SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                this.Log.Debug("Creating session...");
                this.session = Session.LookupSession(sessionId);
                this.FixMessageRouter.ConnectSession(this.session);
                this.Log.Debug($"Session {this.session}");
            });
        }

        /// <summary>
        /// Called on logon to the FIX session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void OnLogon(SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                this.messagingAdapter.SendToBus(
                    new FixSessionConnected(
                        this.Broker,
                        sessionId.ToString(),
                        this.NewGuid(),
                        this.TimeNow()),
                    new Address(nameof(FixGateway)),
                    this.TimeNow());

                this.Log.Debug($"Logon - {sessionId}");
            });
        }

        /// <summary>
        /// Called on logout from the FIX session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void OnLogout(SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                this.messagingAdapter.SendToBus(
                    new FixSessionDisconnected(
                        this.Broker,
                        sessionId.ToString(),
                        this.NewGuid(),
                        this.TimeNow()),
                    new Address(nameof(FixGateway)),
                    this.TimeNow());

                this.Log.Debug($"Logout - {sessionId}");
            });
        }

        /// <summary>
        /// Called when messages are received from the application.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session id.</param>
        public void FromAdmin(Message message, SessionID sessionId)
        {
        }

        /// <summary>
        /// Called when admin messages are sent from the application.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session id.</param>
        public void ToAdmin(Message message, SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                if (message is Logon)
                {
                    message.SetField(new Username(this.config.Credentials.Username));
                    message.SetField(new Password(this.config.Credentials.Password));

                    this.Log.Debug("Authorizing session...");
                }

                if (this.sendAccountTag)
                {
                    message.SetField(new Account(this.config.Credentials.Account));
                }
            });
        }

        /// <summary>
        /// Called when messages are received by the application.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session id.</param>
        public void FromApp(Message message, SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                try
                {
                    this.Crack(message, sessionId);
                }
                catch (UnsupportedMessageType)
                {
                    this.Log.Warning($"Received unsupported message type {message.GetType()}");
                }
            });
        }

        /// <summary>
        /// Called before messages are sent from the application.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <exception cref="DoNotSend">If possible duplication flag is true.</exception>
        public void ToApp(Message message, SessionID sessionId)
        {
            this.commandHandler.Execute<FieldNotFoundException>(() =>
            {
                if (this.sendAccountTag)
                {
                    message.SetField(new Account(this.config.Credentials.Account));
                }

                var possDupFlag = false;

                if (message.Header.IsSetField(Tags.PossDupFlag))
                {
                    possDupFlag =
                        QuickFix.Fields.Converters.BoolConverter.Convert(
                            message.Header.GetField(Tags.PossDupFlag));
                }

                if (possDupFlag)
                {
                    throw new DoNotSend();
                }
            });
        }

        /// <summary>
        /// Handles the business message reject message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(BusinessMessageReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the trading session status message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(TradingSessionStatus message, SessionID sessionId)
        {
            // TODO
        }

        /// <summary>
        /// Handles security list messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(SecurityList message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles quote status report messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(QuoteStatusReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the collateral inquiry acknowledgement message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(CollateralInquiryAck message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the collateral report message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(CollateralReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the request for positions acknowledgement message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(RequestForPositionsAck message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the market data request reject message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(MarketDataRequestReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the market data snapshot full refresh message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(MarketDataSnapshotFullRefresh message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the order cancel reject message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(OrderCancelReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the execution report message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(ExecutionReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles the position report message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(PositionReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }
    }
}
