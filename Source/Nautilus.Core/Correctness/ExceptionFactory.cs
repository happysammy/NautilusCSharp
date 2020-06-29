//--------------------------------------------------------------------------------------------------
// <copyright file="ExceptionFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
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
