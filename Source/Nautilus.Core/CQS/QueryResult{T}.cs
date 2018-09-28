//--------------------------------------------------------------------------------------------------
// <copyright file="QueryResult{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.CQS
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS.Base;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents the result of a query operation. The type may contain a result message.
    /// </summary>
    /// <typeparam name="T">The query type.</typeparam>
    [Immutable]
    public sealed class QueryResult<T> : Result
    {
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
        /// </summary>
        /// <param name="isFailure">The is failure flag.</param>
        /// <param name="value">The query value (cannot be null if successful).</param>
        /// <param name="error">The query error (cannot be null or white space).</param>
        private QueryResult(
            bool isFailure,
            [CanBeNull] T value,
            string error)
            : base(isFailure, error)
        {
            if (!isFailure)
            {
                Debug.NotNull(value, nameof(value));
                Debug.NotNull(error, nameof(error));
            }

            this.value = value;
        }

        /// <summary>
        /// Gets the value (value cannot be null).
        /// </summary>
        /// <returns>The query value.</returns>
        /// <exception cref="InvalidOperationException">If there is no value for a query failure.
        /// </exception>
        public T Value => this.IsSuccess
                        ? this.value
                        : throw new InvalidOperationException("There is no value for a query failure.");

        /// <summary>
        /// Create a success <see cref="QueryResult{T}"/> with the given value.
        /// </summary>
        /// <param name="value">The query value (cannot be null).</param>
        /// <returns>The query result of T.</returns>
        public static QueryResult<T> Ok(T value)
        {
            Debug.NotNull(value, nameof(value));

            return new QueryResult<T>(false, value, "No result message");
        }

        /// <summary>
        /// Create a success <see cref="QueryResult{T}"/> with the given value and message.
        /// </summary>
        /// <param name="value">The query value (cannot be null).</param>
        /// <param name="message">The query message (cannot be null, empty or white space).</param>
        /// <returns>The query result of T.</returns>
        public static QueryResult<T> Ok(T value, string message)
        {
            Debug.NotNull(value, nameof(value));
            Debug.NotNull(message, nameof(message));

            return new QueryResult<T>(false, value, message);
        }

        /// <summary>
        /// Create a failure <see cref="QueryResult{T}"/> with the given error message.
        /// </summary>
        /// <param name="error">The query error message (cannot be null, empty or white space).</param>
        /// <returns>The query result of T.</returns>
        public static QueryResult<T> Fail(string error)
        {
            Debug.NotNull(error, nameof(error));

            return new QueryResult<T>(true, default, error);
        }
    }
}
