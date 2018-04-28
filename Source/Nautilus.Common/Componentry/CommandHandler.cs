//--------------------------------------------------------------
// <copyright file="CommandHandler.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The <see cref="CommandHandler"/> class. Executes all encapsulated <see cref="Action"/>(s)
    /// and handles and logs all errors and exceptions. Validation exceptions are logged and swallowed,
    /// all other exceptions other than a specified exception type are logged and rethrown.
    /// </summary>
    public class CommandHandler
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CommandHandler(ILogger logger)
        {
            Validate.NotNull(logger, nameof(logger));

            this.logger = logger;
        }

        /// <summary>
        /// Executes the given action. Will catch and log all exceptions, will rethrow exceptions
        /// other than validation exceptions.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Execute(Action action)
        {
            Debug.NotNull(action, nameof(action));

            try
            {
                action.Invoke();
            }
            catch (ValidationException ex)
            {
                this.logger.LogException(ex);
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);

                throw;
            }
        }

        /// <summary>
        /// Executes the given action. Will catch and log all exceptions, will rethrow exceptions
        /// other than the specified exception type or validation exceptions.
        /// </summary>
        /// <typeparam name="T">The exception type.</typeparam>
        /// <param name="action">The action.</param>
        public void Execute<T>(Action action) where T : Exception
        {
            Debug.NotNull(action, nameof(action));

            try
            {
                action.Invoke();
            }
            catch (T ex)
            {
                this.logger.LogException(ex);
            }
            catch (ValidationException ex)
            {
                this.logger.LogException(ex);
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);

                throw;
            }
        }
    }
}
