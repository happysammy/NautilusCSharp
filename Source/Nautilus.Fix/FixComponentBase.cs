//--------------------------------------------------------------------------------------------------
// <copyright file="FixComponentBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System;
    using System.Threading.Tasks;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix.Interfaces;
    using NodaTime;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using QuickFix.Transport;
    using Message = QuickFix.Message;

    /// <summary>
    /// The base class for all FIX protocol components.
    /// </summary>
    public class FixComponentBase : MessageCracker, IApplication
    {
        private readonly Enum service;
        private readonly Label component;
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly ILogger logger;
        private readonly CommandHandler commandHandler;
        private readonly FixCredentials credentials;

        private SocketInitiator initiator;
        private Session session;
        private Session sessionMd;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase"/> class.
        /// </summary>
        /// <param name="service">The service name.</param>
        /// <param name="component">The component label.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="tickDataProcessor">The tick data processor.</param>
        /// <param name="fixMessageHandler">The FIX message handler</param>
        /// <param name="fixMessageRouter">The FIX message router.</param>
        /// <param name="credentials">The FIX account credentials</param>
        protected FixComponentBase(
            Enum service,
            Label component,
            IComponentryContainer container,
            ITickDataProcessor tickDataProcessor,
            IFixMessageHandler fixMessageHandler,
            IFixMessageRouter fixMessageRouter,
            FixCredentials credentials)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(tickDataProcessor, nameof(tickDataProcessor));
            Validate.NotNull(credentials, nameof(credentials));

            this.service = service;
            this.component = component;
            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.logger = container.LoggerFactory.Create(service, this.component);
            this.commandHandler = new CommandHandler(this.logger);
            this.credentials = credentials;
            this.FxcmFixMessageHandler = fixMessageHandler;
            this.FxcmFixMessageRouter = fixMessageRouter;
        }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        protected ILogger Log => this.logger;

        /// <summary>
        /// Gets the components FIX message handler.
        /// </summary>
        protected IFixMessageHandler FxcmFixMessageHandler { get; }

        /// <summary>
        /// Gets the components FIX message router.
        /// </summary>
        protected IFixMessageRouter FxcmFixMessageRouter { get; }

        /// <summary>
        /// Returns the current time of the black box system clock.
        /// </summary>
        /// <returns>
        /// A <see cref="ZonedDateTime"/>.
        /// </returns>
        protected ZonedDateTime TimeNow()
        {
            return this.clock.TimeNow();
        }

        /// <summary>
        /// Returns a new <see cref="Guid"/> from the black box systems <see cref="Guid"/> factory.
        /// </summary>
        /// <returns>A <see cref="Guid"/>.</returns>
        protected Guid NewGuid()
        {
            return this.guidFactory.NewGuid();
        }

        /// <summary>
        /// Passes the given <see cref="Action"/> to the <see cref="commandHandler"/> for execution.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected void Execute(Action action)
        {
            this.commandHandler.Execute(action);
        }

        /// <summary>
        /// The is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsFixConnected => this.session.IsLoggedOn;

        /// <summary>
        /// The connect.
        /// </summary>
        public void ConnectFix()
        {
            this.commandHandler.Execute(() =>
            {
                var settings = new SessionSettings("fix_fxcm.cfg");
                var storeFactory = new FileStoreFactory(settings);
                var logFactory = new ScreenLogFactory(settings);
                this.initiator = new SocketInitiator(this, storeFactory, settings, logFactory);

                this.Log.Information("Starting initiator...");
                this.initiator.Start();

                Task.Delay(1000);
            });
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void DisconnectFix()
        {
            this.commandHandler.Execute(() =>
            {
                Validate.NotNull(this.initiator, nameof(this.initiator));

                this.initiator.Stop();

                this.Log.Information("Stopping initiator... ");
            });
        }

        /// <summary>
        /// The on create.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        public void OnCreate(SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                if (this.session == null)
                {
                    this.Log.Information("Creating session...");
                    this.session = Session.LookupSession(sessionId);
                    this.FxcmFixMessageRouter.ConnectSession(this.session);
                    this.Log.Information($"Session {this.session}");
                }

                this.sessionMd = Session.LookupSession(sessionId);
                this.FxcmFixMessageRouter.ConnectSessionMd(this.sessionMd);
            });
        }

        /// <summary>
        /// The on logon.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        public void OnLogon(SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                this.Log.Information($"Logon - {sessionId}");
            });
        }

        /// <summary>
        /// The on logout.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        public void OnLogout(SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                this.Log.Information($"Logout - {sessionId}");
            });
        }

        /// <summary>
        /// The from admin.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session id.</param>
        public void FromAdmin(Message message, SessionID sessionId)
        {
        }

        /// <summary>
        /// The to admin.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session id.</param>
        public void ToAdmin(Message message, SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));
                Validate.NotNull(sessionId, nameof(sessionId));

                if (message.GetType() == typeof(Logon))
                {
                    message.SetField(new Username(this.credentials.Username));
                    message.SetField(new Password(this.credentials.Password));

                    this.Log.Information("Authorizing session...");
                }

                message.SetField(new Account(this.credentials.AccountNumber));
            });
        }

        /// <summary>
        /// The from app.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session id.</param>
        public void FromApp(Message message, SessionID sessionId)
        {
            this.commandHandler.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));
                Validate.NotNull(sessionId, nameof(sessionId));

                this.Crack(message, sessionId);
            });
        }

        /// <summary>
        /// The to app.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session id.</param>
        /// <exception cref="DoNotSend">Will not send.</exception>
        public void ToApp(Message message, SessionID sessionId)
        {
            this.commandHandler.Execute<FieldNotFoundException>(() =>
            {
                message.SetField(new Account(this.credentials.AccountNumber));

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
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(BusinessMessageReject message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnBusinessMessageReject(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(TradingSessionStatus message, SessionID sessionId)
        {
            // TODO
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(SecurityList message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnSecurityList(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(CollateralInquiryAck message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnCollateralInquiryAck(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(CollateralReport message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnCollateralReport(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(RequestForPositionsAck message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnRequestForPositionsAck(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(MarketDataSnapshotFullRefresh message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnMarketDataSnapshotFullRefresh(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(OrderCancelReject message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnOrderCancelReject(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(ExecutionReport message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnExecutionReport(message);
        }

        /// <summary>
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(PositionReport message, SessionID sessionId)
        {
            this.FxcmFixMessageHandler.OnPositionReport(message);
        }
    }
}
