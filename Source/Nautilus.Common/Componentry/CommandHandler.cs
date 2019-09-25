//--------------------------------------------------------------------------------------------------
// <copyright file="CommandHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
            catch (NullReferenceException ex)
            {
                this.log.Error(ex.Message, ex);
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
        /// other than <see cref="ArgumentException"/>(s) and the specified <see cref="Exception"/> type.
        /// </summary>
        /// <typeparam name="T">The expected exception type.</typeparam>
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
        /// other than <see cref="ArgumentException"/>(s) and the specified expected <see cref="Exception"/> types.
        /// </summary>
        /// <typeparam name="T1">The first expected <see cref="Exception"/> type.</typeparam>
        /// <typeparam name="T2">The second expected <see cref="Exception"/> type.</typeparam>
        /// <param name="action">The action to invoke.</param>
        public void Execute<T1, T2>(Action action)
            where T1 : Exception
            where T2 : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T1 ex)
            {
                this.log.Error(ex.Message, ex);
            }
            catch (T2 ex)
            {
                this.log.Error(ex.Message, ex);
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
    }
}
