//--------------------------------------------------------------------------------------------------
// <copyright file="FixComponent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
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
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reviewed. Access OK.")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Reviewed. Access OK.")]
    public class FixComponent : MessageCracker, IApplication
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly ILogger logger;
        private readonly CommandHandler commandHandler;
        private readonly FixCredentials credentials;

        private SocketInitiator initiator;
        private Session session;
        private Session sessionMd;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixComponent"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="fixMessageHandler">The FIX message handler.</param>
        /// <param name="fixMessageRouter">The FIX message router.</param>
        /// <param name="credentials">The FIX account credentials.</param>
        protected FixComponent(
            IComponentryContainer container,
            IFixMessageHandler fixMessageHandler,
            IFixMessageRouter fixMessageRouter,
            FixCredentials credentials)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(credentials, nameof(credentials));

            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.logger = container.LoggerFactory.Create(
                NautilusService.FIX,
                LabelFactory.Component(nameof(FixComponent)));
            this.commandHandler = new CommandHandler(this.logger);
            this.credentials = credentials;
            this.FixMessageHandler = fixMessageHandler;
            this.FixMessageRouter = fixMessageRouter;
        }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        public ILogger Log => this.logger;

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
        public bool IsFixConnected => this.session.IsLoggedOn
                                         && this.sessionMd.IsLoggedOn;

        /// <summary>
        /// The initializes the execution gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        public void InitializeGateway(IExecutionGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.FixMessageHandler.InitializeGateway(gateway);
        }

        /// <summary>
        /// Returns the current time of the black box system clock.
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
                var settings = new SessionSettings("fix_fxcm.cfg");
                var storeFactory = new FileStoreFactory(settings);

                // var logFactory = new ScreenLogFactory(settings);
                this.initiator = new SocketInitiator(this, storeFactory, settings, null);

                this.Log.Information("Starting initiator...");
                this.initiator.Start();

                Task.Delay(1000);
            });
        }

        /// <summary>
        /// Disconnects from the FIX session.
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
                    this.FixMessageRouter.ConnectSession(this.session);
                    this.Log.Information($"Session {this.session}");
                }

                this.sessionMd = Session.LookupSession(sessionId);
                this.FixMessageRouter.ConnectSessionMd(this.sessionMd);
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
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(BusinessMessageReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(TradingSessionStatus message, SessionID sessionId)
        {
            // TODO
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(SecurityList message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(CollateralInquiryAck message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(CollateralReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(RequestForPositionsAck message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(MarketDataRequestReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(MarketDataSnapshotFullRefresh message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(OrderCancelReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(ExecutionReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }

        /// <summary>
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(PositionReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMessage(message);
        }
    }
}
