//--------------------------------------------------------------------------------------------------
// <copyright file="ResultBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
//  https://github.com/nautechsystems/Nautilus.Core
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.CQS
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// The base class for all result types.
    /// </summary>
    [Immutable]
    public abstract class ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultBase"/> class.
        /// </summary>
        /// <param name="isFailure">The is failure boolean flag.</param>
        /// <param name="message">The message string.</param>
        protected ResultBase(bool isFailure, string message)
        {
            Debug.NotNull(message, nameof(message));

            this.IsFailure = isFailure;
            this.Message = message;
        }

        /// <summary>
        /// Gets a value indicating whether the result is a failure.
        /// </summary>
        public bool IsFailure { get; }

        /// <summary>
        /// Gets a value indicating whether the result is a success.
        /// </summary>
        public bool IsSuccess => !this.IsFailure;

        /// <summary>
        /// Gets the result message.
        /// </summary>
        public string Message { get; }
    }
}
