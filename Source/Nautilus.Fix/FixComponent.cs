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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Events;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
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
        private readonly string configFileName;

        private ReadOnlyList<IEndpoint> connectionEventReceivers;
        private SocketInitiator initiator;
        private Session session;
        private Session sessionMd;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixComponent"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="broker">The FIX components brokerage.</param>
        /// <param name="fixMessageHandler">The FIX message handler.</param>
        /// <param name="fixMessageRouter">The FIX message router.</param>
        /// <param name="credentials">The FIX account credentials.</param>
        /// <param name="configFileName">The FIX config file name.</param>
        protected FixComponent(
            IComponentryContainer container,
            Broker broker,
            IFixMessageHandler fixMessageHandler,
            IFixMessageRouter fixMessageRouter,
            FixCredentials credentials,
            string configFileName)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(fixMessageHandler, nameof(fixMessageHandler));
            Validate.NotNull(fixMessageRouter, nameof(fixMessageRouter));
            Validate.NotNull(credentials, nameof(credentials));
            Validate.NotNull(configFileName, nameof(configFileName));

            this.clock = container.Clock;
            this.guidFactory = container.GuidFactory;
            this.logger = container.LoggerFactory.Create(
                NautilusService.FIX,
                LabelFactory.Component(nameof(FixComponent)));
            this.commandHandler = new CommandHandler(this.logger);
            this.Broker = broker;
            this.credentials = credentials;
            this.configFileName = configFileName;
            this.FixMessageHandler = fixMessageHandler;
            this.FixMessageRouter = fixMessageRouter;

            this.connectionEventReceivers = new ReadOnlyList<IEndpoint>(new List<IEndpoint>());
        }

        /// <summary>
        /// Gets the components logger.
        /// </summary>
        public ILogger Log => this.logger;

        /// <summary>
        /// Gets the name of the brokerage.
        /// </summary>
        public Broker Broker { get; }

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
        public void InitializeGateway(IFixGateway gateway)
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
        /// Registers the given receiver to receive connection events from the FIX client.
        /// </summary>
        /// <param name="receiver">The event receiver endpoint.</param>
        public void RegisterConnectionEventReceiver(IEndpoint receiver)
        {
            Validate.NotNull(receiver, nameof(receiver));
            Debug.DoesNotContain(receiver, nameof(receiver), this.connectionEventReceivers);

            var receivers = this.connectionEventReceivers.ToList();
            receivers.Add(receiver);

            this.connectionEventReceivers = new ReadOnlyList<IEndpoint>(receivers);
        }

        /// <summary>
        /// Connects to the FIX session.
        /// </summary>
        public void ConnectFix()
        {
            this.commandHandler.Execute(() =>
            {
                var settings = new SessionSettings(this.configFileName);
                var storeFactory = new FileStoreFactory(settings);

                // var logFactory = new ScreenLogFactory(settings);
                this.initiator = new SocketInitiator(this, storeFactory, settings, null);

                this.Log.Debug("Starting initiator...");
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

                this.Log.Debug("Stopping initiator... ");
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
                Validate.NotNull(sessionId, nameof(sessionId));

                if (this.session == null)
                {
                    this.Log.Debug("Creating session...");
                    this.session = Session.LookupSession(sessionId);
                    this.FixMessageRouter.ConnectSession(this.session);
                    this.Log.Debug($"Session {this.session}");
                }

                this.sessionMd = Session.LookupSession(sessionId);
                this.FixMessageRouter.ConnectSessionMd(this.sessionMd);
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
                Validate.NotNull(sessionId, nameof(sessionId));

                foreach (var receiver in this.connectionEventReceivers)
                {
                    receiver.Send(new BrokerageConnected(
                        this.Broker,
                        sessionId.ToString(),
                        this.NewGuid(),
                        this.TimeNow()));
                }

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
                Validate.NotNull(sessionId, nameof(sessionId));

                foreach (var receiver in this.connectionEventReceivers)
                {
                    receiver.Send(new BrokerageDisconnected(
                        this.Broker,
                        sessionId.ToString(),
                        this.NewGuid(),
                        this.TimeNow()));
                }

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
                Validate.NotNull(message, nameof(message));
                Validate.NotNull(sessionId, nameof(sessionId));

                if (message.GetType() == typeof(Logon))
                {
                    message.SetField(new Username(this.credentials.Username));
                    message.SetField(new Password(this.credentials.Password));

                    this.Log.Debug("Authorizing session...");
                }

                message.SetField(new Account(this.credentials.Account));
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
        /// Called before messages are sent from the application.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <exception cref="DoNotSend">If possible duplication flag is true.</exception>
        public void ToApp(Message message, SessionID sessionId)
        {
            this.commandHandler.Execute<FieldNotFoundException>(() =>
            {
                message.SetField(new Account(this.credentials.Account));

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
        /// Handles the security list message.
        /// </summary>
        /// <param name="message">The FIX message.</param>
        /// <param name="sessionId">The session identifier.</param>
        public void OnMessage(SecurityList message, SessionID sessionId)
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
