// -------------------------------------------------------------------------------------------------
// <copyright file="MessageQueue.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using NetMQ;

    /// <summary>
    /// Provides an asynchronous duplex message queue.
    /// </summary>
    public sealed class MessageQueue : Component
    {
        private const int ExpectedFrameCount = 3; // Version 1.0

        private readonly NetMQSocket socketInbound;
        private readonly NetMQSocket socketOutbound;
        private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();
        private readonly ActionBlock<byte[][]> bufferInbound;
        private readonly ActionBlock<byte[][]> bufferOutbound;
        private readonly Action<byte[][]> framesReceiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueue"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="socketInbound">The inbound socket.</param>
        /// <param name="socketOutbound">The outbound socket.</param>
        /// <param name="framesReceiver">The frames receiver.</param>
        public MessageQueue(
            IComponentryContainer container,
            NetMQSocket socketInbound,
            NetMQSocket socketOutbound,
            Action<byte[][]> framesReceiver)
         : base(container)
        {
            this.socketInbound = socketInbound;
            this.socketOutbound = socketOutbound;
            this.framesReceiver = framesReceiver;

            this.bufferInbound = new ActionBlock<byte[][]>(
                async frames =>
                {
                    await this.ProcessInbound(frames);
                },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 20000,
                    CancellationToken = this.cancellationSource.Token,
                    EnsureOrdered = true,
                    MaxDegreeOfParallelism = 1,
                });

            this.bufferOutbound = new ActionBlock<byte[][]>(
                async frames =>
                {
                    await this.ProcessOutbound(frames);
                },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 20000,
                    CancellationToken = this.cancellationSource.Token,
                    EnsureOrdered = true,
                    MaxDegreeOfParallelism = 1,
                });

            Task.Run(this.ReceiveFrames, this.cancellationSource.Token);
        }

        /// <summary>
        /// Send the given payload on the outbound socket.
        /// </summary>
        /// <param name="frames">The frames payload to send.</param>
        public void Send(params byte[][] frames)
        {
            this.bufferOutbound.Post(frames);
        }

        /// <summary>
        /// Gracefully stops the message queue by waiting for all currently accepted messages to
        /// be processed. Messages received after this command will not be processed.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public Task<bool> GracefulStop()
        {
            try
            {
                this.bufferInbound.Complete();
                this.bufferOutbound.Complete();
                Task.WhenAll(this.bufferInbound.Completion, this.bufferOutbound.Completion).Wait();

                return Task.FromResult(true);
            }
            catch (AggregateException ex)
            {
                this.Logger.LogError(ex.Message, ex);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Immediately kills the message queue.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public Task Kill()
        {
            try
            {
                this.cancellationSource.Cancel();
                Task.WhenAll(this.bufferInbound.Completion, this.bufferOutbound.Completion).Wait();
            }
            catch (AggregateException ex)
            {
                this.Logger.LogError(ex.Message, ex);
            }

            return Task.CompletedTask;
        }

        private Task ReceiveFrames()
        {
            try
            {
                while (!this.cancellationSource.IsCancellationRequested)
                {
                    this.bufferInbound.Post(this.socketInbound.ReceiveMultipartBytes(ExpectedFrameCount).ToArray());
                }
            }
            catch (Exception ex)
            {
                // Interaction with NetMQ
                this.Logger.LogError(LogId.Networking, ex.ToString(), ex);
            }

            this.Logger.LogDebug(LogId.Networking, "Stopped receiving inbound frames.");
            return Task.CompletedTask;
        }

        private Task ProcessInbound(byte[][] frames)
        {
            this.framesReceiver(frames);

            return Task.CompletedTask;
        }

        private Task ProcessOutbound(byte[][] frames)
        {
            try
            {
                this.socketOutbound.SendMultipartBytes(frames); // Blocking
            }
            catch (Exception ex)
            {
                // Interaction with NetMQ
                // A RouterSocket will throw HostUnreadableException if a message cannot be routed
                this.Logger.LogError(LogId.Networking, ex.ToString(), ex);
            }

            return Task.CompletedTask;
        }
    }
}
