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
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using QuickFix.Transport;
    using Message = QuickFix.Message;

    public class FixComponentBase : MessageCracker, IApplication
    {
        private readonly IZonedClock clock;
        private readonly ILogger logger;
        private readonly IGuidFactory guidFactory;
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
        /// <param name="credentials">The FIX account credentials</param>
        protected FixComponentBase(
            Enum service,
            Label component,
            IComponentryContainer container,
            FixCredentials credentials)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));

            this.Service = service;
            this.Component = component;
            this.clock = container.Clock;
            this.logger = container.LoggerFactory.Create(service, this.Component);
            this.guidFactory = container.GuidFactory;
            this.credentials = credentials;
            this.CommandHandler = new CommandHandler(this.logger);

            this.FixMessageHandler = new FixMessageHandler();
            this.FixMessageRouter = new FixMessageRouter();
        }

        /// <summary>
        /// Gets the black box service context.
        /// </summary>
        protected Enum Service { get; }

        /// <summary>
        /// Gets the components label.
        /// </summary>
        protected Label Component { get; }

        /// <summary>
        /// Gets the command handler.
        /// </summary>
        protected CommandHandler CommandHandler { get; }

        /// <summary>
        /// Gets the FIX message handler.
        /// </summary>
        protected FixMessageHandler FixMessageHandler { get; }

        /// <summary>
        /// Gets the FIX message router.
        /// </summary>
        protected FixMessageRouter FixMessageRouter { get; }

        /// <summary>
        /// Creates a log event with the given level and text.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logText">The log text.</param>
        protected void Log(LogLevel logLevel, string logText)
        {
            this.logger.Log(logLevel, logText);
        }

        /// <summary>
        /// Logs the result with the <see cref="ILogger"/>.
        /// </summary>
        /// <param name="result">The command result.</param>
        protected void LogResult(ResultBase result)
        {
            if (result.IsSuccess)
            {
                this.Log(LogLevel.Information, result.Message);
            }
            else
            {
                this.Log(LogLevel.Warning, result.Message);
            }
        }

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
        /// The is connected.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsFixConnected => this.session.IsLoggedOn;

        /// <summary>
        /// The connect.
        /// </summary>
        public void ConnectFix()
        {
            this.CommandHandler.Execute(() =>
            {
                var settings = new SessionSettings("fix_fxcm.cfg");
                var storeFactory = new FileStoreFactory(settings);
                var logFactory = new ScreenLogFactory(settings);
                this.initiator = new SocketInitiator(this, storeFactory, settings, logFactory);

                this.Log(LogLevel.Information, "Starting initiator...");
                this.initiator.Start();

                Task.Delay(1000);
            });
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void DisconnectFix()
        {
            this.CommandHandler.Execute(() =>
            {
                Validate.NotNull(this.initiator, nameof(this.initiator));

                this.initiator.Stop();

                this.Log(LogLevel.Information, "Stopping initiator... ");
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
            this.CommandHandler.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                if (this.session == null)
                {
                    this.Log(LogLevel.Information, "Creating session...");
                    this.session = Session.LookupSession(sessionId);
                    this.FixMessageRouter.ConnectSession(this.session);
                    this.Log(LogLevel.Information, $"Session {this.session}");
                }

                this.sessionMd = Session.LookupSession(sessionId);
                this.FixMessageRouter.ConnectSessionMd(this.sessionMd);
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
            this.CommandHandler.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                this.Log(LogLevel.Information, $"Logon - {sessionId}");

                // allow logon to complete
                Task.Delay(100).Wait();

                if (sessionId.Equals(this.sessionMd.SessionID))
                {
                    return;
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
            this.CommandHandler.Execute(() =>
            {
                Validate.NotNull(sessionId, nameof(sessionId));

                this.Log(LogLevel.Information, $"Logout - {sessionId}");
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
            this.CommandHandler.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));
                Validate.NotNull(sessionId, nameof(sessionId));

                if (message.GetType() == typeof(Logon))
                {
                    message.SetField(new Username(this.credentials.Username));
                    message.SetField(new Password(this.credentials.Password));

                    this.Log(LogLevel.Information, "Authorizing session...");
                }

                message.SetField(new Account(this.credentials.AccountNumber));
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
            this.CommandHandler.Execute(() =>
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
            this.CommandHandler.Execute<FieldNotFoundException>(() =>
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
            this.FixMessageHandler.OnBusinessMessageReject(message);
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
            this.FixMessageHandler.OnSecurityList(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(CollateralInquiryAck message, SessionID sessionId)
        {
            this.FixMessageHandler.OnCollateralInquiryAck(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(CollateralReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnCollateralReport(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(RequestForPositionsAck message, SessionID sessionId)
        {
            this.FixMessageHandler.OnRequestForPositionsAck(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(MarketDataSnapshotFullRefresh message, SessionID sessionId)
        {
            this.FixMessageHandler.OnMarketDataSnapshotFullRefresh(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(OrderCancelReject message, SessionID sessionId)
        {
            this.FixMessageHandler.OnOrderCancelReject(message);
        }

        /// <summary>
        /// The on message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(ExecutionReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnExecutionReport(message);
        }

        /// <summary>
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        protected void OnMessage(PositionReport message, SessionID sessionId)
        {
            this.FixMessageHandler.OnPositionReport(message);
        }
    }
}
