//--------------------------------------------------------------------------------------------------
// <copyright file="ValidationException.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Validation
{
    using System;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Wraps all validation <see cref="ArgumentException"/>(s).
    /// </summary>
    [Immutable]
    public sealed class ValidationException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="ex">The inner exception.</param>
        public ValidationException(ArgumentException ex)
            : base(ex.Message, ex)
        {
            Debug.NotNull(ex, nameof(ex));
        }
    }
}
