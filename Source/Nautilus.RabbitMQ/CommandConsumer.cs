// -------------------------------------------------------------------------------------------------
// <copyright file="CommandConsumer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RabbitMQ
{
    using System;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides a command consumer for the <see cref="RabbitMQServer"/>.
    /// </summary>
    public class CommandConsumer : ComponentBase, IBasicConsumer
    {
        private readonly ICommandSerializer serializer;
        private readonly IEndpoint receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandConsumer"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="serializer">The command serializer.</param>
        /// <param name="model">The RabbitMQ model.</param>
        /// <param name="receiver">The command receiver.</param>
        public CommandConsumer(
            IComponentryContainer setupContainer,
            ICommandSerializer serializer,
            IModel model,
            IEndpoint receiver)
            : base(
                NautilusService.Messaging,
                LabelFactory.Component(nameof(CommandConsumer)),
                setupContainer)
        {
            this.serializer = serializer;
            this.Model = model;
            this.receiver = receiver;
        }

        /// <summary>
        /// TBA.
        /// </summary>
        public event EventHandler<ConsumerEventArgs> ConsumerCancelled = delegate { };

        /// <summary>
        /// Gets the consumers model.
        /// </summary>
        public IModel Model { get; }

        /// <summary>
        /// TBA.
        /// </summary>
        /// <param name="consumerTag">The consumer tag.</param>
        public void HandleBasicCancel(string consumerTag)
        {
            this.Log.Warning($"HandleBasicCancel not implemented...");
        }

        /// <summary>
        /// TBA.
        /// </summary>
        /// <param name="consumerTag">The consumer tag.</param>
        public void HandleBasicCancelOk(string consumerTag)
        {
            this.Log.Warning($"HandleBasicCancelOk not implemented...");
        }

        /// <summary>
        /// TBA.
        /// </summary>
        /// <param name="consumerTag">The consumer tag.</param>
        public void HandleBasicConsumeOk(string consumerTag)
        {
            this.Log.Warning($"HandleBasicConsumeOk not implemented...");
        }

        /// <summary>
        /// Handles the delivery of a message.
        /// </summary>
        /// <param name="consumerTag">The consumer tag.</param>
        /// <param name="deliveryTag">The delivery tag.</param>
        /// <param name="redelivered">The redelivered boolean flag.</param>
        /// <param name="exchange">The exchange name.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="properties">The message properties.</param>
        /// <param name="body">The message body.</param>
        public void HandleBasicDeliver(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            byte[] body)
        {
            var command = this.serializer.Deserialize(body);

            this.receiver.Send(command);

            this.Model.BasicAck(deliveryTag, false);
            this.Log.Debug($"Received {command}.");
        }

        /// <summary>
        /// TBA.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="reason">The shutdown reason.</param>
        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            this.Log.Warning($"HandleModelShutdown not implemented...");
        }
    }
}
