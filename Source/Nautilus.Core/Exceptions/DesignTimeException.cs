//--------------------------------------------------------------------------------------------------
// <copyright file="DesignTimeException.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Exceptions
{
    using System;

    /// <summary>
    /// Represents an exception where the operation has violated the design specification.
    /// </summary>
    public sealed class DesignTimeException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignTimeException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DesignTimeException(string message)
            : base(message)
        {
        }
    }
}
