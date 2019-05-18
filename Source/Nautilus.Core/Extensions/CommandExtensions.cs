//--------------------------------------------------------------------------------------------------
// <copyright file="CommandExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;

    /// <summary>
    /// Provides useful <see cref="CommandResult"/> extension methods.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Passes the result to the given action if the result is a success.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="action">The action to perform on the result.</param>
        /// <returns>The original result.</returns>
        public static CommandResult OnSuccess(this CommandResult result, Action<CommandResult> action)
        {
            if (result.IsSuccess)
            {
                action(result);
            }

            return result;
        }

        /// <summary>
        /// Passes the result to the given action if the result is a failure.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="action">The action to perform on the result.</param>
        /// <returns>The original result.</returns>
        public static CommandResult OnFailure(this CommandResult result, Action<CommandResult> action)
        {
            if (result.IsFailure)
            {
                action(result);
            }

            return result;
        }

        /// <summary>
        /// Passes the result to the given action on either a success or failure.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="action">The action to perform on the result.</param>
        public static void OnBoth(this CommandResult result, Action<CommandResult> action)
        {
            action(result);
        }

        /// <summary>
        /// Invokes the given action for a successful <see cref="CommandResult"/>, then returns the
        /// <see cref="CommandResult"/>.
        /// </summary>
        /// <param name="result">The result to evaluate.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The result.</returns>
        public static CommandResult OnSuccess(this CommandResult result, Action action)
        {
            if (result.IsSuccess)
            {
                action();
            }

            return result;
        }

        /// <summary>
        /// Returns the result of calling the given function for a successful
        /// <see cref="CommandResult"/>>.
        /// </summary>
        /// <param name="result">The result to evaluate.</param>
        /// <param name="func">The function to call.</param>
        /// <returns>The commands result.</returns>
        public static CommandResult OnSuccess(this CommandResult result, Func<CommandResult> func)
        {
            return result.IsFailure
                ? result
                : func();
        }

        /// <summary>
        /// Returns the result of invoking the given action for a failed <see cref="CommandResult"/>.
        /// </summary>
        /// <param name="result">The result to evaluate.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The commands result.</returns>
        public static CommandResult OnFailure(this CommandResult result, Action action)
        {
            if (result.IsFailure)
            {
                action();
            }

            return result;
        }

        /// <summary>
        /// Returns the result of invoking the given action string for a failed
        /// <see cref="CommandResult"/>.
        /// </summary>
        /// <param name="result">The result to evaluated.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The commands result.</returns>
        public static CommandResult OnFailure(this CommandResult result, Action<string> action)
        {
            if (result.IsFailure)
            {
                action(result.Message);
            }

            return result;
        }

        /// <summary>
        /// Calls the given function on either a success or failure result result.
        /// </summary>
        /// <param name="result">The result to evaluate.</param>
        /// <param name="func">The function.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The command result of the given function.</returns>
        public static T OnBoth<T>(this CommandResult result, Func<CommandResult, T> func)
        {
            return func(result);
        }

        /// <summary>
        /// If the result is successful then calls the given function and returns a
        /// <see cref="QueryResult{T}"/>.
        /// </summary>
        /// <typeparam name="T">The query type.</typeparam>
        /// <param name="result">The result to evaluate.</param>
        /// <param name="func">The function to return.</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<T> OnSuccess<T>(this CommandResult result, Func<T> func)
        {
            return result.IsFailure
                 ? QueryResult<T>.Fail(result.Message)
                 : QueryResult<T>.Ok(func());
        }

        /// <summary>
        /// On success returns the given function, otherwise returns a failed <see cref="QueryResult{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="result">The result to evaluate.</param>
        /// <param name="func">The function to return.</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<T> OnSuccess<T>(this CommandResult result, Func<QueryResult<T>> func)
        {
            return result.IsFailure
                 ? QueryResult<T>.Fail(result.Message)
                 : func();
        }

        /// <summary>
        /// On success returns a success <see cref="QueryResult{T}"/> with the given function.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="result">The result to evaluate.</param>
        /// <param name="func">The function to return.</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<T> Map<T>(this CommandResult result, Func<T> func)
        {
            return result.IsFailure
                 ? QueryResult<T>.Fail(result.Message)
                 : QueryResult<T>.Ok(func());
        }

        /// <summary>
        /// Ensures a failed <see cref="CommandResult"/> result is returned on error, otherwise
        /// returns ok.
        /// </summary>
        /// <param name="result">The result to evaluate.</param>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="errorMessage">The error message string.</param>
        /// <returns>The commands result.</returns>
        public static CommandResult Ensure(this CommandResult result, Func<bool> predicate, string errorMessage)
        {
            Debug.NotEmptyOrWhiteSpace(errorMessage, nameof(errorMessage));

            if (result.IsFailure)
            {
                return CommandResult.Fail(result.Message);
            }

            return predicate()
                 ? CommandResult.Ok()
                 : CommandResult.Fail(errorMessage);
        }
    }
}
