//--------------------------------------------------------------------------------------------------
// <copyright file="QueryExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides useful <see cref="QueryResult{T}"/> extension methods.
    /// </summary>
    [Immutable]
    public static class QueryExtensions
    {
        /// <summary>
        /// On success returns a result with the function value, otherwise returns a failure
        /// result.
        /// </summary>
        /// <typeparam name="T">The T type.</typeparam>
        /// <typeparam name="TK">The K type.</typeparam>
        /// <param name="queryResult">The result (cannot be null).</param>
        /// <param name="func">The function (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<TK> OnSuccess<T, TK>(this QueryResult<T> queryResult, Func<T, TK> func)
        {
            Debug.NotNull(queryResult, nameof(queryResult));
            Debug.NotNull(func, nameof(func));

            return queryResult.IsFailure
                 ? QueryResult<TK>.Fail(queryResult.Message)
                 : QueryResult<TK>.Ok(func(queryResult.Value));
        }

        /// <summary>
        /// On success returns the function with the result value (otherwise returns a failed result).
        /// </summary>
        /// <typeparam name="T">The T type.</typeparam>
        /// <typeparam name="TK">The K type.</typeparam>
        /// <param name="queryResult">The result (cannot be null).</param>
        /// <param name="func">The function (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<TK> OnSuccess<T, TK>(this QueryResult<T> queryResult, Func<T, QueryResult<TK>> func)
        {
            Debug.NotNull(queryResult, nameof(queryResult));
            Debug.NotNull(func, nameof(func));

            return queryResult.IsFailure
                 ? QueryResult<TK>.Fail(queryResult.Message)
                 : func(queryResult.Value);
        }

        /// <summary>
        /// On success returns the function, otherwise returns a failure result.
        /// </summary>
        /// <typeparam name="T">The T type.</typeparam>
        /// <typeparam name="TK">The K type.</typeparam>
        /// <param name="queryResult">The result (cannot be null).</param>
        /// <param name="func">The function (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<TK> OnSuccess<T, TK>(this QueryResult<T> queryResult, Func<QueryResult<TK>> func)
        {
            Debug.NotNull(queryResult, nameof(queryResult));
            Debug.NotNull(func, nameof(func));

            return queryResult.IsFailure
                 ? QueryResult<TK>.Fail(queryResult.Message)
                 : func();
        }

        /// <summary>
        /// On success performs the given action, then returns the result.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="result">The result (cannot be null).</param>
        /// <param name="action">The action (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<T> OnSuccess<T>(this QueryResult<T> result, Action<T> action)
        {
            Debug.NotNull(result, nameof(result));
            Debug.NotNull(action, nameof(action));

            if (result.IsSuccess)
            {
                action(result.Value);
            }

            return result;
        }

        /// <summary>
        /// On failure performs the given action, then returns the result.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="result">The result (cannot be null).</param>
        /// <param name="action">The action (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<T> OnFailure<T>(this QueryResult<T> result, Action action)
        {
            Debug.NotNull(result, nameof(result));
            Debug.NotNull(action, nameof(action));

            if (result.IsFailure)
            {
                action();
            }

            return result;
        }

        /// <summary>
        /// On failure performs the given action, then returns the result.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="result">The result (cannot be null).</param>
        /// <param name="action">The action (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<T> OnFailure<T>(this QueryResult<T> result, Action<string> action)
        {
            Debug.NotNull(result, nameof(result));
            Debug.NotNull(action, nameof(action));

            if (result.IsFailure)
            {
                action(result.Message);
            }

            return result;
        }

        /// <summary>
        /// On success or failure returns the function.
        /// </summary>
        /// <typeparam name="T">The T type.</typeparam>
        /// <typeparam name="TK">The K type.</typeparam>
        /// <param name="result">The result (cannot be null).</param>
        /// <param name="func">The function (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static TK OnBoth<T, TK>(this QueryResult<T> result, Func<QueryResult<T>, TK> func)
        {
            Debug.NotNull(result, nameof(result));
            Debug.NotNull(func, nameof(func));

            return func(result);
        }

        /// <summary>
        /// Evaluates the result and predicate.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="result">The result (cannot be null).</param>
        /// <param name="predicate">The predicate (cannot be null).</param>
        /// <param name="errorMessage">The error message (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<T> Ensure<T>(this QueryResult<T> result, Func<T, bool> predicate, string errorMessage)
        {
            Debug.NotNull(result, nameof(result));
            Debug.NotNull(predicate, nameof(predicate));
            Debug.NotNull(errorMessage, nameof(errorMessage));

            if (result.IsFailure)
            {
                return QueryResult<T>.Fail(result.Message);
            }

            if (!predicate(result.Value))
            {
                return QueryResult<T>.Fail(errorMessage);
            }

            return QueryResult<T>.Ok(result.Value);
        }

        /// <summary>
        /// Maps the result result to successful result.
        /// </summary>
        /// <typeparam name="T">The T type.</typeparam>
        /// <typeparam name="TK">The K type.</typeparam>
        /// <param name="result">The result (cannot be null).</param>
        /// <param name="func">The function (cannot be null).</param>
        /// <returns>The queries result.</returns>
        public static QueryResult<TK> Map<T, TK>(this QueryResult<T> result, Func<T, TK> func)
        {
            Debug.NotNull(result, nameof(result));
            Debug.NotNull(func, nameof(func));

            return result.IsFailure
                 ? QueryResult<TK>.Fail(result.Message)
                 : QueryResult<TK>.Ok(func(result.Value));
        }
    }
}
