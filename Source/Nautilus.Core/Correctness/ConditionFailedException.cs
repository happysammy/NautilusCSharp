//--------------------------------------------------------------------------------------------------
// <copyright file="ConditionFailedException.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Correctness
{
    using System;

    /// <summary>
    /// Represents an exception where a condition check has failed.
    /// </summary>
    public sealed class ConditionFailedException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionFailedException"/> class.
        /// </summary>
        /// <param name="inner">The inner exception.</param>
        public ConditionFailedException(ArgumentException inner)
            : base(inner.Message, inner)
        {
        }
    }
}
