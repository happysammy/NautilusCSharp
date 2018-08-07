//--------------------------------------------------------------------------------------------------
// <copyright file="CommandHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// A class which provides encapsulated execution of <see cref="Action"/>(s)
    /// and handles and logs all errors and exceptions. Validation exceptions are logged and swallowed,
    /// all other exceptions other than a specified exception type are logged and rethrown.
    /// </summary>
    [Stateless]
    public class CommandHandler
    {
        private readonly ILogger log;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CommandHandler(ILogger logger)
        {
            Validate.NotNull(logger, nameof(logger));

            this.log = logger;
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
                this.log.Error(ex.Message, ex);
            }
            catch (Exception ex)
            {
                this.log.Fatal(ex.Message, ex);

                throw;
            }
        }

        /// <summary>
        /// Executes the given action. Will catch and log all exceptions, will rethrow exceptions
        /// other than the specified exception type or validation exceptions.
        /// </summary>
        /// <typeparam name="T">The exception type.</typeparam>
        /// <param name="action">The action.</param>
        public void Execute<T>(Action action)
            where T : Exception
        {
            Debug.NotNull(action, nameof(action));

            try
            {
                action.Invoke();
            }
            catch (T ex)
            {
                this.log.Error(ex.Message, ex);
            }
            catch (ValidationException ex)
            {
                this.log.Error(ex.Message, ex);
            }
            catch (Exception ex)
            {
                this.log.Fatal(ex.Message, ex);

                throw;
            }
        }
    }
}
