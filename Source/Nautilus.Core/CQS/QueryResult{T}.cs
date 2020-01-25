//--------------------------------------------------------------------------------------------------
// <copyright file="QueryResult{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.CQS
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS.Base;

    /// <summary>
    /// Represents the result of a query operation. May contain a result message.
    /// </summary>
    /// <typeparam name="T">The query type.</typeparam>
    [Immutable]
    public sealed class QueryResult<T> : Result
    {
        #pragma warning disable 8618
        // Can only be accessed through Value property which checks has value.
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
        /// </summary>
        /// <param name="error">The query error (cannot be null or white space).</param>
        private QueryResult(string error)
            : base(true, error)
        {
            Debug.NotEmptyOrWhiteSpace(error, nameof(error));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
        /// </summary>
        /// <param name="value">The query value.</param>
        private QueryResult(T value)
            : base(false, "No result message")
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value of the query.
        /// </summary>
        /// <returns>The query value.</returns>
        /// <exception cref="InvalidOperationException">If the value of a query failure is accessed.</exception>
        public T Value => this.IsSuccess
                        ? this.value
                        : throw new InvalidOperationException("There is no value for a query failure.");

        /// <summary>
        /// Create a success <see cref="QueryResult{T}"/> with the given value.
        /// </summary>
        /// <param name="value">The query value.</param>
        /// <returns>The query result of T.</returns>
        public static QueryResult<T> Ok(T value)
        {
            return new QueryResult<T>(value);
        }

        /// <summary>
        /// Create a success <see cref="QueryResult{T}"/> with the given value and message.
        /// </summary>
        /// <param name="value">The query value.</param>
        /// <param name="message">The query message (cannot be null, empty or white space).</param>
        /// <returns>The query result of T.</returns>
        public static QueryResult<T> Ok(T value, string message)
        {
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));

            return new QueryResult<T>(value);
        }

        /// <summary>
        /// Create a failure <see cref="QueryResult{T}"/> with the given error message.
        /// </summary>
        /// <param name="error">The query error message (cannot be null, empty or white space).</param>
        /// <returns>The query result of T.</returns>
        public static QueryResult<T> Fail(string error)
        {
            Debug.NotEmptyOrWhiteSpace(error, nameof(error));

            return new QueryResult<T>(error);
        }
    }
}
