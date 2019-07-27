//--------------------------------------------------------------------------------------------------
// <copyright file="ExceptionFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Correctness
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Provides common exception objects.
    /// </summary>
    public static class ExceptionFactory
    {
        /// <summary>
        /// Returns an ArgumentOutOfRangeException from the given arguments.
        /// </summary>
        /// <param name="argument">The out of range argument.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The exception to throw.</returns>
        public static ArgumentOutOfRangeException InvalidSwitchArgument(object argument, string paramName)
        {
            Debug.NotEmptyOrWhiteSpace(paramName, nameof(paramName));

            return new ArgumentOutOfRangeException(
                paramName,
                argument,
                $"The value of argument '{paramName}' of type {argument.GetType().Name} was invalid out of range for this switch.");
        }

        /// <summary>
        /// Returns an InvalidEnumArgumentException from the given arguments.
        /// </summary>
        /// <param name="value">The invalid enum value.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The exception to throw.</returns>
        public static InvalidEnumArgumentException InvalidSwitchArgument(Enum value, string paramName)
        {
            Debug.NotEmptyOrWhiteSpace(paramName, nameof(paramName));

            return new InvalidEnumArgumentException(
                paramName,
                Convert.ToInt32(value),
                value.GetType());
        }
    }
}
