// -------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Nautilus.Messaging.Internal;

namespace Nautilus.Messaging
{
    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    public sealed class MessageProcessor : IDisposable
    {
        private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();
        private readonly ActionBlock<object> processor;
        private readonly List<KeyValuePair<Type, Handler>> registeredHandlers = new List<KeyValuePair<Type, Handler>>();

        private Action<Exception> exceptionHandler;
        private Action<object> unhandled;
        private Handler[] handlers = { };
        private int handlersLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            this.exceptionHandler = this.Rethrow;
            this.unhandled = this.AddToUnhandledMessages;

            this.processor = new ActionBlock<object>(
                async message =>
                {
                    await this.HandleMessage(message);
                },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = (int)Math.Pow(2, 22),
                    CancellationToken = this.cancellationSource.Token,
                    EnsureOrdered = true,
                    MaxDegreeOfParallelism = 1,
                });

            this.Endpoint = new Endpoint(this.processor);
        }

        /// <summary>
        /// Gets the exceptions caught by the processor.
        /// </summary>
        public List<Exception> Exceptions { get; } = new List<Exception>();

        /// <summary>
        /// Gets the processors end point.
        /// </summary>
        public Endpoint Endpoint { get; }

        /// <summary>
        /// Gets the count of input messages for the processor.
        /// </summary>
        public int CountInput => this.processor.InputCount;

        /// <summary>
        /// Gets the count of messages processed for the processor.
        /// </summary>
        public int CountProcessed { get; private set; }

        /// <summary>
        /// Gets the types which can be handled.
        /// </summary>
        /// <returns>The list.</returns>
        public IEnumerable<Type> HandlerTypes => this.registeredHandlers.Select(h => h.Key);

        /// <summary>
        /// Gets the list of unhandled messages.
        /// </summary>
        public List<object> UnhandledMessages { get; } = new List<object>();

        /// <summary>
        /// Gracefully stops the message processor by waiting for all currently accepted messages to
        /// be processed. Messages received after this command will not be processed.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public Task<bool> GracefulStop()
        {
            try
            {
                this.processor.Complete();
                Task.WhenAll(this.processor.Completion).Wait();

                return Task.FromResult(true);
            }
            catch (AggregateException)
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Immediately kills the message processor.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public Task Kill()
        {
            try
            {
                this.cancellationSource.Cancel();
                Task.WhenAll(this.processor.Completion).Wait();
            }
            catch (AggregateException)
            {
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Register the given message type with the gin handler.
        /// </summary>ve
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler<TMessage>(Action<TMessage> handler)
        {
            var type = typeof(TMessage);
            if (this.registeredHandlers.Any(h => h.Key == type))
            {
                throw new ArgumentException("Cannot register handler " +
                                            $"(the internal handlers already contain a handler for {type} type messages).");
            }

            this.registeredHandlers.Add(new KeyValuePair<Type, Handler>(type, Handler.Create(handler)));

            this.MoveHandlerToEndOfList(typeof(object));
            this.BuildHandlers();
        }

        /// <summary>
        /// Register the given delegate to receive exceptions generated by the message queue.
        /// </summary>ve
        /// <param name="handler">The handler delegate.</param>
        public void RegisterExceptionHandler(Action<Exception> handler)
        {
            this.exceptionHandler = handler;
        }

        /// <summary>
        /// Register the given delegate to receive unhandled messages.
        /// </summary>ve
        /// <param name="handler">The handler delegate.</param>
        public void RegisterUnhandled(Action<object> handler)
        {
            this.unhandled = handler;
        }

        /// <summary>
        /// Adds the given message to the list of unhandled messages.
        /// </summary>
        /// <param name="message">The unhandled message.</param>
        public void AddToUnhandledMessages(object message)
        {
            this.UnhandledMessages.Add(message);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.cancellationSource.Dispose();
        }

        private void Rethrow(Exception exception)
        {
            throw exception;
        }

        private void Unhandled(object message)
        {
            this.unhandled(message);
            this.CountProcessed++;
        }

        private void BuildHandlers()
        {
            this.handlers = this.registeredHandlers.Select(h => h.Value).ToArray();
            this.handlersLength = this.handlers.Length;
        }

        private Task HandleMessage(object message)
        {
            try
            {
                for (var i = 0; i < this.handlersLength; i++)
                {
                    if (this.handlers[i].Handle(message).Result)
                    {
                        this.CountProcessed++;
                        return Task.CompletedTask;
                    }
                }

                this.Unhandled(message);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                this.Unhandled(message);
                this.Exceptions.Add(ex);
                this.exceptionHandler(ex);

                return Task.FromException(ex);
            }
        }

        private void MoveHandlerToEndOfList(Type handlerType)
        {
            if (this.registeredHandlers.Any(h => h.Key == handlerType))
            {
                // Move handle object to the end of the handlers
                var handleObject = this.registeredHandlers.FirstOrDefault(h => h.Key == handlerType);
                this.registeredHandlers.Remove(handleObject);
                this.registeredHandlers.Add(handleObject);
            }
        }
    }
}
