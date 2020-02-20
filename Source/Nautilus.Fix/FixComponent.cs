//--------------------------------------------------------------------------------------------------
// <copyright file="FixComponent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Fix.Interfaces;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using QuickFix.Transport;
    using Message = QuickFix.Message;

    /// <summary>
    /// The base class for all FIX components.
    /// </summary>
    public class FixComponent : MessageCracker, IApplication
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly ILogger logger;
        private readonly IMessageBusAdapter messageBusAdapter;
        private readonly FixCredentials credentials;
        private readonly string configPath;
        private readonly bool sendAccountTag;
        private readonly Account accountField;

        private SocketInitiator? initiator;
        private Session? session;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixComponent"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="config">The FIX configuration.</param>
        /// <param name="messageHandler">The FIX message handler.</param>
        /// <param name="messageRouter">The FIX message router.</param>
        protected FixComponent(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            FixConfiguration config,
            IFixMessageHandler messageHandler,
            IFixMessageRouter messageRouter)
        {
            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.logger = container.LoggerFactory.CreateLogger(this.GetType().Name);
            this.messageBusAdapter = messageBusAdapter;

            this.FixMessageHandler = messageHandler;
            this.FixMessageRouter = messageRouter;

            this.AccountId = new AccountId(config.Broker, config.Credentials.AccountNumber, config.AccountType);
            this.Brokerage = this.AccountId.Broker;
            this.AccountNumber = this.AccountId.AccountNumber;

            this.credentials = config.Credentials;
            this.configPath = config.ConfigPath;
            this.sendAccountTag = config.SendAccountTag;
            this.accountField = new Account(this.AccountNumber.Value);
        }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        public ILogger Logger => this.logger;

        /// <summary>
        /// Gets the account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the brokerage identifier.
        /// </summary>
        public Brokerage Brokerage { get; }

        /// <summary>
        /// Gets the account number identifier.
        /// </summary>
        public AccountNumber AccountNumber { get; }

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
        /// Gets a value indicating whether the FIX session is disconnected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsDisconnected => !this.IsConnected;

        /// <summary>
        /// Gets the components FIX session identifier.
        /// </summary>
        protected SessionID? SessionId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a connection should be maintained.
        /// </summary>
        protected bool MaintainConnection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the socket is stopped.
        /// </summary>
        protected bool SocketStopped { get; private set; }

        /// <summary>
        /// The initializes the FIX data gateway.
        /// </summary>
        /// <param name="gateway">The data gateway.</param>
        public void InitializeGateway(IDataGateway gateway)
        {
            this.FixMessageHandler.InitializeGateway(gateway);
        }

        /// <summary>
        /// The initializes the FIX trading gateway.
        /// </summary>
        /// <param name="gateway">The trading gateway.</param>
        public void InitializeGateway(ITradingGateway gateway)
        {
            this.FixMessageHandler.InitializeGateway(gateway.Endpoint);
        }

        /// <summary>
        /// Called when a new FIX session is created.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void OnCreate(SessionID sessionId)
        {
            if (!this.MaintainConnection)
            {
                this.Logger.LogError("QuickFix attempted to create a session with maintainConnection=false.");
                return;
            }

            this.Logger.LogDebug($"Creating session {sessionId}...");
            this.SessionId = sessionId;
            this.session = Session.LookupSession(sessionId);
            this.FixMessageRouter.InitializeSession(this.session);
        }

        /// <summary>
        /// Called on logon to the FIX session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void OnLogon(SessionID sessionId)
        {
            var connected = new SessionConnected(
                this.Brokerage,
                sessionId.ToString(),
                this.guidFactory.Generate(),
                this.clock.TimeNow());

            this.messageBusAdapter.SendToBus(connected, null, this.clock.TimeNow());
            this.Logger.LogDebug($"Connected to session {sessionId}");
        }

        /// <summary>
        /// Called on logout from the FIX session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public void OnLogout(SessionID sessionId)
        {
            var disconnected = new SessionDisconnected(
                this.Brokerage,
                sessionId.ToString(),
                this.guidFactory.Generate(),
                this.clock.TimeNow());

            this.messageBusAdapter.SendToBus(disconnected, null, this.clock.TimeNow());

            this.Logger.LogDebug($"Disconnected from session {sessionId}");
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
            if (message is Logon)
            {
                message.SetField(new Username(this.credentials.Username));
                message.SetField(new Password(this.credentials.Password));

                this.Logger.LogDebug("Authorizing session...");
            }

            if (this.sendAccountTag)
            {
                message.SetField(this.accountField);
            }
        }

        /// <summary>
        /// Called when messages are received by the application.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session id.</param>
        public void FromApp(Message message, SessionID sessionId)
        {
            this.Crack(message, sessionId);
        }

        /// <summary>
        /// Called before messages are sent from the application.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <exception cref="DoNotSend">If possible duplication flag is true.</exception>
        public void ToApp(Message message, SessionID sessionId)
        {
            if (this.sendAccountTag)
            {
                message.SetField(this.accountField);
            }

            var possibleDuplication = false;
            if (message.Header.IsSetField(Tags.PossDupFlag))
            {
                possibleDuplication =
                    QuickFix.Fields.Converters.BoolConverter.Convert(
                        message.Header.GetField(Tags.PossDupFlag));
            }

            if (possibleDuplication)
            {
                throw new DoNotSend();
            }
        }

        /// <summary>
        /// Handles <see cref="BusinessMessageReject"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(BusinessMessageReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="Email"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(Email message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="TradingSessionStatus"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(TradingSessionStatus message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="SecurityList"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(SecurityList message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="QuoteStatusReport"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(QuoteStatusReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="MarketDataRequestReject"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(MarketDataRequestReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="MarketDataSnapshotFullRefresh"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(MarketDataSnapshotFullRefresh message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="CollateralInquiryAck"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(CollateralInquiryAck message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="CollateralReport"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(CollateralReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="RequestForPositionsAck"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(RequestForPositionsAck message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="PositionReport"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(PositionReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="OrderCancelReject"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(OrderCancelReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles <see cref="ExecutionReport"/> messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(ExecutionReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Connects to the FIX session.
        /// </summary>
        protected void ConnectFix()
        {
            this.MaintainConnection = true;
            var settings = new SessionSettings(this.configPath);
            var storeFactory = new FileStoreFactory(settings);

            // var logFactory = new ScreenLogFactory(settings);
            this.initiator = new SocketInitiator(this, storeFactory, settings, null);

            this.Logger.LogDebug("Starting initiator...");
            this.initiator.Start();
            this.SocketStopped = false;
        }

        /// <summary>
        /// Disconnects from the FIX session.
        /// </summary>
        protected void DisconnectFix()
        {
            this.MaintainConnection = false;
            this.Logger.LogDebug("Stopping initiator... ");
            this.initiator?.Stop();
            this.SocketStopped = true;
        }
    }
}
