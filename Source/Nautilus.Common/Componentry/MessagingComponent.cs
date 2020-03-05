//--------------------------------------------------------------------------------------------------
// <copyright file="MessagingComponent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// The base class for all service components.
    /// </summary>
    public abstract class MessagingComponent : Component
    {
        private readonly MessageProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingComponent"/> class.
        /// </summary>
        /// <param name="container">The components componentry container.</param>
        /// <param name="subName">The sub-name for the component.</param>
        protected MessagingComponent(IComponentryContainer container, string subName = "")
        : base(container, subName)
        {
            this.processor = new MessageProcessor();
            this.processor.RegisterUnhandled(this.Unhandled);

            this.Mailbox = new Mailbox(new Address(this.Name.Value), this.processor.Endpoint);
            this.ComponentState = ComponentState.Initialized;

            this.RegisterExceptionHandler(this.HandleException);
            this.RegisterHandler<Start>(this.OnMessage);
            this.RegisterHandler<Stop>(this.OnMessage);
        }

        /// <summary>
        /// Gets the components messaging mailbox.
        /// </summary>
        public Mailbox Mailbox { get; }

        /// <summary>
        /// Gets the components current state.
        /// </summary>
        public ComponentState ComponentState { get; private set; }

        /// <summary>
        /// Gets the agents end point.
        /// </summary>
        public Endpoint Endpoint => this.processor.Endpoint;

        /// <summary>
        /// Gets the message input count for the agent.
        /// </summary>
        public int InputCount => this.processor.CountInput;

        /// <summary>
        /// Gets the message processed count for the agent.
        /// </summary>
        public int ProcessedCount => this.processor.CountProcessed;

        /// <summary>
        /// Gets the message handler types.
        /// </summary>
        public IEnumerable<Type> HandlerTypes => this.processor.HandlerTypes;

        /// <summary>
        /// Gets the unhandled messages.
        /// </summary>
        public IEnumerable<object> UnhandledMessages => this.processor.UnhandledMessages;

        /// <summary>
        /// Sends a new <see cref="Start"/> message to the component.
        /// </summary>
        /// <returns>The asynchronous operation.</returns>
        public Task Start()
        {
            this.SendToSelf(new Start(this.NewGuid(), this.TimeNow())).Wait();

            while (this.ComponentState != ComponentState.Running)
            {
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sends a new <see cref="Stop"/> message to the component.
        /// </summary>
        /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
        /// <returns>The asynchronous operation.</returns>
        public async Task<bool> Stop(int timeoutMilliseconds = 2000)
        {
            this.SendToSelf(new Stop(this.NewGuid(), this.TimeNow())).Wait();

            await Task.WhenAny(this.processor.GracefulStop(), Task.Delay(timeoutMilliseconds));

            return this.ComponentState == ComponentState.Stopped;
        }

        /// <summary>
        /// Immediately kills the internal messaging processor.
        /// </summary>
        /// <returns>The async operation.</returns>
        public Task Kill()
        {
            this.ComponentState = ComponentState.Failed;
            this.Logger.LogError(LogId.Operation, $"{this.ComponentState} from kill command.");

            return this.processor.Kill();
        }

        /// <summary>
        /// Register the message type with the given handler.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The handler to register.</param>
        protected void RegisterHandler<TMessage>(Action<TMessage> handler)
        {
            this.processor.RegisterHandler(handler);
        }

        /// <summary>
        /// Register the given handler to receive exceptions raised by the processor.
        /// </summary>ve
        /// <param name="handler">The handler to register.</param>
        protected void RegisterExceptionHandler(Action<Exception> handler)
        {
            this.processor.RegisterExceptionHandler(handler);
        }

        /// <summary>
        /// Send the given message to this agents own endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The result of the task.</returns>
        protected Task SendToSelf(object message)
        {
            return this.Endpoint.SendAsync(message);
        }

        /// <summary>
        /// Actions to be performed on component start. Called when a Start message is received.
        /// After this method is called the component state with become Running.
        /// Note: A log event will be created for the Start message.
        /// </summary>
        /// <param name="start">The start message.</param>
        protected virtual void OnStart(Start start)
        {
            this.Logger.LogWarning(
                LogId.Operation,
                $"Received {start} with OnStart() not overridden in implementation.",
                start.Id);
        }

        /// <summary>
        /// Actions to be performed on component stop. Called when a Stop message is received.
        /// After this method is called the component state will become Stopped.
        /// Note: A log event will be created for the Stop message.
        /// </summary>
        /// <param name="stop">The stop message.</param>
        protected virtual void OnStop(Stop stop)
        {
            this.Logger.LogWarning(
                LogId.Operation,
                $"Received {stop} with OnStop() not overridden in implementation.",
                stop.Id);
        }

        /// <summary>
        /// Opens the envelope and sends the contained message to self.
        /// </summary>
        /// <param name="envelope">The envelope to open.</param>
        protected void OnEnvelope(IEnvelope envelope)
        {
            this.SendToSelf(envelope.Message);

            this.Logger.LogTrace(LogId.Operation, $"Received {envelope}.", envelope.Id, envelope.Message.Id);
        }

        private static string SetSubName(string subName)
        {
            if (subName != string.Empty)
            {
                subName = $"-{subName}";
            }

            return subName;
        }

        private void OnMessage(Start message)
        {
            this.Logger.LogDebug(LogId.Operation, $"Starting from message {message}...", message.Id);

            this.OnStart(message);
            this.ComponentState = ComponentState.Running;

            this.Logger.LogInformation(LogId.Operation, $"{this.ComponentState}...", this.ComponentState);
        }

        private void OnMessage(Stop message)
        {
            this.Logger.LogDebug(LogId.Operation, $"Stopping from message {message}...", message.Id);

            if (this.ComponentState != ComponentState.Running)
            {
                this.Logger.LogWarning(
                    LogId.Operation,
                    $"Stop message when component not already running, was {this.ComponentState}.",
                    message.Id);
            }

            this.OnStop(message);
            this.ComponentState = ComponentState.Stopped;

            this.Logger.LogInformation(LogId.Operation, $"{this.ComponentState}.", message.Id);
        }

        private void HandleException(Exception ex)
        {
            switch (ex)
            {
                case NullReferenceException _:
                case ArgumentException _:
                    this.Logger.LogError(ex.Message, ex);
                    break;
                default:
                    this.Logger.LogCritical(ex.Message, ex);
                    throw ex;
            }
        }

        private void Unhandled(object message)
        {
            this.Logger.LogError(LogId.Operation, $"Unhandled message: {message}.");
            this.processor.AddToUnhandledMessages(message);
        }
    }
}
