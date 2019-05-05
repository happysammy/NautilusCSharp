// -------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    public class MessageProcessor
    {
        private readonly ActionBlock<object> processor;
        private readonly Dictionary<Type, Handler> handlers = new Dictionary<Type, Handler>();

        private Func<object, Task> unhandled;
        private Func<object, Task> handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            this.unhandled = this.Unhandled;
            this.handle = this.CompileHandlerTree();

            this.processor = new ActionBlock<object>(async message =>
            {
                await this.handle(message);
            });

            this.Endpoint = new Endpoint(this.processor.Post);
        }

        /// <summary>
        /// Gets the processors end point.
        /// </summary>
        public Endpoint Endpoint { get; }

        /// <summary>
        /// Gets the message input count for the processor.
        /// </summary>
        public int InputCount => this.processor.InputCount;

        /// <summary>
        /// Gets the list of messages types which can be handled.
        /// </summary>
        /// <returns>The list.</returns>
        public IEnumerable<Type> HandlerTypes => this.handlers.Keys.ToList().AsReadOnly();

        /// <summary>
        /// Gets the list of unhandled messages.
        /// </summary>
        public List<object> UnhandledMessages { get; } = new List<object>();

        /// <summary>
        /// Register the given message type with the gin handler.
        /// </summary>ve
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler<TMessage>(Action<TMessage> handler)
        {
            var type = typeof(TMessage);
            if (this.handlers.ContainsKey(type))
            {
                throw new ArgumentException($"The internal handlers already contain a handler for {type} type messages.");
            }

            this.handlers.Add(type, Handler.Create(handler));
            this.handle = this.CompileHandlerTree();
        }

        /// <summary>
        /// Register the given handler to receive unhandled messaged.
        /// </summary>ve
        /// <param name="handler">The handler.</param>
        public void RegisterUnhandled(Action<object> handler)
        {
            this.unhandled = Handler.Create(handler).Handle;
            this.handle = this.CompileHandlerTree();
        }

        /// <summary>
        /// Handles unhandled messages.
        /// </summary>
        /// <param name="message">The unhandled message.</param>
        private Task Unhandled(object message)
        {
            this.UnhandledMessages.Add(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Compiles the message handling delegate from the handlers dictionary.
        /// </summary>
        /// <returns>The created delegate.</returns>
        private Func<object, Task> CompileHandlerTree()
        {
            var message = Expression.Parameter(typeof(object), "message");

            var expressions = new List<Expression>();
            foreach (var handler in this.handlers.Values)
            {
                Expression<Func<object, Task>> handleExpression = msg => handler.Handle(msg);
                var call = Expression.Call(
                    handleExpression,
                    typeof(Func<object, Task>).GetMethod(nameof(Func<object, Task>.Invoke)),
                    message);

                expressions.Add(call);
            }

            Expression.Block(expressions);
            Expression<Func<object, Task>> unhandledExpression = msg => this.unhandled(msg);
            var unhandledCall = Expression.Call(
                unhandledExpression,
                typeof(Func<object, Task>).GetMethod(nameof(Func<object, Task>.Invoke)),
                message);
            expressions.Add(unhandledCall);

            var body = Expression.Block(typeof(Task), expressions.ToArray());
            var tree = Expression.Lambda<Func<object, Task>>(body, message);

            return tree.Compile();
        }

// /// <summary>
//        /// Compiles the message handling delegate from the handlers dictionary.
//        /// </summary>
//        /// <returns>The created delegate.</returns>
//        private Action<object> CompileHandlerTree()
//        {
//            var message = Expression.Parameter(typeof(object), "message");
//            var messageType = Expression.Parameter(typeof(Type), "message.Type");
//
//            var cases = new List<SwitchCase>();
//            foreach (var (type, handler) in this.handlers)
//            {
//                var typeConstant = Expression.Constant(type);
//
//                Expression<Action<object>> handleExpression = msg => handler.Handle(msg);
//                var call = Expression.Call(
//                    handleExpression,
//                    typeof(Action<object>).GetMethod(nameof(Action<object>.Invoke)),
//                    message);
//
//                cases.Add(Expression.SwitchCase(call, typeConstant));
//            }
//
//            Expression<Action<object>> unhandledExpression = msg => this.unhandled(msg);
//            var defaultBody = Expression.Call(
//                unhandledExpression,
//                typeof(Action<object>).GetMethod(nameof(Action<object>.Invoke)),
//                message);
//
//            var body = Expression.Switch(message, defaultBody, cases.ToArray());
//            var tree = Expression.Lambda<Action<object>>(body, message);
//
//            return tree.Compile();
//        }
    }
}
