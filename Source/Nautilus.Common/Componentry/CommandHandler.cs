//--------------------------------------------------------------------------------------------------
// <copyright file="CommandHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// A class which provides encapsulated execution of <see cref="Action"/>(s)
    /// and handles and logs all errors and exceptions. Validation exceptions are logged and swallowed,
    /// all other exceptions other than a specified exception type are logged and rethrown.
    /// </summary>
    public sealed class CommandHandler
    {
        private readonly ILogger log;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CommandHandler(ILogger logger)
        {
            this.log = logger;
        }

        /// <summary>
        /// Executes the given action. Will catch and log all exceptions, will rethrow exceptions
        /// other than <see cref="ArgumentException"/>.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        public void Execute(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (ArgumentException ex)
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
        /// <param name="action">The action to invoke.</param>
        public void Execute<T>(Action action)
            where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T ex)
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
