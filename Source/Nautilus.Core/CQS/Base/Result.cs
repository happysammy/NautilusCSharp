//--------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.CQS.Base
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// The base class for all <see cref="Result"/> types.
    /// </summary>
    [Immutable]
    public abstract class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="isFailure">The is failure boolean flag.</param>
        /// <param name="message">The message string.</param>
        protected Result(bool isFailure, string message)
        {
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));

            this.IsFailure = isFailure;
            this.IsSuccess = !isFailure;
            this.Message = message;
        }

        /// <summary>
        /// Gets a value indicating whether the result is a failure.
        /// </summary>
        public bool IsFailure { get; }

        /// <summary>
        /// Gets a value indicating whether the result is a success.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the result message.
        /// </summary>
        public string Message { get; }
    }
}
